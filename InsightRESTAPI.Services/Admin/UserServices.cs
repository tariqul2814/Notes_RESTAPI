using InsightRESTAPI.Common;
using InsightRESTAPI.Common.Configuration;
using InsightRESTAPI.Model.CommonModel;
using InsightRESTAPI.Model.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InsightRESTAPI.Model.ViewModel;
using System.Linq;

namespace InsightRESTAPI.Services.Admin
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private IServiceProvider _serviceProvider;
        private readonly string requestTime = Utilities.GetRequestResponseTime();
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        private readonly ConnectionStringConfig _connectionStringConfig;
        public UserServices(UserManager<ApplicationUser> userManager
            , RoleManager<ApplicationRole> roleManager
            , IServiceProvider serviceProvider
            , ConnectionStringConfig connectionStringConfig)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _serviceProvider = serviceProvider;
            _connectionStringConfig = connectionStringConfig;
            optionsBuilder.UseSqlServer(_connectionStringConfig.DefaultConnection);
        }

        #region Internal Services
        public static int GetUserId(System.Security.Claims.ClaimsPrincipal user)
        {
            return user?.Identity?.Name?.ToInt32() ?? 0;
        }
        #endregion

        public async Task<PayloadResponse<ApplicationUser>> Post(RegisterViewModel registerViewModel)
        {
            var user = new ApplicationUser();
            if (await _userManager.FindByEmailAsync(registerViewModel.Email) == null)
            {
                user.Email = registerViewModel.Email;
                user.Name = registerViewModel.Name;
                user.UserName = registerViewModel.Email;
                user.IsRemoved = false;
                user.DateOfBirth = registerViewModel.DateOfBirth;

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return new PayloadResponse<ApplicationUser>
                    {
                        Success = false,
                        Message = result.Errors.Select(x => x.Description).ToList(),
                        Payload = null,
                        PayloadType = "Create User",
                        RequestTime = requestTime,
                        ResponseTime = Utilities.GetRequestResponseTime()
                    };
                }
                result = await _userManager.AddPasswordAsync(user, registerViewModel.Password);
                if (!result.Succeeded)
                {
                    return new PayloadResponse<ApplicationUser>
                    {
                        Success = false,
                        Message = result.Errors.Select(x => x.Description).ToList(),
                        Payload = null,
                        PayloadType = "Create User",
                        RequestTime = requestTime,
                        ResponseTime = Utilities.GetRequestResponseTime()
                    };
                }
                result = await _userManager.AddToRoleAsync(user, "User");
                if (!result.Succeeded)
                {
                    return new PayloadResponse<ApplicationUser>
                    {
                        Success = false,
                        Message = result.Errors.Select(x => x.Description).ToList(),
                        Payload = null,
                        PayloadType = "Create User",
                        RequestTime = requestTime,
                        ResponseTime = Utilities.GetRequestResponseTime()
                    };
                }

                return new PayloadResponse<ApplicationUser>
                {
                    Success = true,
                    Message = new List<string>() { "User created successfully" },
                    Payload = user,
                    PayloadType = "Create User",
                    RequestTime = requestTime,
                    ResponseTime = Utilities.GetRequestResponseTime()
                };
            }
            else
            {
                return new PayloadResponse<ApplicationUser>
                {
                    Success = false,
                    Message = new List<string>() { "User creation failed as username has already been taken" },
                    Payload = user,
                    PayloadType = "Create User",
                    RequestTime = requestTime,
                    ResponseTime = Utilities.GetRequestResponseTime()
                };
            }

        }
    }

    public interface IUserServices
    {
        Task<PayloadResponse<ApplicationUser>> Post(RegisterViewModel registerViewModel);
    }
}
