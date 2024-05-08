using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Models.Identity
{
    public class PasswordResetViewModel
    {
        public string Email { get; set; }

        //public string Token {get; set;}

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The Password cannot be empty.")]
        [DisplayName("Password")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [JsonPropertyName("password")]
        public string Password
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [DisplayName("Password confirmation")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "The entered passwords do not match.")]
        [JsonPropertyName("passwordConfirmation")]
        public string PasswordConfirmation
        {
            get;
            set;
        }
    }
}
