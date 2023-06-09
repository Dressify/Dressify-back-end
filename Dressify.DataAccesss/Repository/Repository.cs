﻿using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Dressify.DataAccess.Dtos;
using Dressify.DataAccess.Repository.IRepository;
using Dressify.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dressify.DataAccess.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Dressify.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _context;

        //constructor
        public Repository(ApplicationDbContext context)
        {
            _context = context;
           
        }

        //Return all rows of Table
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();

        }

        public async Task<IEnumerable<T>> GetAllAsync( string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync(int? take = null, int? skip = null, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
           
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.ToListAsync();
        }
        //Get specific element using ID
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        //Get specific element using experession 
        //include used to get data belong to element from another table which element have a foreign key
        //Ex: Find(user => user.name == "x", include = new []{ " Purches "})
        public T Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.SingleOrDefault(criteria);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return await query.SingleOrDefaultAsync(criteria);
        }
        //Get list of elements using experession 
        //include used to get data belong to element from another table which element have a foreign key
        //Ex: FindAll(user => user.name == "x", include = new []{ " Purches "})
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Where(criteria).ToList();
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }


        /// <summary>
        ///  next 2 functions for paging
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
             string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);


            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.ToList();
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take,
             string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);
            if(!query.Any())
                return Enumerable.Empty<T>();
            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if (take.HasValue)
                query = query.Take(take.Value);
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.ToListAsync();
        }

        //Get list of elements using experession 
        //include used to get data belong to element from another table which element have a foreign key
        //Ex: FindAll(user => user.name == "x", include = new []{ " Purches "})
        //Skip, skips elements up to a specified position starting from the first element in a sequence.
        //Take, takes elements up to a specified position starting from the first element in a sequence.
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
           Expression<Func<T, object>> orderBy = null, string orderByDirection = SD.Ascending, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);


            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
            {
                if (orderByDirection == SD.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.ToList();
        }

        //public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
        //    Expression<Func<T, object>> orderBy = null, string orderByDirection = SD.Ascending, string[] includes = null)
        //{
        //    IQueryable<T> query = _context.Set<T>().Where(criteria);

        //    if (take.HasValue)
        //        query = query.Take(take.Value);

        //    if (skip.HasValue)
        //        query = query.Skip(skip.Value);

        //    if (orderBy != null)
        //    {
        //        if (orderByDirection == SD.Ascending)
        //            query = query.OrderBy(orderBy);
        //        else
        //            query = query.OrderByDescending(orderBy);
        //    }
        //    if (includes != null)
        //        foreach (var include in includes)
        //            query = query.Include(include);

        //    return await query.ToListAsync();
        //}
       
        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }
        // Add list of elements 
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public T Update(T entity)
        {
            _context.Update(entity);
            return entity;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        //Delete list of elements 
        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
        public int Count()
        {
            return _context.Set<T>().Count();
        }
        
        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        //count number of elements that Fulfill the required conditions
        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }
        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }


        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        //Admins And Super Admins Are
        //validate Password
        public bool ValidatePassword(ValidatePasswordDto model)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(model.PasswordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != model.PasswordHash[i])
                        return false;
                }
            }
            return true;
        }

    }
}
