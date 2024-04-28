using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.Models;
using TreeCutter.Utility;
using TreeCutter.Models.ViewModels;

namespace TreeCutterPanel.Areas.Account.Controllers
{
    [Area("Account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _logger = logger;
        }

        #region Login Action

        public IActionResult Login(string? returnUrl = null)
        {
            LoginVM loginVM = new LoginVM()
            {
                ReturnUrl = returnUrl
            };

            return View(loginVM);
        }

        [HttpPost]
        public IActionResult Login(LoginVM Input)
        {
            Input.ReturnUrl ??= Url.Action("Index", "Home", new { area = "Customer" });
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
					_logger.LogInformation("User logged in.");
					TempData["message"] = "You are now logged in";
					TempData["messageType"] = "success";
					return Redirect(Input.ReturnUrl);
				}
                else if (result.IsLockedOut)
                {
                    _logger.LogInformation("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                } else
                {
                    TempData["message"] = "Invalid Username or Password";
                    TempData["messageType"] = "error";
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(Input);
                }
            }
            return View();
        }

        #endregion

        public IActionResult Logout(string? returnUrl = null)
        {
            _signInManager.SignOutAsync().GetAwaiter();
            _logger.LogInformation("User logged out");
            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            } else
            {
                return View();
            }
        }

        public IActionResult Lockout()
        {
            return View();
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helper Function

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            } catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'.");
            }
        }

        #endregion
    }
}
