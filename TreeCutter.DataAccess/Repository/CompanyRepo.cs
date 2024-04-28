using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.DataAccess.Data;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;

namespace TreeCutter.DataAccess.Repository
{
    public class CompanyRepo : Repository<Company>, ICompanyRepo
    {
        public CompanyRepo(ApplicationDbContext db) : base(db) { }

        public void Update(Company obj)
        {
            _db.Companies.Update(obj);
        }
    }
}
