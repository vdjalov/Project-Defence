using System;
using System.Collections.Generic;
using System.Text;
using ClercSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClercSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {

        protected AppDbContext(){}

        public AppDbContext(DbContextOptions options) : base(options){}

        public DbSet<Category> Categories { get; set; }
        public DbSet<Category> Departments { get; set; }
        public DbSet<Category> Documents { get; set; }
        public DbSet<Category> DocumentLogs { get; set; }
        public DbSet<Category> DocumentsUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
