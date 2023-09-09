using CourseSeller.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseSeller.Core.Security;

public class PermissionCheckerAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    public const byte PanelManagementId = 1;
    public const byte UsersManagementId = 2;
    public const byte CreateUserId = 3;
    public const byte EditUserId = 4;
    public const byte DeleteUserId = 5;
    public const byte RoleManagementId = 6;
    public const byte CreateRoleId = 7;
    public const byte EditRoleId = 8;
    public const byte DeleteRoleId = 9;

    private readonly int _permissionId;
    private IPermissionService _permissionService;

    public PermissionCheckerAttribute(int permissionId)
    {
        _permissionId = permissionId;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        _permissionService =
            (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));

        var userName = context.HttpContext.User.Identity.Name;
        if (!await _permissionService.UserHasPermission(_permissionId, userName))
            context.Result = new RedirectResult($"/Account/Login?ReturnUrl={context.HttpContext.Request.Path}");
    }
}
