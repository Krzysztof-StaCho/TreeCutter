using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.Models;

namespace TreeCutter.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepo : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser obj);
    }
}
