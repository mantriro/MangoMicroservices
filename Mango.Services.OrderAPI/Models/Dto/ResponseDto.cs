﻿namespace Mango.Services.OrderAPI.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "";
        public string Role { get; set; } = "";

    }
}
