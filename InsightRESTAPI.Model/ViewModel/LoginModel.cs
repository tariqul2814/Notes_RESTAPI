using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsightRESTAPI.Model.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string TwoFactorCode { get; set; }
    }

    public class EmailConfirmationModel
    {
        [Required(ErrorMessage = "Two Factor Code is required")]
        public string EmailConfirmationCode { get; set; }
    }

    public class LoggedInUserInfo
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    public class SuccessfulLoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public LoggedInUserInfo User { get; set; }
        public bool IsAPTXAdminSetupCompleted { get; set; }
        public bool IsCompanySetupCompleted { get; set; }
        public bool HasAnyPackage { get; set; }
    }

    public class FailedLoginResponse
    {
        public int Error { get; set; }
        public string ErrorMessage { get; set; }
        public string Token { get; set; } = "";
    }

    public class ErrorInformation
    {
        public string Error { get; set; }
    }

    public class LoginResponseViewModel
    {
        public SuccessfulLoginResponse successResonse { get; set; }
        public FailedLoginResponse failedResponse { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool TwoFactorCodeSent { get; set; }
        public bool TwoFactorCodeValidated { get; set; }
    }

    public class LoginUnauthorizedResponseViewModel
    {
        public SuccessfulLoginResponse successResonse { get; set; }
        public FailedLoginResponse failedResponse { get; set; }
    }

    public class LoginNotFoundResponseViewModel
    {
        public SuccessfulLoginResponse successResonse { get; set; }
        public FailedLoginResponse failedResponse { get; set; }
    }

    public class RefreshTokenModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LogoutModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

}
