using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeCutter.DataAccess.Data;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Utility;
using TreeCutter.Models.ViewModels;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class UserController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork, IUserStore<ApplicationUser> userStore)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userStore = userStore;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            ApplicationUser? userFromDb = _unitOfWork.User.Get(user => user.Id == userId, includeProperties: "Company");
            if (userFromDb == null)
            {
                TempData["message"] = "Cannot find this user inside database";
                TempData["messageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = userFromDb,
                RoleList = _roleManager.Roles.Select(role => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(company => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = company.Name,
                    Value = company.Id.ToString()
                })
            };

            RoleVM.NewRole = _userManager.GetRolesAsync(userFromDb).GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleVM)
        {
            ApplicationUser? applicationUser = _unitOfWork.User.Get(u => u.Id == roleVM.ApplicationUser.Id);
            if (applicationUser == null)
            {
                TempData["message"] = "Error while updating permission #1";
                TempData["messageType"] = "error";
                return RedirectToAction(nameof(Index));
            }
            string? oldRole = _userManager.GetRolesAsync(roleVM.ApplicationUser).GetAwaiter().GetResult().FirstOrDefault();

            //User choose different role
            if (oldRole == null || roleVM.NewRole != oldRole)
            {
                //User choose company role
                if (roleVM.NewRole == ApplicationRoles.Role_Company)
                {
                    applicationUser.CompanyId = roleVM.ApplicationUser.CompanyId;
                }
                //User has before company role
                if (oldRole == ApplicationRoles.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.User.Update(applicationUser);
                _unitOfWork.Save();

                //Changing role
                if (oldRole != null)
                {
                    _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(applicationUser, roleVM.NewRole).GetAwaiter().GetResult();
                }

                TempData["message"] = "Successfully changed role";
                TempData["messageType"] = "success";
            }
            //User choose another company
            else if (oldRole == ApplicationRoles.Role_Company && applicationUser.CompanyId != roleVM.ApplicationUser.CompanyId)
            {
                applicationUser.CompanyId = roleVM.ApplicationUser.CompanyId;
                _unitOfWork.User.Update(applicationUser);
                _unitOfWork.Save();

                TempData["message"] = "Successfully changed company";
                TempData["messageType"] = "success";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateUser()
        {
            CreateUserVM viewModel = new CreateUserVM()
            {
                Input = new RegisterVM(),
                RoleList = _roleManager.Roles.Select(role => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(company => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = company.Name,
                    Value = company.Id.ToString()
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateUser(CreateUserVM viewModel)
        {
            //If user have company role but not have assign company
            if (viewModel.NewRole == ApplicationRoles.Role_Company && viewModel.CompanyId.GetValueOrDefault() == 0)
            {
                ModelState.AddModelError("", "Company must be choose when role is 'Company'");
            }
            if (ModelState.IsValid)
            {
                //Creating account
                ApplicationUser user = CreateUserFunc();

                _userStore.SetUserNameAsync(user, viewModel.Input.Name, CancellationToken.None);

                user.Email = viewModel.Input.Email;
                user.StreetAddress = viewModel.Input.StreetAddress;
                user.City = viewModel.Input.City;
                user.PostalCode = viewModel.Input.PostalCode;
                user.State = viewModel.Input.State;
                user.PhoneNumber = viewModel.Input.PhoneNumber;

                //Account have role company and choosed company
                if (viewModel.CompanyId.GetValueOrDefault() != 0 && viewModel.NewRole == ApplicationRoles.Role_Company)
                {
                    user.CompanyId = viewModel.CompanyId;
                }

                IdentityResult result = _userManager.CreateAsync(user, viewModel.Input.Password).GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, viewModel.NewRole).GetAwaiter().GetResult();

                    TempData["message"] = $"Created account with role = {viewModel.NewRole}";
                    TempData["messageType"] = "success";

                    return RedirectToAction(nameof(Index));
                }

                TempData["message"] = "Cannot created account #1";
                TempData["messageType"] = "error";
                ModelState.AddModelError("", result.Errors.First().Description);
            }

            CreateUserVM newViewModel = new CreateUserVM()
            {
                Input = viewModel.Input,
                RoleList = _roleManager.Roles.Select(role => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(company => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = company.Name,
                    Value = company.Id.ToString()
                })
            };

            return View(newViewModel);
        }

        #region Service API

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.User.GetAll(includeProperties: "Company").ToList();

            foreach (var user in userList)
            {
                user.Company ??= new() { Name = "" };
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                user.Role ??= "";
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var userFromDb = _unitOfWork.User.Get(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                //User is locked
                userFromDb.LockoutEnd = DateTime.Now;
            } else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(10);
            }
            _unitOfWork.User.Update(userFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation successfull" });
        }

        #endregion
        #region Helper Function

        private ApplicationUser CreateUserFunc()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'.");
            }
        }

        #endregion
    }
}
