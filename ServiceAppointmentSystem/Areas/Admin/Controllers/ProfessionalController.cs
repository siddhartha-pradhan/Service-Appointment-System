using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ServiceAppointmentSystem.Models.ViewModels;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfessionalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailSender _emailSender;

		public ProfessionalController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
		}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Professionals()
        {
            var users = _unitOfWork.AppUser.GetAll().Where(x => !x.EmailConfirmed).ToList();
            var services = _unitOfWork.Service.GetAll();
            var professionals = _unitOfWork.Professional.GetAll();

            var result = (from user in users
                          join professional in professionals
                          on user.Id equals professional.UserId
                          join service in services
                          on professional.ServiceId equals service.Id
                          select new ApprovalViewModel
                          {
                              UserId = user.Id,
                              ProfessionalId = professional.Id,
                              FullName = user.FullName,
                              PhoneNumber = user.PhoneNumber,
                              EmailAddress = user.Email,
                              ProfileImage = user.ProfileImage,
                              Specialist = service.Role,
                              Certification = professional.Certification,
                              Resume = professional.Resume
                          }).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var professionals = _unitOfWork.Professional.GetAll().Where(x => x.IsApproved);

            return Json(new { data = professionals });
        }

        [HttpPost, ActionName("Professionals")]
        public IActionResult Approve(string id, int professionalId)
        {
            var user = _unitOfWork.AppUser.GetFirstOrDefault(x => x.Id == id);  
            var professional = _unitOfWork.Professional.GetFirstOrDefault(x => x.Id == professionalId);
            var service = _unitOfWork.Service.GetFirstOrDefault(x => x.Id == professional.ServiceId);

            if(ModelState.IsValid)
            {
                _unitOfWork.Professional.Approve(professional);

				TempData["Success"] = "Professional Approved Successfully";

				_emailSender.SendEmailAsync(user.Email, "Successful Registration",
						$"Hi there, You have been registered to our system as a {service.Role}. The password is Service@123");

				return RedirectToAction("Index");
			}

			return View(user);
		}
	}
}
