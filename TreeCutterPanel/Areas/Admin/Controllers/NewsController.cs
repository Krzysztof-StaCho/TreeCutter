using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Utility;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class NewsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsController(IUnitOfWork unitOfWork)
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

            News? newsFromDb = _unitOfWork.News.Get(u => u.Id == id);
            if (newsFromDb == null) return NotFound();
            return View(newsFromDb);
        }

        [HttpPost]
        public IActionResult Upsert(News input)
        {
            if (ModelState.IsValid)
            {
                input.CreatedAt = DateTime.Now;
                input.Order = 0;

                if (input.Id == 0)
                {
                    _unitOfWork.News.Add(input);

                    TempData["message"] = "News created successfully";
                    TempData["messageType"] = "success";
                }
                else
                {
                    _unitOfWork.News.Update(input);

                    TempData["message"] = "News updated successfully";
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
            List<News> newsList = _unitOfWork.News.GetAll().ToList();
            return Json(new { data = newsList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            News? newsFromDb = _unitOfWork.News.Get(u => u.Id == id);
            if (newsFromDb == null) return Json(new { success = false, message = "Cannot find object to delete" });

            _unitOfWork.News.Remove(newsFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
