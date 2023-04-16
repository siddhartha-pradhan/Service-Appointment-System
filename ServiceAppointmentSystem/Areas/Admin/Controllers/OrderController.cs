using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Models.ViewModels;
using ServiceAppointmentSystem.Repositories.Interfaces;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;
using ServiceAppointmentSystem.Models.Constants;

namespace ServiceAppointmentSystem.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = Constants.Admin)]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		#region Razor Pages
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Detail(int ID)
		{
			OrderViewModel orderViewModel = new OrderViewModel()
			{
				Order = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == ID, includeProperties: "AppUser"),
				OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == ID, includeProperties: "Item")
			};
			return View(orderViewModel);
		}
		#endregion

		#region API Calls
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeadersList;

			if (User.IsInRole(Constants.Admin))
			{
				orderHeadersList = _unitOfWork.Order.GetAll(null, includeProperties: "AppUser");
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				orderHeadersList = _unitOfWork.Order.GetAll(u => u.UserId == claim.Value, includeProperties: "AppUser");
			}

			switch (status)
			{
				case "pending":
					orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == Constants.StatusPending);
					break;
				case "inprocess":
					orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == Constants.StatusProcessing);
					break;
				case "completed":
					orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == Constants.StatusShipped);
					break;
				default:
					break;
			}

			return Json(new { data = orderHeadersList });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Constants.Admin)]
		public IActionResult UpdateOrderDetails(OrderViewModel orderViewModel)
		{
			var orderHeader = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderViewModel.Order.Id);
			orderHeader.Name = orderViewModel.Order.Name;
			orderHeader.PhoneNumber = orderViewModel.Order.PhoneNumber;
			orderHeader.City = orderViewModel.Order.City;
			orderHeader.Region = orderViewModel.Order.Region;

			if (orderViewModel.Order.Carrier != null)
			{
				orderHeader.Carrier = orderViewModel.Order.Carrier;
			}
			if (orderViewModel.Order.TrackingNumber != null)
			{
				orderHeader.TrackingNumber = orderViewModel.Order.TrackingNumber;
			}

			_unitOfWork.Order.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Order Details updated successfully";
			return RedirectToAction("Detail", "Order", new { ID = orderHeader.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Constants.Admin)]
		public IActionResult StartProcessing(OrderViewModel orderViewModel)
		{
			_unitOfWork.Order.UpdateStatus(orderViewModel.Order.Id, Constants.StatusPending);
			_unitOfWork.Save();
			TempData["Success"] = "Order Status updated successfully";
			return RedirectToAction("Detail", "Order", new { ID = orderViewModel.Order.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Constants.Admin)]
		public IActionResult ShipOrder(OrderViewModel orderViewModel)
		{
			var orderHeader = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderViewModel.Order.Id);
			orderHeader.TrackingNumber = orderViewModel.Order.TrackingNumber;
			orderHeader.Carrier = orderViewModel.Order.Carrier;
			orderHeader.OrderStatus = Constants.StatusShipped;
			orderHeader.ShippedDate = DateTime.Now;

			if (orderHeader.PaymentStatus == Constants.PaymentDelayed)
			{
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}

			_unitOfWork.Order.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Order shipped successfully";
			return RedirectToAction("Detail", "Order", new { ID = orderViewModel.Order.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Constants.Admin)]
		public IActionResult CancelOrder(OrderViewModel orderViewModel)
		{
			var orderHeader = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderViewModel.Order.Id);

			if (orderHeader.PaymentStatus == Constants.PaymentApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.Order.UpdateStatus(orderHeader.Id, Constants.StatusCancelled, Constants.PaymentRefunded);
			}
			else
			{
				_unitOfWork.Order.UpdateStatus(orderHeader.Id, Constants.StatusCancelled, Constants.PaymentCancelled);
			}

			_unitOfWork.Save();
			TempData["Success"] = "Order cancelled successfully";
			return RedirectToAction("Detail", "Order", new { ID = orderViewModel.Order.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = Constants.Admin)]
		public IActionResult DelayedPayment(OrderViewModel orderViewModel)
		{
			orderViewModel.Order = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderViewModel.Order.Id, "AppUser");

			orderViewModel.OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderViewModel.Order.Id, null, "Item");

			var domain = "https://localhost:44389/";

			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>()
					{
						"card"
					},
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderID={orderViewModel.Order.Id}",
				CancelUrl = domain + $"Admin/Order/Detail?ID={orderViewModel.Order.Id}",
			};

			foreach (var item in orderViewModel.OrderDetails)
			{
				var sessionLine = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Item.Name
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionLine);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			_unitOfWork.Order.UpdatePaymentStatus(orderViewModel.Order.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

		public IActionResult PaymentConfirmation(int orderHeaderID)
		{
			var orderHeader = _unitOfWork.Order.GetFirstOrDefault(i => i.Id == orderHeaderID);

			if (orderHeader.PaymentStatus == Constants.PaymentDelayed)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.Order.UpdatePaymentStatus(orderHeader.Id, session.Id, session.PaymentIntentId);
					_unitOfWork.Order.UpdateStatus(orderHeader.Id, orderHeader.OrderStatus, Constants.PaymentApproved);
					_unitOfWork.Save();
				}
			}

			return View(orderHeaderID);
		}

		#endregion
	}
}
