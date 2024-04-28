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
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM? ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string userId = GetLoggedUserId();
            ShoppingCartVM = GetShoppingCart(userId);

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            string userId = GetLoggedUserId();

            ShoppingCartVM = GetShoppingCart(userId);

            //Filled data from application user
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.User.Get(u => u.Id == userId) ?? throw new Exception("Cannot find user with that id");
            ShoppingCartVM.OrderHeader = GetFilledOrderHeader(ShoppingCartVM.OrderHeader, ShoppingCartVM.OrderHeader.ApplicationUser);

            return View(ShoppingCartVM);

        }

        [HttpPost][ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            if (ShoppingCartVM == null) throw new Exception("Shopping cart is null #1");

            string userId = GetLoggedUserId();

            ShoppingCartVM = GetShoppingCart(userId, ShoppingCartVM.OrderHeader);
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser applicationUser = _unitOfWork.User.Get(u => u.Id == userId) ?? throw new Exception("User doesnt exist in database #1");

            //Checking if user have company role
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = OrderStatus.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            } else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = OrderStatus.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = OrderStatus.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            //Creating Order Details
            foreach (ShoppingCart cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    ShopItemId = cart.ItemId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Item.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            //Send to payment Stripe when user dont have company role
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Configure options
                string domain = "https://localhost:44357/";
                SessionCreateOptions options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"Customer/Cart/Index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };

                foreach (ShoppingCart item in ShoppingCartVM.ShoppingCartList)
                {
                    SessionLineItemOptions sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Item.Price * 100),
                            Currency = "pln",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Item.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                //Sending to payment page
                SessionService service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Append("Location", session.Url);
                return new StatusCodeResult(StatusCodes.Status303SeeOther);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id } );
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

            //Change status if user paid
            if (orderHeader.PaymentStatus != OrderStatus.PaymentStatusDelayedPayment)
            {
                SessionService service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, OrderStatus.StatusApproved, OrderStatus.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();
            }

            List<ShoppingCart> shoppingCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCart);
            _unitOfWork.Save();

            return View(id);
        }

        public IActionResult Plus(int id)
        {
            ShoppingCart? cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id);
            if (cartFromDb == null)
            {
                TempData["message"] = "Cannot find item with that id";
                TempData["messageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            cartFromDb.Count += 1;

            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int id)
        {
            ShoppingCart? cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id, tracked: true);
            if (cartFromDb == null)
            {
                TempData["message"] = "Cannot find item with that id";
                TempData["messageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            if (cartFromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(MiscConstant.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            } else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            ShoppingCart? cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == id, tracked: true);
            if (cartFromDb == null)
            {
                TempData["message"] = "Cannot find item with that id";
                TempData["messageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetInt32(MiscConstant.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }


        #region Helper Method

        private string GetLoggedUserId()
        {
            ClaimsIdentity? claimIdentity = User.Identity as ClaimsIdentity;
            return claimIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }
        private ShoppingCartVM GetShoppingCart(string userId, OrderHeader? header = null)
        {
            IEnumerable<ShoppingCart> shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Item");
            OrderHeader orderHeader = (header == null) ? new OrderHeader() : header;

            orderHeader.OrderTotal = shoppingCartList.Sum(u => u.Price);

			return new ShoppingCartVM
            {
                ShoppingCartList = shoppingCartList,
                OrderHeader = orderHeader
            };
        }
        private OrderHeader GetFilledOrderHeader(OrderHeader header, ApplicationUser user)
        {
            header.Name = user.UserName ?? string.Empty;
            header.PhoneNumber = user.PhoneNumber ?? string.Empty;
            header.StreetAddress = user.StreetAddress ?? string.Empty;
            header.City = user.City ?? string.Empty;
            header.State = user.State ?? string.Empty;
            header.PostalCode = user.PostalCode ?? string.Empty;

            return header;
        }

        #endregion
    }
}
