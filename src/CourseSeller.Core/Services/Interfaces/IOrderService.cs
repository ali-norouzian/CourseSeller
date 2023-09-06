namespace CourseSeller.Core.Services.Interfaces;

public interface IOrderService
{
    Task<int> CreateOrder(string userName, int courseId);

}