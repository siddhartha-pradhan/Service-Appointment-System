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

namespace ServiceAppointmentSystem.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly RoleManager<IdentityRole> _roleManager;   
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public RegisterModel(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender,
        IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public class InputModel
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Names can have a length of only 255 characters.")]
        [Display(Name = "Full Name")]
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

        public string Role { get; set; } = Constants.User;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid)
        {
            var user = CreateUser();

            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;
            user.CityAddress = Input.CityAddress;
            user.RegionName = Input.RegionName;

            await _userManager.AddToRoleAsync(user, Input.Role);

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);

            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, Input.Password);

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var userId = await _userManager.GetUserIdAsync(user);

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files.FirstOrDefault();

                    var fileName = $"[User - {finalString}] {user.FullName} - Image";

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

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(Input.Email, "Email Confirmation",
                    $"Dear {user.FullName}, <br> <br>You have been registered to our system as an user. To continue your activation process, please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. <br>Regards.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
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
