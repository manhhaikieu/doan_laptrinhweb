﻿using Microsoft.AspNetCore.Identity;

namespace LAB03.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }

        
    }
}
