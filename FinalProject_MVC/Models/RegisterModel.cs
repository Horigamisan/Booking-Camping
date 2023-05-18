using System.ComponentModel.DataAnnotations;

namespace FinalProject_MVC.Models
{
    public class RegisterModel
    {
        //validate email
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string? Email { get; set; }
        //validate password
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? Password { get; set; }
        //validate confirm password
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string? ConfirmPassword { get; set; }
        //validate name
        [Required(ErrorMessage = "Name is required")]
        [MinLength(6, ErrorMessage = "Name must be at least 6 characters")]
        public string? Name { get; set; }
        //validate phone
        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^(\+84|0)\d{9,10}$", ErrorMessage = "Phone is invalid")]
        public string? Phone { get; set; }
        //validate address
        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
    }
}
