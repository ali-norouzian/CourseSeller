using CourseSeller.Core.Convertors;
using CourseSeller.Core.Security;
using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Discount = CourseSeller.DataLayer.Entities.Orders.Discount;

namespace CourseSeller.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [PermissionChecker(PermissionCheckerAttribute.DiscountManagement)]
    public class DiscountController : Controller
    {
        private readonly IOrderService _orderService;

        public DiscountController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _orderService.GetAllDiscount());
        }

        private async Task<(DateTime?, DateTime?)> ConvertShamsiToGregorian(string? stDate, string? edDate)
        {
            if (stDate == null || edDate == null)
                return (null, null);
            DateTime? StartDateTime = null, EndDateTime = null;
            if (!string.IsNullOrEmpty(stDate))
                StartDateTime = stDate.ShamsiToGregorian();

            if (!string.IsNullOrEmpty(edDate))
                EndDateTime = edDate.ShamsiToGregorian();

            return (StartDateTime, EndDateTime);
        }

        [Route("[area]/[controller]/Create")]
        public async Task<IActionResult> CreateDisount()
        {
            return View();
        }

        [HttpPost]
        [Route("[area]/[controller]/Create")]
        public async Task<IActionResult> CreateDisount(Discount discount, string? stDate, string? edDate)
        {
            (discount.StartDateTime, discount.EndDateTime) = await ConvertShamsiToGregorian(stDate, edDate);

            if (!ModelState.IsValid && await _orderService.IsExistDiscountCode(discount.Code))
                return View(discount);

            await _orderService.AddDiscount(discount);

            return RedirectToAction(nameof(Index));
        }

        [Route("[area]/[controller]/Update/{discountId}")]
        public async Task<IActionResult> UpdateDisount(int discountId)
        {
            var discount = await _orderService.GetDiscount(discountId);
            return View("CreateDisount", discount);
        }

        [HttpPost]
        [Route("[area]/[controller]/Update/{discountId}")]
        public async Task<IActionResult> UpdateDisount(Discount discount, string? stDate, string? edDate)
        {
            (discount.StartDateTime, discount.EndDateTime) = await ConvertShamsiToGregorian(stDate, edDate);

            if (!ModelState.IsValid && await _orderService.IsExistDiscountCode(discount.Code))
                return View("CreateDisount", discount);

            await _orderService.UpdateDiscount(discount);

            return RedirectToAction(nameof(Index));
        }

        [Route("[area]/[controller]/CheckDiscountCodeIsExist/{discountCode}")]
        public async Task<IActionResult> CheckDiscountCodeIsExist(string discountCode)
        {
            return Content((await _orderService.IsExistDiscountCode(discountCode)).ToString());
        }
    }
}
