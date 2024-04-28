using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Utility;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            //Save all categories options in ViewData
            ViewData["CategorySelectList"] = GetCategorySelectList();

            if (id.GetValueOrDefault() == 0) return View();

            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if (productFromDb == null) return NotFound();
            return View(productFromDb);
        }

        [HttpPost]
        public IActionResult Upsert(Product input)
        {
            if (ModelState.IsValid)
            {
                //Create/Update model action
                if (input.Id == 0)
                {
                    _unitOfWork.Product.Add(input);
                } else
                {
                    _unitOfWork.Product.Update(input);
                }
                _unitOfWork.Save();

                TempData["message"] = "Product created successfully";
                TempData["messageType"] = "success";

                return RedirectToAction(nameof(Index));
            } else
            {
                //Save all categories options in ViewData
                ViewData["CategorySelectList"] = GetCategorySelectList();
                return View(input);
            }
        }

        #region Service API

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = productList });
        }

        #endregion
        #region Helper Function

        private IEnumerable<SelectListItem> GetCategorySelectList()
        {
            return _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

        #endregion
    }
}
