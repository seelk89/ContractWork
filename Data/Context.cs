using System;
using Microsoft.EntityFrameworkCore;

namespace ContractWork.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Word> Words { get; set; }
    }
}
