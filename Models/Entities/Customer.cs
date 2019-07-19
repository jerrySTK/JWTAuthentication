using System.ComponentModel.DataAnnotations.Schema;

namespace NG_Core_Auth.Models.Entities {
    public class Customer {
        public int Id { get; set; }
        public long user_id { get; set; }
        
        [ForeignKey("user_id")]
        public AppUser User { get; set; } // navigation property
        public string Location { get; set; }
        public string Locale { get; set; }
        public string Gender { get; set; }
    }
}