using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
    public class AiSessionRequest
    {
        public string CvFileUrl { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;

    }
}
