using Dressify.DataAccess.Dtos;
using Dressify.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dressify.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync( string[]? includes = null);
        Task<IEnumerable<T>> GetAllAsync(int? take=null, int? skip=null ,string[]? includes = null);
        T Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        // for paging 
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip ,string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take, string[] includes = null);
        ///--------------------------------------------------------------------------
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip,
           Expression<Func<T, object>> orderBy = null, string orderByDirection = SD.Ascending, string[] includes = null);
        //Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take,
        //   Expression<Func<T, object>> orderBy = null, string orderByDirection = SD.Ascending, string[] includes = null);
        T Add(T entity);
        Task<T> AddAsync(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        T Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);

        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool ValidatePassword(ValidatePasswordDto model);

    }
}
