using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.CV
{
    public class EditPdfRequest
    {
        public int CvId { get; set; }
        public string NewText { get; set; }
    }
}
