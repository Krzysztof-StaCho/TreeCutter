using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models;
using TreeCutter.Models.ViewModels;
using TreeCutter.Utility;

namespace TreeCutterWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderHeader? orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser");
			if (orderHeader == null) return NotFound();


            OrderVM viewModel = new OrderVM
			{
				OrderHeader = orderHeader,
				OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "ShopItem")
			};

			return View(viewModel);
		}

		[HttpPost]
		public IActionResult Details(OrderVM viewModel)
		{
			viewModel.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == viewModel.OrderHeader.Id, includeProperties: "ApplicationUser") ?? throw new Exception("Cannot find order header #1");
			viewModel.OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == viewModel.OrderHeader.Id, includeProperties: "ShopItem");

			string domain = "https://localhost:44382/";
			SessionCreateOptions options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"Customer/Order/PaymentConfirmation?orderHeaderId={viewModel.OrderHeader.Id}",
				CancelUrl = domain + $"Customer/Order/Details?orderId={viewModel.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment"
			};

			foreach (OrderDetail item in viewModel.OrderDetails)
			{
				SessionLineItemOptions sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),
						Currency = "pln",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.ShopItem.Name
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
            }

			SessionService service = new SessionService();
			Session session = service.Create(options);

			_unitOfWork.OrderHeader.UpdateStripePaymentId(viewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Append("Location", session.Url);
			return new StatusCodeResult(StatusCodes.Status303SeeOther);
        }

		public IActionResult PaymentConfirmation(int orderHeaderId)
		{
			OrderHeader? orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
			if (orderHeader == null) return NotFound();

			if (orderHeader.PaymentStatus == OrderStatus.PaymentStatusDelayedPayment)
			{
				SessionService service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, OrderStatus.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}

			return View(orderHeaderId);
		}

        #region Service API

        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeaders;
			
			switch (status)
			{
				case "pending":
					status = OrderStatus.StatusPending;
					break;
                case "inprocess":
                    status = OrderStatus.StatusInProcess;
                    break;
                case "completed":
                    status = OrderStatus.StatusShipped;
                    break;
                case "approved":
                    status = OrderStatus.StatusApproved;
                    break;
				default:
					status = "";
					break;
            }

			string userId = GetLoggedUserId();
			orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId && u.OrderStatus.Contains(status), includeProperties: "ApplicationUser");

			return Json(new { data = orderHeaders });
		}

        #endregion
        #region Helper Method

        private string GetLoggedUserId()
        {
            ClaimsIdentity? claimIdentity = User.Identity as ClaimsIdentity;
            return claimIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        #endregion
    }
}
