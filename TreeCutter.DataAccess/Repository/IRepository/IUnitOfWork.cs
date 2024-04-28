using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepo User { get; }
        ICompanyRepo Company { get; }
        ICategoryRepo Category { get; }
        IProductRepo Product { get; }
        IBundleRepo Bundle { get; }
        IShoppingCartRepo ShoppingCart { get; }
        IOrderHeaderRepo OrderHeader { get; }
        IOrderDetailRepo OrderDetail { get; }
        IPageRepo Page { get; }
        INewsRepo News { get; }

        void Save();
    }
}
