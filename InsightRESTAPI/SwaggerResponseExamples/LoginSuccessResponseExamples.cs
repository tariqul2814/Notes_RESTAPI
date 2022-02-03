using InsightRESTAPI.Model.ViewModel;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightRESTAPI.SwaggerResponseExamples
{
    public class LoginSuccessResponseExamples : IExamplesProvider<LoginResponseViewModel>
    {
        public LoginResponseViewModel GetExamples()
        {
            return new LoginResponseViewModel
            {
                successResonse = new SuccessfulLoginResponse
                {
                    RefreshToken = "Yn0Aa2TazWdDcbnENdEkDmIVoyj66M02gXzfRaYpZlM",
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEwMjgiLCJyb2xlIjoiVXNlciIsIm5iZiI6MTU5MTM3OTAxMCwiZXhwIjoxNTkxNTUxODEwLCJpYXQiOjE1OTEzNzkwMTB9.IeDtntMS7bGOTDQCtg92MolSbv2xe1GWQUjfp2h2Ls4",
                    User = new LoggedInUserInfo
                    {
                        Name = "John",
                        Role = "User",
                        UserID = 1,
                        UserName = "muyeen.cse@gmail.com"
                    }
                },
                failedResponse = null
            };
        }
    }
}
