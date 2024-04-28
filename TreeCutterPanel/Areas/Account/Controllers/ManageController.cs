using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.Models;
using TreeCutter.Models.ViewModels;

namespace TreeCutterWeb.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Index Action

        public IActionResult Index()
        {
            ApplicationUser? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'");
            }

            ManageProfileVM manageProfileVM = new ManageProfileVM
            {
                Username = _userManager.GetUserNameAsync(user).GetAwaiter().GetResult() ?? string.Empty,
                PhoneNumber = _userManager.GetPhoneNumberAsync(user).GetAwaiter().GetResult() ?? string.Empty,
                StreetAddress = user.StreetAddress,
                City = user.City,
                PostalCode = user.PostalCode,
                State = user.State
            };

            return View(manageProfileVM);
        }

        [HttpPost]
        public IActionResult Index(ManageProfileVM Input)
        {
            ApplicationUser? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            string phoneNumber = _userManager.GetPhoneNumberAsync(user).GetAwaiter().GetResult() ?? string.Empty;
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneNumberResult = _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber).GetAwaiter().GetResult();
                if (!setPhoneNumberResult.Succeeded)
                {
                    TempData["message"] = "Unexpected error when trying to set phone number.";
                    TempData["messageType"] = "error";
                    return View();
                }
            }
            var setUserNameResult = _userManager.SetUserNameAsync(user, Input.Username).GetAwaiter().GetResult();
            if (!setUserNameResult.Succeeded)
            {
                TempData["message"] = "Unexpected error when trying to set username.";
                TempData["messageType"] = "error";
                return View();
            }

            user.StreetAddress = Input.StreetAddress;
            user.City = Input.City;
            user.State = Input.State;
            user.PostalCode = Input.PostalCode;
            _userManager.UpdateAsync(user).GetAwaiter().GetResult();

            _signInManager.RefreshSignInAsync(user).GetAwaiter().GetResult();
            TempData["message"] = "Your profile has been update";
            TempData["messageType"] = "success";
            return View();
        }

        #endregion
        #region Email Action

        public IActionResult Email()
        {
            ApplicationUser? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            EmailVM emailVM = new EmailVM
            {
                Email = _userManager.GetEmailAsync(user).GetAwaiter().GetResult() ?? string.Empty,
            };

            return View(emailVM);
        }

        [HttpPost]
        public IActionResult Email(EmailVM Input)
        {
            ApplicationUser? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (!ModelState.IsValid)
            {
                return View(Input);
            }

            string emailFromDb = _userManager.GetEmailAsync(user).GetAwaiter().GetResult() ?? string.Empty;
            if (Input.NewEmail != emailFromDb)
            {
                var setEmailResult = _userManager.SetEmailAsync(user, Input.NewEmail).GetAwaiter().GetResult();
                if (!setEmailResult.Succeeded)
                {
                    TempData["message"] = "Unexpected error when trying to set new email.";
                    TempData["messageType"] = "error";
                    return View(Input);
                }
                TempData["message"] = "Your email has been changed";
                TempData["messageType"] = "success";
                Input.Email = Input.NewEmail;
                return View(Input);
            }

            TempData["message"] = "Your email is unchanged.";
            TempData["messageType"] = "info";
            return View(Input);
        }

        #endregion
        #region Password Action

        public IActionResult Password()
        {
            ApplicationUser? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Password(PasswordVM Input)
        {
            if (!ModelState.IsValid)
            {
                return View(Input);
            }

            ApplicationUser? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword).GetAwaiter().GetResult();
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(Input);
            }

            _signInManager.RefreshSignInAsync(user).GetAwaiter().GetResult();
            TempData["message"] = "Your password has been changed";
            TempData["messageType"] = "success";

            return View();
        }

        #endregion
    }
}
