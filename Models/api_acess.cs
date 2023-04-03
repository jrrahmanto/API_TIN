using System.ComponentModel.DataAnnotations;

namespace API_TIN_KBI.Models
{
    public class api_acess
    {
        [Key]
        public int id { get; set; }
        public string? userid { get; set; }
        public string? apikey { get; set; }
        public int? actived { get; set; }
    }
}
