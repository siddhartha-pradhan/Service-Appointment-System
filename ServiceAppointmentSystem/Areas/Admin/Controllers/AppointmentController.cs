using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceAppointmentSystem.Models.Constants;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Models.ViewModels;
using ServiceAppointmentSystem.Repositories.Interfaces;
using Stripe;

namespace ServiceAppointmentSystem.Areas.Admin.Controllers;

[Area("Admin")]
public class AppointmentController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;

    public AppointmentController(IUnitOfWork unitOfWork, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
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

    public AppUser AppUser(string Id)
    {
        return _unitOfWork.AppUser.GetById(Id);
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

    public IActionResult Booking()
    {
        var result = _unitOfWork.Appointment.GetAll().Where(x => x.ActionStatus == Constants.Booked).ToList()
                 .Select(x => new AppointmentViewModel()
                 {
                     AppointmentId = x.Id,
                     ServiceId = x.ServiceId,
                     UserId = x.UserId,
                     UserName = AppUser(x.UserId).FullName,
                     UserProfile = AppUser(x.UserId).ProfileImage,
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

    public IActionResult Details(int id)
    {
        var appointment = _unitOfWork.Appointment.Get(id);

        var service = _unitOfWork.Service.GetFirstOrDefault(x => x.Id == appointment.ServiceId);

        var professionals = _unitOfWork.Professional.GetAll().Where(x => x.ServiceId == service.Id).ToList();

        var users = (from user in _unitOfWork.AppUser.GetAll()
                     join professional in professionals
                     on user.Id equals professional.UserId
                     select new
                     {
                         UserId = user.Id,
                         ProfessionalId = professional.Id,
                         ProfessionalName = user.FullName
                     }).ToList();

        var appUsers = users.Select(x => new SelectListItem()
        {
            Text = x.ProfessionalName,
            Value = x.ProfessionalId.ToString()
        });

        var result = new AppointmentDetailViewModel()
        {
            Request = appointment.Request,
            AppointedDate = appointment.AppointmentDate.ToString("dddd, dd MMMM yyyy"),
            BookedDate = appointment.BookedDate.ToString("dddd, dd MMMM yyyy"),
            AppointmentId = appointment.Id,
            Professionals = appUsers.ToList(),
            Service = service.Name,
            UserName = AppUser(appointment.UserId).FullName,
            UserProfile = AppUser(appointment.UserId).ProfileImage,
            ServiceId = appointment.ServiceId,
            UserId = appointment.UserId,
            FinalizedDate = "",
        };

        return View(result);
    }

    [HttpPost]
    public IActionResult Details(AppointmentDetailViewModel detail)
    {
        var professional = _unitOfWork.Professional.GetAll().Where(x => x.Id == detail.ProfessionalId).FirstOrDefault();

        var user = _unitOfWork.AppUser.GetById(professional.UserId);

        var appUser = _unitOfWork.AppUser.GetById(detail.UserId);

        var appointment = _unitOfWork.Appointment.Get(detail.AppointmentId);

        if (appointment != null)
        {
            appointment.ActionStatus = Constants.Completed;
            appointment.AdminRemarks = "Completed";
            appointment.FinalizedDate = DateTime.Now;
            appointment.PaymentStatus = Constants.Completed;
            appointment.ProfessionalId = detail.ProfessionalId;
            appointment.ProfessionalRemarks = "Taking things into account";
        }

        _unitOfWork.Save();

        _emailSender.SendEmailAsync(appUser.Email, "Appointment Booking Approved", $"Dear {appUser.FullName},<br><br>We have assigned Mr. {user.FullName} as your technician. Please contact with him with the following details {user.PhoneNumber}.<br><br>");

        _emailSender.SendEmailAsync(user.Email, "Appointment Booking", @$"Dear {user.FullName},<br><br>You have been assigned to Mr. {appUser.FullName}'s place as the respective technician. They have appealed " + appointment.Request + $" as their call for appointment request.<br><br>Please contact with the client with the following details {user.PhoneNumber}.<br><br>");

        TempData["Success"] = "Appointment Successfully Completed";

        return RedirectToAction("Booking");
    }



    public IActionResult History() 
    {
        var result = _unitOfWork.Appointment.GetAll().Where(x => x.ActionStatus == Constants.Completed).ToList()
                 .Select(x => new AppointmentViewModel()
                 {
                     AppointmentId = x.Id,
                     ServiceId = x.ServiceId,
                     UserId = x.UserId,
                     UserName = AppUser(x.UserId).FullName,
                     UserProfile = AppUser(x.UserId).ProfileImage,
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
}
