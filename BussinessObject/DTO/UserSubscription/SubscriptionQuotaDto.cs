using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.UserSubscription
{
    public class SubscriptionQuotaDto
    {
        public int InterviewPerDayRemaining { get; set; }
        public int MaxInterviewPerDay { get; set; }

        public int VoiceInterviewRemaining { get; set; }
        public int MaxVoiceInterview { get; set; }

        public int CvRemaining { get; set; }
        public int MaxCvsAllowed { get; set; }
    }
}
