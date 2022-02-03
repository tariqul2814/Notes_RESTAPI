using InsightRESTAPI.Model.ViewModel;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightRESTAPI.SwaggerResponseExamples
{
    public class LoginUnauthorizedResponseExamples : IExamplesProvider<LoginUnauthorizedResponseViewModel>
    {
        public LoginUnauthorizedResponseViewModel GetExamples()
        {
            return new LoginUnauthorizedResponseViewModel
            {
                successResonse = null,
                failedResponse = new FailedLoginResponse
                {
                    Error = 1007,
                    Token = ""
                }
            };
        }
    }
}
