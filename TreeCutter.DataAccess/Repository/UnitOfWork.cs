using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.DataAccess.Data;
using TreeCutter.DataAccess.Repository.IRepository;

namespace TreeCutter.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public IApplicationUserRepo User { get; private set; }
        public ICompanyRepo Company { get; private set; }
        public ICategoryRepo Category { get; private set; }
        public IProductRepo Product { get; private set; }
        public IBundleRepo Bundle { get; private set; }
        public IShoppingCartRepo ShoppingCart { get; private set; }
        public IOrderHeaderRepo OrderHeader { get; private set; }
        public IOrderDetailRepo OrderDetail { get; private set; }
        public IPageRepo Page { get; private set; }
        public INewsRepo News { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            User = new ApplicationUserRepo(db);
            Company = new CompanyRepo(db);
            Category = new CategoryRepo(db);
            Product = new ProductRepo(db);
            Bundle = new BundleRepo(db);
            ShoppingCart = new ShoppingCartRepo(db);
            OrderHeader = new OrderHeaderRepo(db);
            OrderDetail = new OrderDetailRepo(db);
            Page = new PageRepo(db);
            News = new NewsRepo(db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
