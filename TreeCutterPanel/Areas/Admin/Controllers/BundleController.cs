using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Models.ViewModels;
using TreeCutter.Utility;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class BundleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BundleController(IUnitOfWork unitOfWork)
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
            ViewData["ProductSelectList"] = GetProductSelectList();

            if (id.GetValueOrDefault() == 0) return View();

            Bundle? bundleFromDb = _unitOfWork.Bundle.Get(u => u.Id == id, includeProperties: "Products");
            if (bundleFromDb == null) return NotFound();
            return View(new BundleVM
            {
                Model = bundleFromDb,
                ProductsId = bundleFromDb.Products.Select(u => u.Id).ToList()
            });
        }

        [HttpPost]
        public IActionResult Upsert(BundleVM input)
        {
            if (ModelState.IsValid)
            {
                //Create/Update model action
                if (input.Model.Id == 0)
                {
                    _unitOfWork.Bundle.Add(input.Model);
                } else
                {
                    _unitOfWork.Bundle.Update(input.Model);
                }
                _unitOfWork.Save();

                //Saving list of products
                if (input.ProductsId != null)
                {
                    input.Model.Products = _unitOfWork.Bundle.Get(u => u.Id == input.Model.Id, includeProperties: "Products", tracked: true).Products;
                    //Removing
                    foreach (Product product in input.Model.Products.ToList())
                    {
                        if (!input.ProductsId.Contains(product.Id))
                            input.Model.Products.Remove(product);
                    }
                    //Adding
                    foreach (int id in input.ProductsId)
                    {
                        if (input.Model.Products.FirstOrDefault(u => u.Id == id) != null) continue;
                        else
                        {
                            input.Model.Products.Add(_unitOfWork.Product.Get(u => u.Id == id, tracked: true));
                        }
                    }

                    _unitOfWork.Bundle.Update(input.Model);
                    _unitOfWork.Save();
                }

                TempData["message"] = "Bundle created successfully";
                TempData["messageType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                //Save all categories options in ViewData
                ViewData["CategorySelectList"] = GetCategorySelectList();
                ViewData["ProductSelectList"] = GetProductSelectList();
                return View(input);
            }
        }

        #region Service API

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Bundle> bundleList = _unitOfWork.Bundle.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = bundleList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Bundle? bundleFromDb = _unitOfWork.Bundle.Get(u => u.Id == id, includeProperties: "Products");
            if (bundleFromDb == null) return Json(new { success = false, message = "Cannot find object to delete" });

            //Remove bundle
            _unitOfWork.Bundle.Remove(bundleFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successfull" });
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
        private IEnumerable<SelectListItem> GetProductSelectList()
        {
            return _unitOfWork.Product.GetAll().Select(u => new SelectListItem
            {
                Text = $"{u.Id} | {u.Name} | {u.Category?.Name ?? "None"}",
                Value = u.Id.ToString()
            });
        }

        #endregion
    }
}
