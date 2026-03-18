using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicAI.Infrastructure.Persistence
{
    public class ClinicDbContext :DbContext
    {
        public ClinicDbContext(DbContextOptions<ClinicDbContext> options):base(options)
        {

        }
        public DbSet<object> Dummy { get; set; }
    }
}
