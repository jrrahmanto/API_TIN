using System.ComponentModel.DataAnnotations;

namespace API_TIN_KBI.Models
{
    public class api_log
    {
        [Key]
        public int id { get; set; }
        public string userid { get; set; }
        public string timestamp { get; set; }
        public string signature { get; set; }
        public string respons { get; set; }
        public string request { get; set; }
        public DateTime createddate { get; set; }
    }
}
