using SigortaDernegi.Models;
using System.ComponentModel.DataAnnotations;

namespace SigortaDernegi.Areas.Admin.ViewModels;

public class RolePermissionsViewModel
{
    [Required]
    public string SelectedRoleName { get; set; } =
        string.Empty;

    public string SelectedRoleDisplayName { get; set; } =
        string.Empty;

    public string SelectedRoleDescription { get; set; } =
        string.Empty;

    public bool IsLocked { get; set; }

    public List<RolePermissionRoleViewModel> Roles
    { get; set; } = [];

    public List<RolePermissionItemViewModel> Permissions
    { get; set; } = [];
}

public class RolePermissionRoleViewModel
{
    public string RoleName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconClass { get; set; } = string.Empty;

    public int EnabledPermissionCount { get; set; }

    public int TotalPermissionCount { get; set; }

    public bool IsLocked { get; set; }
}

public class RolePermissionItemViewModel
{
    public ManagementArea Area { get; set; }

    public string AreaName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string IconClass { get; set; } = string.Empty;

    [Display(Name = "Görüntüle")]
    public bool CanView { get; set; }

    [Display(Name = "Ekle")]
    public bool CanAdd { get; set; }

    [Display(Name = "Düzenle")]
    public bool CanEdit { get; set; }

    [Display(Name = "Onayla")]
    public bool CanApprove { get; set; }

    [Display(Name = "Sil")]
    public bool CanDelete { get; set; }
}