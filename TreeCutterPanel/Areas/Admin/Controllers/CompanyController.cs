using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Utility;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = ApplicationRoles.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Company { Name = "" });
            } else
            {
                Company? company = _unitOfWork.Company.Get(u => u.Id == id);
                if (company == null)
                {
                    TempData["message"] = "Cannot find this company inside database #1";
                    TempData["messageType"] = "error";
                    return RedirectToAction(nameof(Index));
                }
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);

                    TempData["message"] = "Company created successfully";
                    TempData["messageType"] = "success";
                } else
                {
                    _unitOfWork.Company.Update(obj);

                    TempData["message"] = "Company updated successfully";
                    TempData["messageType"] = "success";
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            } else
            {
                TempData["message"] = "Test error";
                TempData["messageType"] = "error";

                return View(obj);
            }
        }

        #region Service API

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Company? companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyToBeDeleted == null)
            {
                TempData["message"] = "Cannot find this company inside database #2";
                TempData["messageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Company.Remove(companyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted successfull" });
        }

        #endregion
    }
}
