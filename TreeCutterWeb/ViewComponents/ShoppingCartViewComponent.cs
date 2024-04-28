using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Utility;

namespace TreeCutterWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
            Claim? claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(MiscConstant.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(MiscConstant.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }

                return View(HttpContext.Session.GetInt32(MiscConstant.SessionCart));
            } else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
