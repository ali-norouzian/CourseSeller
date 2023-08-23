using CourseSeller.Core.DTOs.Admin;
using CourseSeller.Core.Services;
using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseSeller.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Route("[area]/Roles")]
        public async Task<IActionResult> Index()
        {
            ViewData["RoleList"] = await _roleService.GetAllRoles();

            return View();
        }

        [Route("/[area]/Roles/Create")]
        public async Task<IActionResult> CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Route("/[area]/Roles/Create")]
        public async Task<IActionResult> CreateRole(Role viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var roleId = await _roleService.AddRole(viewModel);

            return RedirectToAction(nameof(Index));
        }

        [Route("/[area]/Roles/Edit/{id}")]
        public async Task<IActionResult> EditRole(int id)
        {
            return View(await _roleService.GetRoleById(id));
        }

        [HttpPost]
        [Route("/[area]/Roles/Edit/{id}")]
        public async Task<IActionResult> EditRole(Role viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _roleService.UpdateRole(viewModel);

            return RedirectToAction(nameof(Index));
        }

        [Route("/[area]/Roles/Delete/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            return View(await _roleService.GetRoleById(id));
        }

        [HttpPost]
        [Route("/[area]/Roles/Delete/{id}")]
        public async Task<IActionResult> DeleteRole(Role viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _roleService.DeleteRole(viewModel);

            return RedirectToAction(nameof(Index));
        }
    }
}
