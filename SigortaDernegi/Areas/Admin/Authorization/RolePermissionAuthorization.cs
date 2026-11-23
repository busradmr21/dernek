using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SigortaDernegi.Data;
using SigortaDernegi.Models;

namespace SigortaDernegi.Areas.Admin.Authorization
{
    public enum PermissionActionType
    {
        View = 1,
        Add = 2,
        Edit = 3,
        Approve = 4,
        Delete = 5
    }

    public interface IRolePermissionService
    {
        Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            ManagementArea area,
            PermissionActionType action);
    }

    public class RolePermissionService : IRolePermissionService
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            ManagementArea area,
            PermissionActionType action)
        {
            if (user.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            // Ana yönetici her zaman bütün yetkilere sahiptir.
            if (user.IsInRole(IdentitySeedData.AdminRole))
            {
                return true;
            }

            var userRoles = user.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .Distinct()
                .ToArray();

            if (userRoles.Length == 0)
            {
                return false;
            }

            var permissions = _context.RolePermissions
                .AsNoTracking()
                .Where(permission =>
                    userRoles.Contains(permission.RoleName) &&
                    permission.Area == area);

            permissions = action switch
            {
                PermissionActionType.View =>
                    permissions.Where(permission => permission.CanView),

                PermissionActionType.Add =>
                    permissions.Where(permission => permission.CanAdd),

                PermissionActionType.Edit =>
                    permissions.Where(permission => permission.CanEdit),

                PermissionActionType.Approve =>
                    permissions.Where(permission => permission.CanApprove),

                PermissionActionType.Delete =>
                    permissions.Where(permission => permission.CanDelete),

                _ => permissions.Where(permission => false)
            };

            return await permissions.AnyAsync();
        }
    }

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = true)]
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(
            ManagementArea area,
            PermissionActionType action)
            : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { area, action };
        }
    }

    public class RequirePermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly ManagementArea _area;
        private readonly PermissionActionType _action;
        private readonly IRolePermissionService _permissionService;

        public RequirePermissionFilter(
            ManagementArea area,
            PermissionActionType action,
            IRolePermissionService permissionService)
        {
            _area = area;
            _action = action;
            _permissionService = permissionService;
        }

        public async Task OnAuthorizationAsync(
            AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                context.Result = new ChallengeResult();
                return;
            }

            var hasPermission =
                await _permissionService.HasPermissionAsync(
                    user,
                    _area,
                    _action);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}