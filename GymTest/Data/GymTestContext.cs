using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GymTest.Models;

namespace GymTest.Models
{
    public class GymTestContext : DbContext
    {
        public GymTestContext (DbContextOptions<GymTestContext> options)
            : base(options)
        {
        }

        public DbSet<GymTest.Models.User> User { get; set; }

        public DbSet<GymTest.Models.Payment> Payment { get; set; }

        public DbSet<GymTest.Models.Assistance> Assistance { get; set; }
    }
}
