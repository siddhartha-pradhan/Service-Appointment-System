using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAppointmentSystem.Models.Constants;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;

namespace ServiceAppointmentSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.Admin)]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Razor Pages
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? Id)
        {
            var service = new Service();

            if (Id == null)
            {
                return View(service);
            }

            service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == Id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        public IActionResult Delete(int Id)
        {
            var service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == Id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }
        #endregion

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var services = _unitOfWork.Service.GetAll();

            return Json(new { data = services });
        }

        [HttpPost, ActionName("Upsert")]
        public IActionResult UpsertPost(Service service)
        {
            if (ModelState.IsValid)
            {
                if (service.Id == 0)
                {
                    _unitOfWork.Service.Add(service);
                    TempData["Success"] = "Service added successfully";
                }
                else
                {
                    _unitOfWork.Service.Update(service);
                    TempData["Info"] = "Service altered successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            return View(service);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var service = _unitOfWork.Service.GetFirstOrDefault(u => u.Id == id);

            if (service != null)
            {
                _unitOfWork.Service.Delete(service);
                _unitOfWork.Save();

                TempData["Delete"] = "Service deleted successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        #endregion
    }
}
