using CourseSeller.Core.DTOs.Order;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Courses;
using CourseSeller.DataLayer.Entities.Orders;
using CourseSeller.DataLayer.Entities.Users;
using CourseSeller.DataLayer.Entities.Wallets;
using Microsoft.EntityFrameworkCore;
using static CourseSeller.Core.Services.UserPanelService;

namespace CourseSeller.Core.Services;

public class OrderService : IOrderService
{
    private readonly MssqlContext _context;
    private readonly IUserPanelService _userPanelService;
    private readonly IAccountService _accountService;

    public OrderService(MssqlContext context, IUserPanelService userPanelService, IAccountService accountService)
    {
        _context = context;
        _userPanelService = userPanelService;
        _accountService = accountService;
    }

    public async Task<string> GetUserIdByUserName(string userName)
    {
        return (await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName)).UserId;
        ;
    }

    public async Task<int> CreateOrder(string userName, int courseId)
    {
        // Check user had oppened factor
        var userId = await GetUserIdByUserName(userName);
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.UserId == userId && o.IsFinished == false);
        var course = await _context.Courses.FindAsync(courseId);

        if (order == null)
        {
            order = new Order()
            {
                UserId = userId,
                IsFinished = false,
                CreateDateTime = DateTime.Now,
                OrderSum = course.CoursePrice,
                OrderDetails = new List<OrderDetail>()
                {
                    new OrderDetail()
                    {
                        CourseId = course.CourseId,
                        CurrentPrice = course.CoursePrice,
                        Count = 1,
                    }
                }
            };

            await _context.Orders.AddAsync(order);
        }
        else
        {
            // If product is exist in list. ++count
            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId &&
                                          o.CourseId == course.CourseId);
            // Its exist
            if (orderDetail != null)
            {
                orderDetail.Count++;
                order.OrderSum += orderDetail.CurrentPrice;
                _context.Update(orderDetail);
            }
            else
            {
                orderDetail = new OrderDetail()
                {
                    OrderId = order.OrderId,
                    Count = 1,
                    CourseId = course.CourseId,
                    CurrentPrice = course.CoursePrice,
                };
                order.OrderSum += orderDetail.CurrentPrice;
                await _context.OrderDetails.AddAsync(orderDetail);
            }
        }

        await _context.SaveChangesAsync();

        return order.OrderId;
    }

    public async Task<Order> GetOrderById(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<Order> GetUserOrder(string userName, int orderId)
    {
        var userId = await GetUserIdByUserName(userName);

        return await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Course)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderId == orderId);
    }

    public async Task UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> FinishOrder(string userName, int orderId)
    {
        var user = await _accountService.GetUserByUserName(userName);
        var userId = user.UserId;
        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Course)
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

        if (order == null || order.IsFinished)
            return false;

        // Cash is enought or not
        if (order.OrderSum <= await _userPanelService.GetUserBalance(userName))
        {
            order.IsFinished = true;

            var wallet = new Wallet()
            {
                Amount = -order.OrderSum,
                CreatedDateTime = DateTime.Now,
                Description = $"فاکتور شماره #{order.OrderId}",
                IsPaid = true,
                TypeId = WITHDRAWAL_TYPEID,
                UserId = userId,
            };

            await using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Perform your database operations within the transaction
                    // For example, you can add, update, or delete entities

                    // Add to UserCourse Table
                    var userCourse = new List<UserCourse>();
                    foreach (var od in order.OrderDetails)
                    {
                        userCourse.Add(new UserCourse()
                        {
                            CourseId = od.CourseId,
                            UserId = userId,
                        });
                    }

                    await _context.UserCourses.AddRangeAsync(userCourse);

                    _context.Orders.Update(order);

                    // Decrease from UserBalance
                    user.WalletBalance -= order.OrderSum;
                    _context.Users.Update(user);

                    // Decrease from wallet. (=> IT HAS #SAVECHANGES# <=)
                    await _userPanelService.AddWallet(wallet);


                    // Commit the transaction if everything succeeds
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception)
                {
                    // Handle exceptions or rollback the transaction if needed
                    await transaction.RollbackAsync();
                    throw; // Optional: rethrow the exception
                }
            }
        }

        return false;
    }

    public async Task<List<Order>> GetUserOrders(string userName)
    {
        var userId = await GetUserIdByUserName(userName);

        return await _context.Orders
            .Where(u => u.UserId == userId)
            .ToListAsync();
    }

    public async Task<DiscountErrorType> UserDiscount(int orderId, string code)
    {
        // Single mean that we have not duplicated column of code
        var discount = await _context.Discounts.SingleOrDefaultAsync(d => d.Code == code);
        if (discount == null)
            return DiscountErrorType.NotFound;

        if (discount.StartDateTime != null && discount.StartDateTime >= DateTime.Now)
            return DiscountErrorType.NotStarted;
        if (discount.EndDateTime != null && discount.EndDateTime <= DateTime.Now)
            return DiscountErrorType.FinishedTime;

        if (discount.UsableCount != null && discount.UsableCount < 1)
            return DiscountErrorType.Finished;

        var order = await GetOrderById(orderId);
        if (order.UsedDiscount)
            return DiscountErrorType.UserUsed;


        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                order.OrderSum = (order.OrderSum * (100 - discount.Percentage)) / 100;
                _context.Orders.Update(order);

                if (discount.UsableCount != null)
                {
                    discount.UsableCount--;
                    _context.Discounts.Update(discount);
                }

                await _context.SaveChangesAsync();

                // Commit the transaction if everything succeeds
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                // Handle exceptions or rollback the transaction if needed
                await transaction.RollbackAsync();
                throw; // Optional: rethrow the exception
            }


            return DiscountErrorType.Success;
        }
    }
}