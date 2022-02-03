using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace InsightRESTAPI.Model.CommonModel
{
    public class ErrorResponse
    {
        public static BadRequestObjectResult BadRequest(string error)
        {
            return new BadRequestObjectResult(new ResponseObject()
            {
                errors = new { general = new[] { error } }
            });
        }

        public static BadRequestObjectResult BadRequest(Object errors)
        {
            return new BadRequestObjectResult(new ResponseObject()
            {
                errors = errors
            });
        }

        public static BadRequestObjectResult ServerError(string error = "Something went wrong. Please try again later.")
        {
            // TODO: Return proper 500 error status
            return new BadRequestObjectResult(new ResponseObject()
            {
                errors = new { general = new[] { error } }
            });
        }

        public static BadRequestObjectResult FromServiceResponseMessage(List<string> messages)
        {
            try
            {
                var errors = new ExpandoObject() as IDictionary<string, object>;

                foreach (var msg in messages)
                {
                    var seperatorIndex = msg.IndexOf(':');
                    var fieldName = (seperatorIndex == -1 ? "general" : msg.Substring(0, seperatorIndex)).Trim().Replace("@", "");
                    var fieldError = (seperatorIndex == -1 ? msg : msg.Substring(seperatorIndex + 1)).Trim();

                    if (!errors.ContainsKey(fieldName))
                    {
                        errors[fieldName] = new List<string>() { fieldError };
                    }
                    else
                    {
                        (errors[fieldName] as List<string>).Add(fieldError);
                    }
                }

                return ErrorResponse.BadRequest(errors);
            }
            catch (Exception)
            {
                return ErrorResponse.BadRequest("Something went wrong.");
            }
        }
    }
    
    class ResponseObject
    {
        public Object errors { get; set; }
        public string type { get; set; } = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        public string title { get; set; } = "One or more validation errors occurred.";
        public int status { get; set; } = 400;
        public string traceId { get; set; } = "";
    }
}
