using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCutter.Utility
{
	public static class OrderStatus
	{
		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";

		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
		public const string PaymentStatusRejected = "Rejected";

        public static Dictionary<string, string> GetOrderStatusStyles(string activeStatus)
        {
            const string regularStyle = "text-primary";
            const string activeStyle = "active text-white bg-primary";

            Dictionary<string, string> orderStatusStyles = new Dictionary<string, string>
            {
                { "pending", regularStyle },
                { "inprocess", regularStyle },
                { "completed", regularStyle },
                { "approved", regularStyle },
                { "all", regularStyle }
            };

            switch (activeStatus)
            {
                case "pending":
                    orderStatusStyles["pending"] = activeStyle;
                    break;
                case "inprocess":
                    orderStatusStyles["inprocess"] = activeStyle;
                    break;
                case "completed":
                    orderStatusStyles["completed"] = activeStyle;
                    break;
                case "approved":
                    orderStatusStyles["approved"] = activeStyle;
                    break;
                default:
                    orderStatusStyles["all"] = activeStyle;
                    break;
            }

            return orderStatusStyles;
        }
    }
}
