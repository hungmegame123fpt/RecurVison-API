using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class SuggestedJob
    {
        [JsonPropertyName("job_title")]
        public string JobTitle { get; set; } = null!;

        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; } = null!;

        [JsonPropertyName("required_skills")]
        public List<string> RequiredSkills { get; set; } = new();

        [JsonPropertyName("salary_range")]
        public string SalaryRange { get; set; } = null!;

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("url")]
        public string Url { get; set; } = null!;
    }
}
