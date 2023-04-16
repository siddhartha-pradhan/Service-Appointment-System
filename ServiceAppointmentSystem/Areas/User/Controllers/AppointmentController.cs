using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    private readonly IEmailSender _emailSender;

    public AppointmentController(IUnitOfWork unitOfWork, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var user = _unitOfWork.AppUser.GetById(claim.Value);

        var result = _unitOfWork.Appointment.GetAll().Where(x => x.ActionStatus == Constants.Completed && x.UserId == user.Id).ToList()
                     .Select(x => new AppointmentViewModel()
                     {
                         AppointmentId = x.Id,
                         ServiceId = x.ServiceId,
                         Service = _unitOfWork.Service.Get(x.ServiceId).Name,
                         BookedDate = x.BookedDate.ToString("dddd, dd MMMM yyyy"),
                         AppointedDate = x.AppointmentDate.ToString("dddd, dd MMMM yyyy"),
                         FinalizedDate = x.FinalizedDate?.ToString("dddd, dd MMMM yyyy"),
                         ProfessionalId = x.ProfessionalId ,
                         ProfessionalName = _unitOfWork.AppUser.GetById(_unitOfWork.Professional.GetById(x.ProfessionalId).UserId).FullName,
                         Professionalism = _unitOfWork.Service.Get(_unitOfWork.Professional.GetById(x.ProfessionalId).ServiceId).Role,
                         Request = x.Request,
                         ActionStatus = x.ActionStatus,
                         ProfessionalRemarks = x.ProfessionalRemarks,
                     }).ToList();

        return View(result);
    }

    private string AppUser(Guid? Id)
    {
        var professional = _unitOfWork.Professional.GetById(Id);

        if (professional == null)
        {
            return "No professionals assigned yet.";
        }

        var result = _unitOfWork.AppUser.GetById(professional.UserId);

        return result.FullName;
    }

    private string Service(Guid? Id)
    {
        var professional = _unitOfWork.Professional.GetById(Id);

        if (professional == null)
        {
            return "No professionals assigned yet.";
        }

        var result = _unitOfWork.Service.Get(professional.ServiceId);
        
        return result.Name;
    }

    public IActionResult Booked()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;

        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        var user = _unitOfWork.AppUser.GetById(claim.Value);

        var result = _unitOfWork.Appointment.GetAll().Where(x => x.ActionStatus == Constants.Booked && x.UserId == user.Id).ToList()
                     .Select(x => new AppointmentViewModel()
                     {
                         AppointmentId = x.Id,
                         ServiceId = x.ServiceId,
                         Service = _unitOfWork.Service.Get(x.ServiceId).Name,
                         BookedDate = x.BookedDate.ToString("dddd, dd MMMM yyyy"),
                         AppointedDate = x.AppointmentDate.ToString("dddd, dd MMMM yyyy"),
                         FinalizedDate = x.FinalizedDate?.ToString("dddd, dd MMMM yyyy"),
						 ProfessionalId = x.ProfessionalId,
                         ProfessionalName = AppUser(x.ProfessionalId),
                         Professionalism = Service(x.ProfessionalId),
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

        var user = _unitOfWork.AppUser.GetById(claim.Value);

        var service = _unitOfWork.Service.Get(appointment.ServiceId);

        var appointedDate = DateTime.ParseExact(appointment.AppointedDate, "yyyy-MM-dd'T'HH:mm", System.Globalization.CultureInfo.InvariantCulture);

        var result = new Appointment()
        {
            UserId = claim.Value,
            Request = appointment.Request,
            BookedDate = DateTime.Now,
            ServiceId = appointment.ServiceId,
            ActionStatus = Constants.Booked,
            AppointmentDate = appointedDate,
        };

        _unitOfWork.Appointment.Add(result);

        _unitOfWork.Save();

        _emailSender.SendEmailAsync(user.Email, "Appointment Booking", 
            $"Dear {user.FullName}, <br><br>A new appointment for your request on {service.Name} has been booked. <br>Please wait a while till the admin assigns a technician for the process. <br><br>Regards.");

        TempData["Success"] = "Appointment Booked Successfully";

        return RedirectToAction("Index");
    }
}
