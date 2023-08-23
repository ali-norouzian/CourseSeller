using CourseSeller.Core.Security;
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

        [PermissionChecker(PermissionCheckerAttribute.RoleManagementId)]
        [Route("[area]/Roles")]
        public async Task<IActionResult> Index()
        {
            ViewData["RoleList"] = await _roleService.GetAllRoles();

            return View();
        }

        [PermissionChecker(PermissionCheckerAttribute.CreateRoleId)]
        [Route("/[area]/Roles/Create")]
        public async Task<IActionResult> CreateRole()
        {
            ViewData["Permissions"] = await _permissionService.GetAll();

            return View();
        }

        [PermissionChecker(PermissionCheckerAttribute.CreateRoleId)]
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

        [PermissionChecker(PermissionCheckerAttribute.EditRoleId)]
        [Route("/[area]/Roles/Edit/{id}")]
        public async Task<IActionResult> EditRole(int id)
        {
            ViewData["Permissions"] = await _permissionService.GetAll();
            ViewData["SelectedPermissions"] = await _permissionService.GetRoleSelectedPermissionsId(id);

            return View(await _roleService.GetRoleById(id));
        }

        [PermissionChecker(PermissionCheckerAttribute.EditRoleId)]
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

        [PermissionChecker(PermissionCheckerAttribute.DeleteRoleId)]
        [Route("/[area]/Roles/Delete/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            return View(await _roleService.GetRoleById(id));
        }

        [PermissionChecker(PermissionCheckerAttribute.DeleteRoleId)]
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
