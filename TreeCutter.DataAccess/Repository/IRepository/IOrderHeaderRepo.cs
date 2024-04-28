using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCutter.Models;

namespace TreeCutter.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepo : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStripePaymentId(int id, string sessionId, string paymentId);
	}
}
