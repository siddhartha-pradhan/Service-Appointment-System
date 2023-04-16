using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ServiceAppointmentSystem.Models.Constants;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Models.ViewModels;
using ServiceAppointmentSystem.Repositories.Interfaces;
using System.Security.Claims;

namespace ServiceAppointmentSystem.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = Constants.User)]
public class CartController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	private readonly IWebHostEnvironment _webHostEnvironment;

	private readonly IEmailSender _emailSender;

	public CartController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
	{
		_unitOfWork = unitOfWork;
		_webHostEnvironment = webHostEnvironment;
		_emailSender = emailSender;
	}

	#region Razor Page
	public IActionResult Index()
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;

		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		ShoppingCartViewModel cartViewModel = new ShoppingCartViewModel()
		{
			CartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, null, "Item"),
			Order = new OrderHeader()
		};

		foreach (var item in cartViewModel.CartList)
		{
			item.Price = GetPriceByQuantity(item.Count, item.Item.Price, item.Item.Price20, item.Item.Price50);
			cartViewModel.Order.OrderTotal += (item.Price * item.Count);
		}

		return View(cartViewModel);
	}

	public IActionResult Summary()
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;

		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		ShoppingCartViewModel cartViewModel = new ShoppingCartViewModel()
		{
			CartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, null, "Item"),
			Order = new OrderHeader()
		};

		cartViewModel.Order.AppUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);
		cartViewModel.Order.Name = cartViewModel.Order.AppUser.FullName;
		cartViewModel.Order.PhoneNumber = cartViewModel.Order.AppUser.PhoneNumber;
		cartViewModel.Order.City = cartViewModel.Order.AppUser.CityAddress;
		cartViewModel.Order.Region = cartViewModel.Order.AppUser.RegionName;

		foreach (var item in cartViewModel.CartList)
		{
			item.Price = GetPriceByQuantity(item.Count, item.Item.Price, item.Item.Price20, item.Item.Price50);
			cartViewModel.Order.OrderTotal += (item.Price * item.Count);
		}

		return View(cartViewModel);
	}

	#endregion

	#region API Calls
	public IActionResult Add(int cartID)
	{
		var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartID);
		_unitOfWork.ShoppingCart.IncrementCount(cart, 1);
		_unitOfWork.Save();
		return RedirectToAction(nameof(Index));
	}

	public IActionResult Substract(int cartID)
	{
		var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartID);

		if (cart.Count <= 1)
		{
			_unitOfWork.ShoppingCart.Remove(cart);
			_unitOfWork.Save();
		}
		else
		{
			_unitOfWork.ShoppingCart.DecrementCount(cart, 1);
			_unitOfWork.Save();
		}

		return RedirectToAction(nameof(Index));
	}

	public IActionResult Remove(int cartID)
	{
		var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartID);
		_unitOfWork.ShoppingCart.Remove(cart);
		_unitOfWork.Save();
		return RedirectToAction(nameof(Index));
	}

	[HttpPost, ActionName("Summary")]
	[ValidateAntiForgeryToken]
	public IActionResult SummaryPost(ShoppingCartViewModel cartViewModel)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		cartViewModel.CartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, null, "Item");
		cartViewModel.Order.OrderedDate = DateTime.Now;
		cartViewModel.Order.UserId = claim.Value;

		foreach (var item in cartViewModel.CartList)
		{
			item.Price = GetPriceByQuantity(item.Count, item.Item.Price, item.Item.Price50, item.Item.Price50);
			cartViewModel.Order.OrderTotal += (item.Price * item.Count);
		}

		var applicationUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);

		cartViewModel.Order.PaymentStatus = Constants.PaymentPending;
		cartViewModel.Order.OrderStatus = Constants.StatusPending;

		_unitOfWork.Order.Add(cartViewModel.Order);
		_unitOfWork.Save();

		foreach (var item in cartViewModel.CartList)
		{
			OrderDetail orderDetail = new OrderDetail()
			{
				OrderId = cartViewModel.Order.Id,
				ItemId = item.ItemId,
				Price = item.Price,
				Count = item.Count
			};

			_unitOfWork.OrderDetail.Add(orderDetail);
			_unitOfWork.Save();
		}

		return RedirectToAction("OrderConfirmation", "Cart", new { id = cartViewModel.Order.Id });
		
	}

	public IActionResult OrderConfirmation(int ID)
	{
		var orderHeader = _unitOfWork.Order.GetFirstOrDefault(i => i.Id == ID);

		var claimsIdentity = (ClaimsIdentity)User.Identity;

		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		var applicationUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);

		orderHeader.AppUser.Email = applicationUser.Email;

		if (orderHeader.PaymentStatus != Constants.PaymentDelayed)
		{
			//var service = new SessionService();

			//var session = service.Get(orderHeader.SessionId);

			//if (session.PaymentStatus.ToLower() == "paid")
			//{
			//	_unitOfWork.Order.UpdatePaymentStatus(orderHeader.ID, session.Id, session.PaymentIntentId);
			//	_unitOfWork.Order.UpdateStatus(orderHeader.ID, StaticDetails.Status_Approved, StaticDetails.Payment_Approved);
			//	_unitOfWork.Save();
			//}
		}

		_emailSender.SendEmailAsync(orderHeader.AppUser.Email, "Order Confirmation", $"Dear {applicationUser.FullName}, <br><br>A new order has been placed.");

		var cartList = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == orderHeader.UserId, null, null).ToList();

		_unitOfWork.ShoppingCart.RemoveRange(cartList);
		
		_unitOfWork.Save();

		return View(ID);
	}

	#endregion

	public double GetPriceByQuantity(int quantity, double price, double price50, double price100)
	{
		if (quantity <= 50)
		{
			return price;
		}
		else if (quantity <= 100)
		{
			return price50;
		}
		else
		{
			return price100;
		}
	}
}
