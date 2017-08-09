﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCS.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Last name")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Middle name")]
        [StringLength(50, MinimumLength = 2)]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Given name")]
        [StringLength(50, MinimumLength = 2)]
        public string GivenName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(50, MinimumLength = 5)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Zone Group")]
        [StringLength(30, MinimumLength = 1)]
        public string ZoneGroupCode { get; set; }

        [Required]
        [Display(Name = "Role")]
        [StringLength(30, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Status")]
        [StringLength(30, MinimumLength = 1)]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Division")]
        [StringLength(30, MinimumLength = 1)]
        public string Division { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [StringLength(20, MinimumLength = 5)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        [System.Web.Mvc.HiddenInput(DisplayValue = false)]
        public string LocalPass { get; set; }
        public string Zone { get; set; }

    }

    public class UpdateUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(20, MinimumLength = 3)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Middle name")]
        [StringLength(20, MinimumLength = 3)]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Given name")]
        [StringLength(20, MinimumLength = 3)]
        public string GivenName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(30, MinimumLength = 5)]
        public string Email { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
