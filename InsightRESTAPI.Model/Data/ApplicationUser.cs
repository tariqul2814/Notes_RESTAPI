using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsightRESTAPI.Model.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool IsRemoved { get; set; }
    }

    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base()
        {

        }

    }
}
