﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class CreateFeedbackRequest
    {
        public string Content { get; set; } = string.Empty;
        public int? Rating { get; set; }
    }
}
