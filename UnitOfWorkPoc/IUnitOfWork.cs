using System.Data.Entity;
using System;

namespace UnitOfWorkPoc
{
    public interface IUnitOfWork<out TContext>
        where TContext : DbContext,new()
    {
        TContext Context { get; }
        void CreateTransaction();
        void Commit();
        void RollBack();
        void Save();
    }
}
