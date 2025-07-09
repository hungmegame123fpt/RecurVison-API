using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AiClient
{
	public class CvAnalysisResultRequest
	{
		[Required]
		public int CvId { get; set; }

		[Required]
		public IFormFile jdFile { get; set; }
	}
}
