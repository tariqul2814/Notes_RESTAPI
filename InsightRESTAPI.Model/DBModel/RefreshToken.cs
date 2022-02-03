using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsightRESTAPI.Model.DBModel
{
    public class RefreshToken : BaseModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string IdentityUserId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Token { get; set; }
    }
}
