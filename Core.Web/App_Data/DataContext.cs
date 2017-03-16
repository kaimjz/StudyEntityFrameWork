using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Web.Models;
using Microsoft.EntityFrameworkCore;
namespace Core.Web.App_Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
          : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }
    }
}
