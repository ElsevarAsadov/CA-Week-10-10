using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.Core.Models;
using Pustok.ViewModels;

namespace Pustok.Controllers
{
    public class UserAccountController : Controller
    {
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<User> _signInManager;

		public UserAccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
		}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel memberLoginVM)
        {
            if(!ModelState.IsValid) return View();
            User user = null;

            user = await _userManager.FindByNameAsync(memberLoginVM.Username);

            if(user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var result =  await _signInManager.PasswordSignInAsync(user, memberLoginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            return RedirectToAction("index", "home");

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterVM)
        {
            if(!ModelState.IsValid) return View();
            User user = null;

            user = await _userManager.FindByNameAsync(memberRegisterVM.Username);

            if(user is not null)
            {
                ModelState.AddModelError("Username", "Username already exist!");
                return View();
            }

            user = await _userManager.FindByEmailAsync(memberRegisterVM.Email);

            if(user is not null )
            {
				ModelState.AddModelError("Email", "Email already exist!");
				return View();
			}

            User user = new User
            {
                Fullname = memberRegisterVM.Fullname,
                UserName = memberRegisterVM.Username,
                Email = memberRegisterVM.Email,
                BirthDate = memberRegisterVM.Birthdate,
            };

            var result = await _userManager.CreateAsync(user,memberRegisterVM.Password);

            if(!result.Succeeded) 
            {
                foreach (var err in result.Errors)
                {
					ModelState.AddModelError("", err.Description);
					return View();
				}
            }

            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index","home");
        }
    }
}
