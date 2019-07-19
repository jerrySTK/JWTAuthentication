using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NG_Core_Auth.Models.Entities
{
    public class AppUser:IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? FacebookId { get; set; }
        public string PictureUrl { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Customer> Customers {get;set;}
    }
}