using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class OrderService : IOrderService
{
    private readonly MssqlContext _context;

    public OrderService(MssqlContext context)
    {
        _context = context;
    }

    public async Task<int> CreateOrder(string userName, int courseId)
    {
        // Check user had oppened factor
        var userId = (await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName)).UserId;
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
}