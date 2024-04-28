using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;

namespace TreeCutterWeb.ViewComponents
{
    public class ShopMenuViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShopMenuViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_unitOfWork.Category.GetAll());
        }
    }
}
