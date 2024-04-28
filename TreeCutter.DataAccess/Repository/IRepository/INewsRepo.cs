using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.Models;

namespace TreeCutter.DataAccess.Repository.IRepository
{
    public interface INewsRepo : IRepository<News>
    {
        void Update(News obj);
        IEnumerable<News> GetTop3(Expression<Func<News, bool>>? filter = null, string? includeProperties = null);
    }
}
