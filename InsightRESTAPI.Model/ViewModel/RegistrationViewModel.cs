using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InsightRESTAPI.Model.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password should be of minimum 6 character and maximum 50 character.")]
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
