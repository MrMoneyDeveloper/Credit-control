using IdentityModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eBaseApp.Engine.Generic.Abstract
{
    public interface IGenericEntityBuilder
    {
        void Dispose();
        void Complete();
        void AddToTable<T>(T entity) where T : class, new();
        void RangeAdd<T>(List<T> entity) where T : class, new();
        T GetById<T>(Int32 Id) where T : class, new();
        IEnumerable<T> GetActionsTypes<T>() where T : class, new();
        IEnumerable<T> GetSolar<T>() where T : class, new();
        T GetSolarById<T>(Int32 Id) where T : class, new();
        T GetTypeByKey<T>(Expression<Func<T, bool>> prediction) where T : class, new();
        IEnumerable<T> GetTypeByPrediction<T>(Expression<Func<T, bool>> prediction) where T : class, new();
        void UpdateTable<T>(T entity) where T : class, new();
    }
}
