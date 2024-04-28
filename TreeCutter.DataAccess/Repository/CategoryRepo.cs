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
    public class CategoryRepo : Repository<Category>, ICategoryRepo
    {
        public CategoryRepo(ApplicationDbContext db) : base(db) { }

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }

        public override IEnumerable<Category> GetAll(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null)
        {
            return base.GetAll(filter, includeProperties).OrderBy(c => c.DisplayOrder);
        }
    }
}
