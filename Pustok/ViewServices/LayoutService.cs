using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;
using Pustok.DAL;
using Pustok.Models;

namespace Pustok.ViewServices
{
    public class LayoutService
    {
        private readonly PustokContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService( PustokContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Setting>> GetBook()
        {
            var settings = _context.Settings.ToList();
            return settings;
        }

        public async Task<User> GetAppUser()
        {
            User user = null;
            //string userName = null;
            //if (_httpContextAccessor.HttpContext.User.Identity != null && _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            //}

            //if (userName != null)
            //{
            //    user = await _userManager.FindByNameAsync(userName);
            //}

            //string userName = null;
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
                //user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _httpContextAccessor.HttpContext.User.Identity.Name);
            }

            return user; 
        }

    }
}
