using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Core.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminDashboardController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser admin = new AppUser()
        //    {
        //        Fullname = "Ramzi Hasanov",
        //        UserName = "Ramzes",
        //        BirthDate = "18 fevral"
        //    };

        //    var result = await _userManager.CreateAsync(admin, "Admin123@");


        //    return Ok(result);
        //}

        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityRole role1 = new IdentityRole("SuperAdmin");
        //    IdentityRole role2 = new IdentityRole("Admin");
        //    IdentityRole role3 = new IdentityRole("Editor");
        //    IdentityRole role4 = new IdentityRole("Member");

        //    await _roleManager.CreateAsync(role1);
        //    await _roleManager.CreateAsync(role2);
        //    await _roleManager.CreateAsync(role3);
        //    await _roleManager.CreateAsync(role4);

        //    return Ok("Yarandi");
        //}

        //public async Task<IActionResult> AddRoleAdmin()
        //{
        //    AppUser admin = await _userManager.FindByNameAsync("ramzes");

        //    await _userManager.AddToRoleAsync(admin, "Admin");

        //    return Ok("Add Olundu");
        //}
    }
}
