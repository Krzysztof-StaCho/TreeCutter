using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Utility;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class PageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PageController(IUnitOfWork unitOfWork)
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

            Page? pageFromDb = _unitOfWork.Page.Get(u => u.Id == id);
            if (pageFromDb == null) return NotFound();
            return View(pageFromDb);
        }

        [HttpPost]
        public IActionResult Upsert(Page input)
        {
            if (ModelState.IsValid)
            {
                input.CreatedAt = DateTime.Now;

                if (input.Id == 0)
                {
                    _unitOfWork.Page.Add(input);

                    TempData["message"] = "Page created successfully";
                    TempData["messageType"] = "success";
                }
                else
                {
                    _unitOfWork.Page.Update(input);

                    TempData["message"] = "Page updated successfully";
                    TempData["messageType"] = "success";
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            else return View(input);
        }

        #region Service API

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Page> pageList = _unitOfWork.Page.GetAll().ToList();
            return Json(new { data = pageList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Page? pageFromDb = _unitOfWork.Page.Get(u => u.Id == id);
            if (pageFromDb == null) return Json(new { success = false, message = "Cannot find object to delete" });

            _unitOfWork.Page.Remove(pageFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
