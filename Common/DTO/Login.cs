using System.ComponentModel.DataAnnotations;
using Common.Enum;

namespace Common.DTO
{
    public class LoginRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "ID CANNOT BE EMPTY")]
        [StringLength(50, ErrorMessage = "ID IS TOO LONG")]
         public string Id { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
        [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        [Required] 
        public ErrorCode Result { get; set; } = ErrorCode.None;
        [Required] 
        public string AuthToken { get; set; } = "";
    }
}
