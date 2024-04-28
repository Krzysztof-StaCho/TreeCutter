using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.DataAccess.Data;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;

namespace TreeCutter.DataAccess.Repository
{
    public class ProductRepo : Repository<Product>, IProductRepo
    {
        public ProductRepo(ApplicationDbContext db) : base(db) { }

        public void Update(Product obj)
        {
            _db.Products.Update(obj);
        }

        public override IEnumerable<Product> GetAll(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null)
        {
            return base.GetAll(filter, includeProperties).OrderBy(p => p.IsPromoted);
        }
    }
}
