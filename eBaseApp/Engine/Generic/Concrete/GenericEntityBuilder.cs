using eBaseApp.DataAccessLayer;
using eBaseApp.Engine.Generic.Abstract;
using IdentityModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace eBaseApp.Engine.Generic.Concrete
{
    public class GenericEntityBuilder : IGenericEntityBuilder
    {
        private readonly eServicesDbContext _context;
        private readonly SolarESBDbContext _solarESB;
        public GenericEntityBuilder(eServicesDbContext context)
        {
            _context = context;
        }
        public GenericEntityBuilder(SolarESBDbContext solar)
        {
            _solarESB = solar;
        }
        public void AddToTable<T>(T entity) where T : class, new() => _context.Set<T>().Add(entity);

        public void Complete() => _context.SaveChanges();

        public void Dispose() => _context.Dispose();

        public IEnumerable<T> GetActionsTypes<T>() where T : class, new() => _context.Set<T>().ToList();

        public T GetById<T>(Int32 Id) where T : class, new() =>  _context.Set<T>().Find(Id) ?? null;
        public T GetSolarById<T>(Int32 Id) where T : class, new() => _solarESB.Set<T>().Find(Id) ?? null;
        public IEnumerable<T> GetSolar<T>() where T : class, new() => _solarESB.Set<T>().ToList();

        public T GetTypeByKey<T>(Expression<Func<T, bool>> prediction) where T : class, new()
        {
            return _context.Set<T>().FirstOrDefault(prediction);
        }

        public IEnumerable<T> GetTypeByPrediction<T>(Expression<Func<T, bool>> prediction) where T : class, new()
        {
            return _context.Set<T>().Where(prediction).ToList();
        }

        public void RangeAdd<T>(List<T> entity) where T : class, new() => _context.Set<T>().AddRange(entity);

        public void UpdateTable<T>(T entity) where T : class, new()
        {
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}