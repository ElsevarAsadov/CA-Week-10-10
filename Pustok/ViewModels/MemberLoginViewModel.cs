using System.ComponentModel.DataAnnotations;

namespace Pustok.ViewModels
{
    public class MemberLoginViewModel
    {
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength:20, MinimumLength =8)]
        public string Password { get; set; }
    }
}
