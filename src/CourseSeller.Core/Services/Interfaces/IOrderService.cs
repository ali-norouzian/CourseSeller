using CourseSeller.DataLayer.Entities.Orders;

namespace CourseSeller.Core.Services.Interfaces;

public interface IOrderService
{
    Task<string> GetUserIdByUserName(string userName);
    Task<int> CreateOrder(string userName, int courseId);
    Task<Order> GetUserOrder(string userName, int orderId);
    Task<bool> FinishOrder(string userName, int orderId);

}