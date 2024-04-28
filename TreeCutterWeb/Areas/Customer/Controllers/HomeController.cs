using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Models.ViewModels;
using TreeCutter.Utility;

namespace TreeCutterWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? id)
        {
            CustomerHomeVM viewModel;

            //Get all bundle and product
            if (id.GetValueOrDefault() == 0)
            {
                viewModel = new CustomerHomeVM
                {
                    BundleList = _unitOfWork.Bundle.GetAll(b => b.IsEnabled, includeProperties: "Category"),
                    ProductList = _unitOfWork.Product.GetAll(p => p.IsEnabled, includeProperties: "Category")
                };
            } else if (id.GetValueOrDefault() == -1)
            {
                viewModel = new CustomerHomeVM
                {
                    BundleList = _unitOfWork.Bundle.GetAll(b => b.IsEnabled && b.IsPromoted, includeProperties: "Category"),
                    ProductList = _unitOfWork.Product.GetAll(p => p.IsEnabled && p.IsPromoted, includeProperties: "Category")
                };
            } else
            {
                viewModel = new CustomerHomeVM
                {
                    BundleList = _unitOfWork.Bundle.GetAll(b => b.IsEnabled && b.CategoryId == id, includeProperties: "Category"),
                    ProductList = _unitOfWork.Product.GetAll(p => p.IsEnabled && p.CategoryId == id, includeProperties: "Category")
                };
            }
            ViewData["ActiveCategory"] = id;
            return View(viewModel);
        }

        public IActionResult ProductDetails(int id)
        {
            Product? itemDetails = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (itemDetails == null) return NotFound();

            ShoppingCart viewModel = new ShoppingCart
            {
                Item = itemDetails,
                Count = 1,
                ItemId = id
            };
            return View(viewModel);
        }

        public IActionResult BundleDetails(int id)
        {
            Bundle? itemDetails = _unitOfWork.Bundle.Get(u => u.Id == id, includeProperties: "Category,Products");
            if (itemDetails == null) return NotFound();

            ShoppingCart viewModel = new ShoppingCart
            {
                Item = itemDetails,
                Count = 1,
                ItemId = id
            };
            return View(viewModel);
        }

        [HttpPost][Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            ClaimsIdentity claimsIdentity = (User.Identity as ClaimsIdentity) ?? throw new Exception("Cannot get claims identity #1");
            string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Cannot get name identifier value #2");
            cart.ApplicationUserId = userId;
            cart.Id = 0;

            ShoppingCart? cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&  u.ItemId == cart.ItemId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            } else
            {
                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(MiscConstant.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }

            TempData["message"] = "Cart updated successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
