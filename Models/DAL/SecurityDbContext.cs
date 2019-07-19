using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NG_Core_Auth.Models.Entities;

namespace NG_Core_Auth.Models.DAL {
    public class SecurityDbContext : IdentityDbContext<AppUser,AppRole,long> {

        public DbSet<Customer> Customers {get;set;}
        public SecurityDbContext (DbContextOptions options) : base (options) {

        }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().Property(p=>p.Id).ValueGeneratedOnAdd();
            builder.Entity<AppRole>().Property(p=>p.Id).ValueGeneratedOnAdd();
        }
    }
}