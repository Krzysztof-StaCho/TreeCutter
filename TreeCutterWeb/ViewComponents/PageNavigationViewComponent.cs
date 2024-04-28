using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Models.ViewModels;

namespace TreeCutterWeb.ViewComponents
{
    public class PageNavigationViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public PageNavigationViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            PageVM viewModel = new PageVM
            {
                Pages = _unitOfWork.Page.GetAll(u => u.GetType() == typeof(Page)),
                News = _unitOfWork.News.GetTop3()
            };
            return View(viewModel);
        }
    }
}
