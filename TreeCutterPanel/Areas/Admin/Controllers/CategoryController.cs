using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Utility;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            if (id.GetValueOrDefault() == 0) return View();

            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null) return NotFound();
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Upsert(Category input)
        {
            if (ModelState.IsValid)
            {
                if (input.Id == 0)
                {
                    _unitOfWork.Category.Add(input);

                    TempData["message"] = "Category created successfully";
                    TempData["messageType"] = "success";
                } else
                {
                    _unitOfWork.Category.Update(input);

                    TempData["message"] = "Category updated successfully";
                    TempData["messageType"] = "success";
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else return View();
        }

        #region Service API

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> categoryList = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = categoryList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            if (categoryFromDb == null) return Json(new { success = false, message = "Cannot find object to delete" });

            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
