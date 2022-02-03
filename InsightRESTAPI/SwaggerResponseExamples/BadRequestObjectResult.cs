using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsightRESTAPI.SwaggerResponseExamples
{
    public class BadRequestObjectResultExamples : IExamplesProvider<BadRequestObjectResult>
    {
        public BadRequestObjectResultExamples()
        {
        }
        public BadRequestObjectResult GetExamples()
        {
            return new BadRequestObjectResult(new
            {
                errors = new
                {
                    property_name = new[] { "Invalid property_name value" }
                },
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "One or more validation errors occurred.",
                status = 400,
                traceId = ""
            });
        }
    }
}
