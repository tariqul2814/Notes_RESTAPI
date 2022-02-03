using InsightRESTAPI.Common;
using InsightRESTAPI.Model.CommonModel;
using InsightRESTAPI.Model.Data;
using InsightRESTAPI.Model.DBModel;
using InsightRESTAPI.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightRESTAPI.Services.Admin
{
    public class TokenServices : ITokenServices
    {
        private readonly UnitOfWork _repo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly string requestTime = Utilities.GetRequestResponseTime();

        public TokenServices(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _repo = new UnitOfWork(_context);
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PayloadResponse<object>> SaveRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.CreatedBy = Convert.ToInt32(refreshToken.IdentityUserId);
            refreshToken.CreatedDate = DateTime.Now;
            await _repo.RefreshToken.AddAsync(refreshToken);
            var data = await _repo.SaveAsync();
            return new PayloadResponse<object>
            {
                Message = new List<string>() { "Token saved successfully" },
                Payload = data,
                PayloadType = "Save Refresh Token",
                RequestTime = requestTime,
                ResponseTime = Utilities.GenerateRefreshToken(),
                Success = true
            };
        }
        public string GetRefreshTokenByUserIdAndRefreshToken(string username, string refreshToken)
        {
            return _repo.RefreshToken.Where(x => x.IdentityUserId == username && x.Token == refreshToken).Select(x => x.Token).FirstOrDefault();
        }
        public async Task<PayloadResponse<object>> DeleteRefreshToken(string username, string refreshToken)
        {
            var tokenToDelete = _repo.RefreshToken.Where(x => x.IdentityUserId == username && x.Token == refreshToken).Select(x => x).FirstOrDefault();
            _repo.RefreshToken.Delete(tokenToDelete);
            var data = await _repo.SaveAsync();
            return new PayloadResponse<object>
            {
                Message = new List<string>() { "Token deleted successfully" },
                Payload = data,
                PayloadType = "Delete Refresh Token",
                RequestTime = requestTime,
                ResponseTime = Utilities.GenerateRefreshToken(),
                Success = true
            };

        }
    }

    public interface ITokenServices
    {
        Task<PayloadResponse<object>> SaveRefreshToken(RefreshToken refreshToken);
        string GetRefreshTokenByUserIdAndRefreshToken(string username, string refreshToken);
        Task<PayloadResponse<object>> DeleteRefreshToken(string username, string refreshToken);
    }
}
