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
    public class NewsRepo : Repository<News>, INewsRepo
    {
        public NewsRepo(ApplicationDbContext db) : base(db) { }

        public void Update(News obj)
        {
            _db.News.Update(obj);
        }

        public IEnumerable<News> GetTop3(Expression<Func<News, bool>>? filter = null, string? includeProperties = null)
        {
            return GetAll(filter, includeProperties).Take(3);
        }

        public override IEnumerable<News> GetAll(Expression<Func<News, bool>>? filter = null, string? includeProperties = null)
        {
            return base.GetAll(filter, includeProperties).OrderBy(n => n.CreatedAt);
        }
    }
}
