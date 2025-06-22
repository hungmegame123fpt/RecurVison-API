using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class ParseCvRequest
    {
        public int UserId { get; set; }
        public int CvId { get; set; }
    }
}
