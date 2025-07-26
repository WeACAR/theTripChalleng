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
using theTripChalleng.ViewModel;

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

    // edit profile view model
    public class EditProfileViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public byte[]? Image { get; set; } // Optional image field
        public IFormFile? ImageFile { get; set; }
        public bool IsImageDeleted { get; set; }
    }

    // ViewModel for user details containing user information, points history, and point requests
    public class UserDetailsViewModel
    {
        public User User { get; set; }
        public List<PointsHistoryViewModel> PointsHistory { get; set; }
        public List<PointRequestViewModel> PointsRequests { get; set; }

        public string RoleName { get; set; }


    }

    // ViewModel for add reward (same but with image file)
    public class AddRewardViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long MinPoints { get; set; }
        public long CategoryId { get; set; }
        public IFormFile ImageFile { get; set; }
        public byte[]? Image { get; set; } // Optional image field
    }

    // ViewModel for edit reward
    public class EditRewardViewModel
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? MinPoints { get; set; }
        public long? CategoryId { get; set; }
        public bool IsImageDeleted { get; set; } // Flag to indicate if the image should be deleted
        public IFormFile? ImageFile { get; set; } // Optional image file for upload
        public byte[]? Image { get; set; } // Optional image field
    }
}
