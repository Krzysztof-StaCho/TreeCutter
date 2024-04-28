using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;

namespace TreeCutterWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            Page? firstPage = _unitOfWork.Page.Get(u => true);
            if (firstPage == null) return NotFound();

            ViewData["ActivePage"] = firstPage.Id;
            return View("ShowPage", firstPage);
        }

        public IActionResult ShowPage(int id)
        {
            if (id < 1) return NotFound();

            Page? pageFromDb = _unitOfWork.Page.Get(u => u.Id == id);
            if (pageFromDb == null) return NotFound();

            ViewData["ActivePage"] = id;
            return View(pageFromDb);
        }
    }
}
