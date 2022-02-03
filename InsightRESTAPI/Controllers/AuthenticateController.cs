using InsightRESTAPI.Common;
using InsightRESTAPI.Common.Configuration;
using InsightRESTAPI.Model.CommonModel;
using InsightRESTAPI.Model.Data;
using InsightRESTAPI.Model.DBModel;
using InsightRESTAPI.Model.ViewModel;
using InsightRESTAPI.Services.Admin;
using InsightRESTAPI.SwaggerResponseExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsightRESTAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthenticateController : Controller
    {
        private readonly IUserServices _services;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private ApplicationDbContext _context;
        private JWTServices Jwt { get; }
        private readonly ITokenServices _tokenServices;
        private readonly string requestTime = Utilities.GetRequestResponseTime();
        //private readonly IUserServices _userServices;
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        private readonly ConnectionStringConfig _connectionStringConfig;

        #region Private Services
        private async Task<IActionResult> ValidLoginResponse(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            LoggedInUserInfo userInfo = new LoggedInUserInfo();
            userInfo.UserID = user.Id;
            userInfo.Name = user.Name;
            userInfo.UserName = user.UserName;
            userInfo.Role = roles.FirstOrDefault();
            var refreshToken = new RefreshToken
            {
                IdentityUserId = user.Id.ToString(),
                Token = Utilities.GenerateRefreshToken(),
            };
            await _tokenServices.SaveRefreshToken(refreshToken);
            var response = new LoginResponseViewModel
            {
                successResonse = new SuccessfulLoginResponse
                {
                    Token = Jwt.GetTokenFor(user.Id.ToString(), roles.FirstOrDefault()),
                    RefreshToken = refreshToken.Token,
                    User = userInfo
                },
                TwoFactorEnabled = user.TwoFactorEnabled
            };
            return Ok(response);
        }
        #endregion

        public AuthenticateController(IUserServices services, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, JWTServices jwt, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, ITokenServices tokenServices, ConnectionStringConfig connectionStringConfig)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _services = services;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            Jwt = jwt;
            _context = context;
            _tokenServices = tokenServices;
            _connectionStringConfig = connectionStringConfig;
            optionsBuilder.UseSqlServer(_connectionStringConfig.DefaultConnection);
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(LoginResponseViewModel), 200)]
        [ProducesResponseType(typeof(LoginUnauthorizedResponseViewModel), 401)]
        [ProducesResponseType(typeof(LoginNotFoundResponseExamples), 404)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        public async Task<IActionResult> Login([FromBody] LoginModel info)
        {
            var user = await _userManager.FindByNameAsync(info.Username);

            #region Bad Login Attempts
            if (user == null || user.IsRemoved || !await _userManager.CheckPasswordAsync(user ?? new ApplicationUser(), info.Password))
            {
                var response = new LoginUnauthorizedResponseViewModel
                {
                    failedResponse = new FailedLoginResponse
                    {
                        ErrorMessage = "Invalid login credentials"
                    }
                };
                return Ok(response);
            }
            #endregion

            #region Basic Login
            if (!user.TwoFactorEnabled)
            {
                return await ValidLoginResponse(user);
            } // Basic Login
            #endregion

            return Unauthorized();
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutModel logoutModel)
        {
            var principal = Jwt.GetPrincipalFromExpiredToken(logoutModel.Token);
            var user = await _userManager.FindByIdAsync(principal.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault());
            var roles = await _userManager.GetRolesAsync(user);
            var data = await _tokenServices.DeleteRefreshToken(user.Id.ToString(), logoutModel.RefreshToken);
            return Ok(new PayloadResponse<object>
            {
                Success = true,
                Message = new List<string>() { "You have been logged out successfully" },
                Payload = data,
                PayloadType = "Logout",
                RequestTime = requestTime,
                ResponseTime = Utilities.GetRequestResponseTime()
            });
        }

        [ProducesResponseType(typeof(SuccessfulLoginResponse), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel refreshTokenModel)
        {
            try
            {
                if (refreshTokenModel != null)
                {
                    var principal = Jwt.GetPrincipalFromExpiredToken(refreshTokenModel.Token);
                    var user = await _userManager.FindByIdAsync(principal.Claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault());
                    var roles = await _userManager.GetRolesAsync(user);
                    var savedRefreshToken = _tokenServices.GetRefreshTokenByUserIdAndRefreshToken(user.Id.ToString(), refreshTokenModel.RefreshToken); //retrieve the refresh token from a data store
                    if (savedRefreshToken != refreshTokenModel.RefreshToken)
                        throw new SecurityTokenException("Invalid refresh token");

                    var newJwtToken = Jwt.GetTokenFor(user.Id.ToString(), roles.FirstOrDefault());
                    var newRefreshToken = new RefreshToken()
                    {
                        Token = Common.Utilities.GenerateRefreshToken(),
                        IdentityUserId = user.Id.ToString()
                    };
                    await _tokenServices.DeleteRefreshToken(user.Id.ToString(), refreshTokenModel.RefreshToken);
                    await _tokenServices.SaveRefreshToken(newRefreshToken);
                    LoggedInUserInfo userInfo = new LoggedInUserInfo();
                    userInfo.UserID = user.Id;
                    userInfo.Name = user.Name;
                    userInfo.UserName = user.UserName;
                    userInfo.Role = roles.FirstOrDefault();
                    var success = new SuccessfulLoginResponse
                    {
                        Token = newJwtToken,
                        RefreshToken = newRefreshToken.Token,
                        User = userInfo
                    };
                    return Ok(success);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("IDX10503: Signature validation failed.");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(SuccessfulLoginResponse), 200)]
        [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
        [Route("Registration")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel registerViewModel)
        {
            try
            {
                return Ok(await _services.Post(registerViewModel));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
