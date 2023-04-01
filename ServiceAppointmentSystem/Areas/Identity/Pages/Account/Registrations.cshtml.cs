using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ServiceAppointmentSystem.Models.Constants;
using ServiceAppointmentSystem.Models.Entities;
using ServiceAppointmentSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ServiceAppointmentSystem.Areas.Identity.Pages.Account;

public class RegistrationsModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly RoleManager<IdentityRole> _roleManager;   
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationsModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        IWebHostEnvironment webHostEnvironment,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
        _unitOfWork = unitOfWork;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Full Name")]
        [MaxLength(255, ErrorMessage = "Names can have a length of only 255 characters.")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "City Address")]
        public string CityAddress { get; set; }

        [Display(Name = "Region Name")]
        public string RegionName { get; set; }

        [Required]
        [Display(Name = "Profile Image")]
        public byte[]? ProfileImage { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10 digit phone number.")]
        public string PhoneNumber { get; set; }

        public string Role { get; set; } = "Employee";

        public string Certification { get; set; }

        public string Resume { get; set; }

        public int Service { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Services { get; set; }
    }


    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

		Input = new InputModel()
		{
			Services = _unitOfWork.Service.GetAll().Select(i => new SelectListItem
			{
				Text = i.Role,
				Value = i.Id.ToString()
			}),
		};

	}

	public async Task<IActionResult> OnPostAsync(IFormFile resume, IFormFile certification, string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid)
        {
            var user = CreateUser();
            var professional = new Professional();
            var password = "Service@123";
            var service = _unitOfWork.Service.Role(Input.Service);

            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;
            user.CityAddress = Input.CityAddress;
            user.RegionName = Input.RegionName;

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);

            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[8];
                var random = new Random();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }

                var finalString = new string(stringChars);

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files.FirstOrDefault();

                    var fileName = $"[Employee - {finalString}] {user.FullName} - Image";

                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @$"images\users\");

                    var extension = Path.GetExtension(file.FileName);

                    using (var dataStream = new MemoryStream())
                    {
                        await file.CopyToAsync(dataStream);
                        user.ProfileImage = dataStream.ToArray();
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    await _userManager.UpdateAsync(user);
                }

                if(resume != null)
                {
                    var fileName = $"[{service} - {finalString}] {user.FullName} - Resume";

                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @$"images\resumes\");
                    
                    var extension = Path.GetExtension(resume.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        resume.CopyTo(fileStreams);
                    }

                    professional.Resume = @$"\images\resumes\" + fileName + extension;
                }

                if (certification != null)
                {
                    var fileName = $"[{service} - {finalString}] {user.FullName} - Certification";

                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @$"images\certifications\");

                    var extension = Path.GetExtension(certification.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        certification.CopyTo(fileStreams);
                    }

                    professional.Certification = @$"\images\certifications\" + fileName + extension;
                }

                professional.UserId = user.Id;
                professional.ServiceId = Input.Service;

                _unitOfWork.Professional.Add(professional);

                await _userManager.AddToRoleAsync(user, Input.Role);

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);

                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                TempData["Success"] = "Registration requested successfully";

                return RedirectToPage("./Registrations");

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }

    private AppUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<AppUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
                $"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<IdentityUser>)_userStore;
    }
}
