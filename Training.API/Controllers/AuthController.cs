using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.Options;
using Training.API.Auth;
using Training.Application.Features.Account.Commands.ErasePersonalData;
using Training.Application.Features.Subscription.Queries.HasActiveSubscription;
using Training.Domain;
using Training.Domain.Models;

namespace Training.API.Controllers
{
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<TrainingUser> _signInManager;
        private readonly UserManager<TrainingUser> _userManager;
        private readonly IAntiforgery _antiforgery;
        private readonly IMediator _mediator;
        private readonly AuthOptions _auth;

        public AuthController(
            SignInManager<TrainingUser> signInManager,
            UserManager<TrainingUser> userManager,
            IAntiforgery antiforgery,
            IMediator mediator,
            IOptions<AuthOptions> auth)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _antiforgery = antiforgery;
            _mediator = mediator;
            _auth = auth.Value;
        }

        [HttpGet("oauth/{provider}")]
        public IActionResult Start(string provider, string? returnUrl)
        {
            if (!IsGoogle(provider))
            {
                return NotFound();
            }

            if (!_auth.Google.IsStub)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }

            return RedirectToAction(nameof(Stub), new { provider = "google", returnUrl = SafeReturnUrl(returnUrl) });
        }

        [HttpGet("oauth/{provider}/stub")]
        public IActionResult Stub(string provider, string? returnUrl)
        {
            if (!IsGoogleStub(provider))
            {
                return NotFound();
            }

            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var html = BuildStubPage(tokens.RequestToken!, SafeReturnUrl(returnUrl));
            return Content(html, "text/html; charset=utf-8");
        }

        [EnableRateLimiting(TrainingRateLimits.Auth)]
        [HttpPost("oauth/{provider}/stub")]
        public async Task<IActionResult> StubSignIn(string provider, string? returnUrl)
        {
            if (!IsGoogleStub(provider))
            {
                return NotFound();
            }

            if (!await IsRequestValidAsync())
            {
                return BadRequest();
            }

            var callback = Url.Action(nameof(Callback), values: new { returnUrl = SafeReturnUrl(returnUrl) })!;
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callback);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _auth.Google.ProviderKey),
                new(ClaimTypes.Email, _auth.Google.Email),
                new(ClaimTypes.Name, _auth.Google.Name)
            };
            var identity = new ClaimsIdentity(claims, IdentityConstants.ExternalScheme);
            await HttpContext.SignInAsync(
                IdentityConstants.ExternalScheme,
                new ClaimsPrincipal(identity),
                properties);

            return Redirect(callback);
        }

        [EnableRateLimiting(TrainingRateLimits.Auth)]
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string? returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user?.AcceptedTermsAt is null)
            {
                return Redirect(ConsentUrl());
            }

            await EnsureStandardRoleAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: true);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return Redirect(SafeReturnUrl(returnUrl));
        }

        [EnableRateLimiting(TrainingRateLimits.Auth)]
        [HttpPost("consent")]
        public async Task<IActionResult> Consent()
        {
            if (!await IsRequestValidAsync())
            {
                return BadRequest();
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info is null)
            {
                return BadRequest();
            }

            var user = await FindOrLinkUserAsync(info);
            if (user is null)
            {
                return BadRequest();
            }

            user.AcceptedTermsAt = DateTimeOffset.UtcNow;
            var updated = await _userManager.UpdateAsync(user);
            if (!updated.Succeeded)
            {
                return BadRequest();
            }

            await EnsureStandardRoleAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: true);
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("/api/v1/me")]
        public async Task<IActionResult> Erase()
        {
            if (!await IsRequestValidAsync())
            {
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var status = await _mediator.Send(new ErasePersonalDataCommand(userId), HttpContext.RequestAborted);
            if (status == ErasePersonalDataStatus.LastAdmin)
            {
                return Conflict(new { code = "lastAdmin" });
            }

            if (status != ErasePersonalDataStatus.Erased)
            {
                return NotFound();
            }

            await _signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
            {
                HttpOnly = false,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            if (User.Identity?.IsAuthenticated != true)
            {
                return Ok(new AuthMeResponse(false, null, null, [], false));
            }

            var user = await _userManager.GetUserAsync(User);
            var roles = user is null
                ? []
                : (await _userManager.GetRolesAsync(user)).ToArray();
            var premiumActive = user is not null
                && await _mediator.Send(new HasActiveSubscriptionQuery(user.Id), HttpContext.RequestAborted);
            return Ok(new AuthMeResponse(true, user?.Email, user?.UserName, roles, premiumActive));
        }

        [EnableRateLimiting(TrainingRateLimits.Auth)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!await IsRequestValidAsync())
            {
                return BadRequest();
            }

            await _signInManager.SignOutAsync();
            return NoContent();
        }

        private async Task<bool> IsRequestValidAsync()
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
                return true;
            }
            catch (AntiforgeryValidationException)
            {
                return false;
            }
        }

        private bool IsGoogle(string provider)
        {
            return provider.Equals("google", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsGoogleStub(string provider)
        {
            return IsGoogle(provider) && _auth.Google.IsStub;
        }

        private async Task<TrainingUser?> FindOrLinkUserAsync(ExternalLoginInfo info)
        {
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user is not null)
            {
                return user;
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new TrainingUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var created = await _userManager.CreateAsync(user);
                if (!created.Succeeded)
                {
                    return null;
                }
            }

            var login = await _userManager.AddLoginAsync(user, info);
            return login.Succeeded ? user : null;
        }

        private async Task EnsureStandardRoleAsync(TrainingUser user)
        {
            if ((await _userManager.GetRolesAsync(user)).Count == 0)
            {
                await _userManager.AddToRoleAsync(user, TrainingRoles.Standard);
            }
        }

        private string ConsentUrl()
        {
            return $"{_auth.UiOrigin.TrimEnd('/')}/consent";
        }

        private string SafeReturnUrl(string? returnUrl)
        {
            var origin = _auth.UiOrigin.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(returnUrl)
                && Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri)
                && string.Equals(uri.GetLeftPart(UriPartial.Authority), origin, StringComparison.OrdinalIgnoreCase))
            {
                return uri.ToString();
            }

            return origin;
        }

        private string BuildStubPage(string requestToken, string returnUrl)
        {
            var email = WebUtility.HtmlEncode(_auth.Google.Email);
            var name = WebUtility.HtmlEncode(_auth.Google.Name);
            var token = WebUtility.HtmlEncode(requestToken);
            var back = WebUtility.HtmlEncode(Uri.EscapeDataString(returnUrl));
            return $$"""
                <!DOCTYPE html>
                <html lang="uk">
                <head>
                  <meta charset="utf-8">
                  <title>Заглушка Google</title>
                </head>
                <body>
                  <h1>Заглушка Google</h1>
                  <p>Це не справжній Google. Вхід створить локальну сесію для <strong>{{name}}</strong> ({{email}}).</p>
                  <form method="post" action="/api/v1/auth/oauth/google/stub?returnUrl={{back}}">
                    <input type="hidden" name="__RequestVerificationToken" value="{{token}}">
                    <button type="submit">Увійти як {{email}}</button>
                  </form>
                </body>
                </html>
                """;
        }
    }

    public record AuthMeResponse(
        bool IsAuthenticated,
        string? Email,
        string? Name,
        IReadOnlyList<string> Roles,
        bool PremiumActive);
}
