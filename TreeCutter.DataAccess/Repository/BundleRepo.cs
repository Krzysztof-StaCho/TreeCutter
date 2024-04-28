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
    public class BundleRepo : Repository<Bundle>, IBundleRepo
    {
        public BundleRepo(ApplicationDbContext db) : base(db) { }

        public void Update(Bundle obj)
        {
            _db.Bundles.Update(obj);
        }
    }
}
