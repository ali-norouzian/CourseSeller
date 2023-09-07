using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _orderService.GetUserOrders(User.Identity.Name));
        }

        [Route("/[area]/[controller]/{orderId}")]
        public async Task<IActionResult> ShowOrder(int orderId)
        {
            var order = await _orderService.GetUserOrder(User.Identity.Name, orderId);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [Route("/[area]/[controller]/{orderId}/Finish")]
        public async Task<IActionResult> FinishOrder(int orderId)
        {
            var order = await _orderService.FinishOrder(User.Identity.Name, orderId);
            // Something went wrong
            if (order == false)
                return BadRequest();

            TempData["isFinish"] = true;
            return RedirectToAction(nameof(ShowOrder), new { orderId = orderId });
        }

        [HttpPost]
        [Route("/[area]/[controller]/{orderId}/UseDiscount")]
        public async Task<IActionResult> UseDiscount(int orderId, string discountCode)
        {
            var errType = await _orderService.UserDiscount(orderId, discountCode);


            TempData["errType"] = errType;


            return RedirectToAction(nameof(ShowOrder), new { orderId = orderId });
        }
    }
}
