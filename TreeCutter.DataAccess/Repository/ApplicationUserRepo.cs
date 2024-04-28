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
    public class ApplicationUserRepo : Repository<ApplicationUser>, IApplicationUserRepo
    {
        public ApplicationUserRepo(ApplicationDbContext db) : base(db) { }

        public void Update(ApplicationUser obj)
        {
            _db.Users.Update(obj);
        }
    }
}
