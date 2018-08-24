using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetoSaude.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoSaude.Data
{
    public class IDatabaseContext : DbContext
    {
        public IDatabaseContext(DbContextOptions<IDatabaseContext> options) : base(options) {}

        public DbSet<IUser> Users { get; set; }
    }
}
