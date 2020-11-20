using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarskoUdruzenjeV3.Areas.Identity.Data;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    [Authorize(Roles = "administrator")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int p=1)
        {

            int pageSize = 8;
            var users = await _userManager.Users
                .OrderByDescending(u => u.Id)
                .Skip((p - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (User user in users)
            {
                var thisViewModel = new UserRolesViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.PhoneNumber = user.PhoneNumber;
                thisViewModel.Roles = await GetUserRoles(user);
                thisViewModel.isActive = user.EmailConfirmed;
                userRolesViewModel.Add(thisViewModel);
            }

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_userManager.Users.Count() / pageSize);

            return View(userRolesViewModel);
        }

        private async Task<List<string>> GetUserRoles(User user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        public async Task<IActionResult> Activate (string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.EmailConfirmed = !user.EmailConfirmed;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
