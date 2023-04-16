using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAppointmentSystem.Models;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

namespace ServiceAppointmentSystem.Areas.User.Controllers;

[Area("User")]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;

	private readonly IUnitOfWork _unitOfWork;

	public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
	{
		_logger = logger;
		_unitOfWork = unitOfWork;
	}

	#region Razor Pages
	public IActionResult Home()
	{
		return View();
	}

	public IActionResult Items()
	{
		var items = _unitOfWork.Item.GetAll(includeProperties: "Service");

		return View(items);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	public IActionResult Details(int itemID)
	{
		ShoppingCart cart = new ShoppingCart()
		{
			Count = 1,
			ItemId = itemID,
			Item = _unitOfWork.Item.GetFirstOrDefault(x => x.Id == itemID, includeProperties: "Service")
		};

		return View(cart);

	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
	#endregion

	#region API Calls
	[HttpPost]
	[Authorize]
	[ValidateAntiForgeryToken]
	public IActionResult Details(ShoppingCart shoppingCart)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;

		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		shoppingCart.UserId = claim.Value;

		ShoppingCart cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u =>
			u.UserId == claim.Value && u.ItemId == shoppingCart.ItemId);

		if (cart == null)
		{
			_unitOfWork.ShoppingCart.Add(shoppingCart);
		}
		else
		{
			_unitOfWork.ShoppingCart.IncrementCount(cart, shoppingCart.Count);
		}

		_unitOfWork.Save();

		TempData["Success"] = "Successfully Added To Cart";

		return RedirectToAction(nameof(Items));
	}
	#endregion
}