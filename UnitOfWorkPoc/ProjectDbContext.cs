using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace UnitOfWorkPoc
{
    public partial class ProjectDbContext : DbContext
    {
        public ProjectDbContext() : base("")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
        public virtual DbSet<Employee> Employees { get; set; }
    }
}
