﻿using System.ComponentModel.DataAnnotations;

namespace YATDL.Models.Account
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

    }
}