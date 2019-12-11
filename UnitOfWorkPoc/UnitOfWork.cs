using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace UnitOfWorkPoc
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable
    where TContext : DbContext, new()
    {

        #region Fields

        //Here TContext is nothing but your DBContext class
        //In our example it is EmployeeDBContext class

        private readonly TContext _context;
        private bool _disposed;
        private string _errorMessage = string.Empty;
        private DbContextTransaction _objTran;
        private Dictionary<string, object> _repositories;
        #endregion

        #region Ctor

        //Using the Constructor we are initializing the _context variable is nothing but
        //we are storing the DBContext (EmployeeDBContext) object in _context variable

        public UnitOfWork()
        {
            _context = new TContext();
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public TContext Context
        {
            get { return _context; }
        }
        public void CreateTransaction()
        {
            _objTran = _context.Database.BeginTransaction();
        }
        public void Commit()
        {
            _objTran.Commit();
        }
        public void RollBack()
        {
            _objTran.Rollback();
            _objTran.Dispose();
        }
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        _errorMessage += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;
                throw new Exception(_errorMessage, dbEx);
            }
        }
        public GenericRepository<T> GenericRepository<T>() where T :class
        {
            if (_repositories == null)
                _repositories = new Dictionary<string, object>();

            var type = typeof(T).Name;

            if(!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<T>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)),
                    _context);

                _repositories.Add(type, repositoryInstance);
            }
            return (GenericRepository<T>)_repositories[type];
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }
        #endregion

    }
}
