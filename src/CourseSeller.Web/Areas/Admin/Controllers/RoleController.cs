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
        private readonly IPermissionService _permissionService;

        public RoleController(IRoleService roleService, IPermissionService permissionService)
        {
            _roleService = roleService;
            _permissionService = permissionService;
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
            ViewData["Permissions"] = await _permissionService.GetAll();

            return View();
        }

        [HttpPost]
        [Route("/[area]/Roles/Create")]
        public async Task<IActionResult> CreateRole(Role viewModel, List<int> selectedPermission)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var roleId = await _roleService.AddRole(viewModel);
            await _permissionService.AddPermissionsToRole(roleId, selectedPermission);

            return RedirectToAction(nameof(Index));
        }

        [Route("/[area]/Roles/Edit/{id}")]
        public async Task<IActionResult> EditRole(int id)
        {
            ViewData["Permissions"] = await _permissionService.GetAll();
            ViewData["SelectedPermissions"] = await _permissionService.GetRoleSelectedPermissionsId(id);

            return View(await _roleService.GetRoleById(id));
        }

        [HttpPost]
        [Route("/[area]/Roles/Edit/{id}")]
        public async Task<IActionResult> EditRole(Role viewModel, List<int> selectedPermission)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            await _roleService.UpdateRole(viewModel);
            await _permissionService.UpdateRolePermission(viewModel.RoleID, selectedPermission);

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
