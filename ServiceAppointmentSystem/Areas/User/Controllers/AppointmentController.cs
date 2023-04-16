using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceAppointmentSystem.Models.Constants;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Models.ViewModels;
using ServiceAppointmentSystem.Repositories.Interfaces;
using System.Numerics;
using System.Security.Claims;

namespace ServiceAppointmentSystem.Areas.User.Controllers;

[Area("User")]
[Authorize(Roles = Constants.User)]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var result = _unitOfWork.Appointment.GetAll().Where(x => x.ActionStatus == Constants.Completed).ToList()
                     .Select(x => new AppointmentViewModel()
                     {
                         AppointmentId = x.Id,
                         ServiceId = x.ServiceId,
                         Service = _unitOfWork.Service.Get(x.ServiceId).Name,
                         BookedDate = x.BookedDate.ToString("dddd, dd MMMM yyyy"),
                         FinalizedDate = x.AppointedDate?.ToString("dddd, dd MMMM yyyy"),
                         ProfessionalId = x.ProfessionalId ,
                         ProfessionalName = _unitOfWork.AppUser.GetById(_unitOfWork.Professional.Get(x.ProfessionalId).UserId).FullName,
                         Professionalism = _unitOfWork.Service.Get(_unitOfWork.Professional.Get(x.ProfessionalId).ServiceId).Role,
                         Request = x.Request,
                         ActionStatus = x.ActionStatus,
                         ProfessionalRemarks = x.ProfessionalRemarks,
                     }).ToList();

        return View(result);
    }

    public IActionResult Booked()
    {
        var result = _unitOfWork.Appointment.GetAll().Where(x => x.ActionStatus == Constants.Booked).ToList()
                     .Select(x => new AppointmentViewModel()
                     {
                         AppointmentId = x.Id,
                         ServiceId = x.ServiceId,
                         Service = _unitOfWork.Service.Get(x.ServiceId).Name,
                         BookedDate = x.BookedDate.ToString("dddd, dd MMMM yyyy"),
                         FinalizedDate = x.AppointedDate?.ToString("dddd, dd MMMM yyyy"),
                         ProfessionalId = x.ProfessionalId,
                         ProfessionalName = _unitOfWork.AppUser.GetById(_unitOfWork.Professional.Get(x.ProfessionalId).UserId).FullName,
                         Professionalism = _unitOfWork.Service.Get(_unitOfWork.Professional.Get(x.ProfessionalId).ServiceId).Role,
                         Request = x.Request,
                         ActionStatus = x.ActionStatus,
                         ProfessionalRemarks = x.ProfessionalRemarks,
                     }).ToList();

        return View(result);
    }

    public IActionResult Start()
    {
        var appointment = new AppointmentViewModel();

        var services = new SelectList(_unitOfWork.Service.GetAll(), "Id", "Name");

        ViewData["ServiceId"] = services;

        return View(appointment);
    }

    [HttpPost]
    public IActionResult Start(AppointmentViewModel appointment)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var result = new Appointment()
        {
            UserId = claim.Value,
            Request = appointment.Request,
            BookedDate = DateTime.Now,
            ServiceId = appointment.ServiceId,
            ActionStatus = Constants.Booked,
        };

        _unitOfWork.Appointment.Add(result);

        TempData["Success"] = "Appointment Booked Successfully";

        return RedirectToAction("Index");
    }
}
