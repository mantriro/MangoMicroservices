﻿using System.ComponentModel.DataAnnotations;

namespace Mango.Service.AuthAPI.Models.Dto
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required] 
        public string Password { get; set; }
    }
}
