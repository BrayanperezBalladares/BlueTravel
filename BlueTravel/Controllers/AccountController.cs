using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BlueTravel.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace BlueTravel.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new IdentityUser 
                    { 
                        UserName = model.Email, 
                        Email = model.Email,
                        EmailConfirmed = true // ✅ Para que no requiera confirmación
                    };
                    
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Usuario creado exitosamente: {Email}", model.Email);

                        // 🔑 Asignar rol Cliente por defecto
                        await _userManager.AddToRoleAsync(user, "Cliente");

                        // Iniciar sesión automáticamente
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        
                        TempData["SuccessMessage"] = "¡Cuenta creada exitosamente! Bienvenido a BlueTravel.";
                        
                        // Redirigir al returnUrl o Home
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        
                        return RedirectToAction("Index", "Home");
                    }

                    // Agregar errores de Identity al ModelState
                    foreach (var error in result.Errors)
                    {
                        _logger.LogWarning("Error al crear usuario: {Code} - {Description}", error.Code, error.Description);
                        
                        // Traducir errores comunes al español
                        string errorMessage = error.Code switch
                        {
                            "DuplicateUserName" => "Este correo electrónico ya está registrado.",
                            "InvalidEmail" => "El formato del correo electrónico no es válido.",
                            "PasswordTooShort" => "La contraseña debe tener al menos 6 caracteres.",
                            "PasswordRequiresNonAlphanumeric" => "La contraseña debe contener al menos un carácter especial (!@#$%^&*).",
                            "PasswordRequiresDigit" => "La contraseña debe contener al menos un número.",
                            "PasswordRequiresUpper" => "La contraseña debe contener al menos una letra mayúscula.",
                            "PasswordRequiresLower" => "La contraseña debe contener al menos una letra minúscula.",
                            _ => error.Description
                        };
                        
                        ModelState.AddModelError(string.Empty, errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al registrar usuario");
                    ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Por favor intente nuevamente.");
                }
            }
            
            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        model.Email, 
                        model.Password, 
                        model.RememberMe, 
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Usuario inició sesión: {Email}", model.Email);
                        TempData["SuccessMessage"] = "¡Bienvenido de nuevo!";
                        
                        // Redirigir al returnUrl o Home
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        
                        return RedirectToAction("Index", "Home");
                    }

                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Cuenta bloqueada: {Email}", model.Email);
                        ModelState.AddModelError(string.Empty, "Tu cuenta está bloqueada. Intenta más tarde.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al iniciar sesión");
                    ModelState.AddModelError(string.Empty, "Ocurrió un error. Por favor intente nuevamente.");
                }
            }
            
            return View(model);
        }

        // 🆕 POST: Account/ExternalLogin (Google OAuth)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // 🔧 La URL de callback será /signin-google (manejado automáticamente por Google provider)
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl }, Request.Scheme);
            
            _logger.LogInformation("🔍 ExternalLogin iniciado:");
            _logger.LogInformation("   Provider: {Provider}", provider);
            _logger.LogInformation("   RedirectUrl: {RedirectUrl}", redirectUrl);
            _logger.LogInformation("   ReturnUrl: {ReturnUrl}", returnUrl);
            
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // 🆕 GET: Account/ExternalLoginCallback
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                _logger.LogError("Error del proveedor externo: {Error}", remoteError);
                TempData["ErrorMessage"] = $"Error de Google: {remoteError}";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            // Obtener información del login externo
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogWarning("No se pudo obtener información del login externo");
                TempData["ErrorMessage"] = "No se pudo conectar con Google. Intenta nuevamente.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            // Intentar login con el proveedor externo
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, 
                info.ProviderKey, 
                isPersistent: false, 
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("✅ Usuario inició sesión con {Provider}: {Email}", 
                    info.LoginProvider, 
                    info.Principal.FindFirstValue(ClaimTypes.Email));
                TempData["SuccessMessage"] = "¡Bienvenido de nuevo!";
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("⚠️ Cuenta bloqueada");
                TempData["ErrorMessage"] = "Tu cuenta está bloqueada temporalmente.";
                return RedirectToAction(nameof(Login));
            }
            else
            {
                // Usuario no existe, crear nueva cuenta
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("❌ Google no proporcionó email");
                    TempData["ErrorMessage"] = "No se pudo obtener tu email de Google. Verifica los permisos.";
                    return RedirectToAction(nameof(Register), new { returnUrl });
                }

                // Verificar si el email ya existe
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // Vincular el login externo al usuario existente
                    var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                        _logger.LogInformation("✅ Login de Google vinculado a cuenta existente: {Email}", email);
                        TempData["SuccessMessage"] = "¡Cuenta de Google vinculada exitosamente!";
                        return LocalRedirect(returnUrl);
                    }
                }

                // Crear nuevo usuario
                var user = new IdentityUser 
                { 
                    UserName = email, 
                    Email = email,
                    EmailConfirmed = true // Google ya verificó el email
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    // Asignar rol Cliente
                    await _userManager.AddToRoleAsync(user, "Cliente");
                    _logger.LogInformation("✅ Rol Cliente asignado a {Email}", email);

                    // Vincular el login externo
                    createResult = await _userManager.AddLoginAsync(user, info);
                    if (createResult.Succeeded)
                    {
                        _logger.LogInformation("✅ Usuario creado con Google: {Email}", email);
                        
                        // Iniciar sesión
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        TempData["SuccessMessage"] = $"¡Bienvenido {name ?? email}! Tu cuenta ha sido creada.";
                        return LocalRedirect(returnUrl);
                    }
                }

                // Si hubo errores al crear la cuenta
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                _logger.LogError("❌ Error al crear usuario con Google: {Errors}", errors);
                TempData["ErrorMessage"] = "No se pudo crear tu cuenta. Por favor intenta con email y contraseña.";
                return RedirectToAction(nameof(Register), new { returnUrl });
            }
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión");
            TempData["SuccessMessage"] = "Has cerrado sesión exitosamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}