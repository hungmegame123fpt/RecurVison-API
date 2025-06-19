using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class VerifyOtpDto
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
