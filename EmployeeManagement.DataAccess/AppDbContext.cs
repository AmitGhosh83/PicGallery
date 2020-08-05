using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PicGallery.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.DataAccess
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext( DbContextOptions<AppDbContext> options): base(options)
        {
           
        }
        public DbSet<EmployeeModel> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
