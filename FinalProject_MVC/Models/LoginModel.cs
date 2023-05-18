using System.ComponentModel.DataAnnotations;

namespace FinalProject_MVC.Models
{
    public class LoginModel
    {
        //validate
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
