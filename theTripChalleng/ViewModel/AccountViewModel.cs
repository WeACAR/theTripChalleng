// create view models for account
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using theTripChalleng.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using theTripChalleng.Data;
using System;

namespace theTripChalleng.ViewModels
{
    public class LoginViewModel
    {
        public string Phone { get; set; }
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
