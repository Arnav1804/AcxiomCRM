using AxciomCRM.Models;
using AxciomCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AxciomCRM.Controllers;

[Authorize(Roles = AppConstants.Admin)]
public class PermissionsController : Controller
{
    public IActionResult Index()
    {
        var model = new List<PermissionViewModel>
        {
            new PermissionViewModel { Role = AppConstants.Admin, Access = "All modules, users, roles, permissions and audit log" },
            new PermissionViewModel { Role = AppConstants.Manager, Access = "Dashboard, customers, leads, opportunities, follow-ups and activities" },
            new PermissionViewModel { Role = AppConstants.SalesExecutive, Access = "Assigned customers, leads, opportunities, follow-ups and activities" }
        };

        return View(model);
    }
}
