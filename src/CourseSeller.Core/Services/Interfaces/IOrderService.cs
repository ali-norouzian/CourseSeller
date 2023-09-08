using CourseSeller.Core.DTOs.Order;
using CourseSeller.DataLayer.Entities.Orders;

namespace CourseSeller.Core.Services.Interfaces;

public interface IOrderService
{
    Task<string> GetUserIdByUserName(string userName);
    Task<int> CreateOrder(string userName, int courseId);
    Task<Order> GetOrderById(int id);
    Task<Order> GetUserOrder(string userName, int orderId);
    Task UpdateOrder(Order order);
    Task<bool> FinishOrder(string userName, int orderId);
    Task<List<Order>> GetUserOrders(string userName);
    Task<bool> UserHasCourse(string userName, int courseId);


    #region Discount

    Task<DiscountErrorType> UserDiscount(int orderId, string code);
    Task AddDiscount(Discount discount);
    Task<List<Discount>> GetAllDiscount();
    Task<Discount> GetDiscount(int discountId);
    Task UpdateDiscount(Discount discount);
    Task<bool> IsExistDiscountCode(string discountCode);

    #endregion

}