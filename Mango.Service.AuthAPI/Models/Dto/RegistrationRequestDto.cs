﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Mango.Service.AuthAPI.Models.Dto
{
    public class RegistrationRequestDto
    {
        //public string UserId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }

        public string? Role { get; set; }
    }
}
