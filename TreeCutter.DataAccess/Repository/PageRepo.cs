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
    public class PageRepo : Repository<Page>, IPageRepo
    {
        public PageRepo(ApplicationDbContext db) : base(db) { }

        public void Update(Page obj)
        {
            _db.Pages.Update(obj);
        }

        public override IEnumerable<Page> GetAll(Expression<Func<Page, bool>>? filter = null, string? includeProperties = null)
        {
            return base.GetAll(filter, includeProperties).OrderBy(p => p.Order);
        }
    }
}
