using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeCutter.DataAccess.Repository.IRepository;
using TreeCutter.Models.ViewModels;
using TreeCutter.Models;
using TreeCutter.Utility;
using System.Security.Claims;
using Stripe;

namespace TreeCutterPanel.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{ApplicationRoles.Role_Admin},{ApplicationRoles.Role_Employee}")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
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
        public IActionResult UpdateOrderDetail(OrderVM input)
        {
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == input.OrderHeader.Id) ?? throw new Exception("Cannot find order header in db! #1");

            orderHeaderFromDb.Name = input.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = input.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = input.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = input.OrderHeader.City;
            orderHeaderFromDb.State = input.OrderHeader.State;
            orderHeaderFromDb.PostalCode = input.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(input.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = input.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(input.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = input.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["message"] = "Order details updated successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        public IActionResult StartProcessing(OrderVM input)
        {
            _unitOfWork.OrderHeader.UpdateStatus(input.OrderHeader.Id, OrderStatus.StatusInProcess);
            _unitOfWork.Save();

            TempData["message"] = "Order details updated successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction(nameof(Details), new { orderId = input.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult ShipOrder(OrderVM input)
        {
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == input.OrderHeader.Id) ?? throw new Exception("Cannot find order header in db! #2");

            orderHeaderFromDb.TrackingNumber = input.OrderHeader.TrackingNumber;
            orderHeaderFromDb.Carrier = input.OrderHeader.Carrier;
            orderHeaderFromDb.OrderStatus = OrderStatus.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;

            if (orderHeaderFromDb.PaymentStatus == OrderStatus.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["message"] = "Order shipped successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction(nameof(Details), new { orderId = input.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult CancelOrder(OrderVM input)
        {
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == input.OrderHeader.Id) ?? throw new Exception("Cannot find order header in db! #3");

            if (orderHeaderFromDb.PaymentStatus == OrderStatus.PaymentStatusApproved)
            {
                RefundCreateOptions options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId
                };

                RefundService service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, OrderStatus.StatusCancelled, OrderStatus.StatusRefunded);
            } else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, OrderStatus.StatusCancelled, OrderStatus.StatusCancelled);
            }
            _unitOfWork.Save();

            TempData["message"] = "Order cancelled successfully!";
            TempData["messageType"] = "success";

            return RedirectToAction(nameof(Details), new { orderId = input.OrderHeader.Id });
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

            orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus.Contains(status), includeProperties: "ApplicationUser");

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
