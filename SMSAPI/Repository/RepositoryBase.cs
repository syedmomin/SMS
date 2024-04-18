// -----------------------------------------------------------------------
// <copyright file="RepositoryBase.cs" company="Playtertainment">
// Copyright (c) Playtertainment. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace WinnerWinner.Services.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SMS_API;

    /// <inheritdoc/>
    public abstract class RepositoryBase<T> : IRepositoryBase<T>
        where T : class
    {
        protected RepositoryBase(ApplicationDbContext repositoryContext) => this.RepositoryContext = repositoryContext;
        protected ApplicationDbContext RepositoryContext { get; set; }

        public T Find(int id)
        {
            return this.RepositoryContext.Set<T>().Find(id);
        }

        public IQueryable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool suppressEntityTracking = true)
        {
            return suppressEntityTracking
                ? this.RepositoryContext.Set<T>().Where(expression).AsNoTracking()
                : this.RepositoryContext.Set<T>().Where(expression);
        }

        public T Create(T entity)
        {
            var entry = this.RepositoryContext.Set<T>().Add(entity);
            this.RepositoryContext.SaveChanges();
            return entity;
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
            this.RepositoryContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
            this.RepositoryContext.SaveChanges();
        }

        public bool Exist(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Any(expression);
        }

        public int Save()
        {
            return this.RepositoryContext.SaveChanges();
        }

        public async Task<T> FindAsync(int id)
        {
            return await this.RepositoryContext.Set<T>().FindAsync(id);
        }

        public async Task<T> CreateAsync(T entity)
        {
            var entry = this.RepositoryContext.Set<T>().Add(entity);
            await this.RepositoryContext.SaveChangesAsync();
            return entity;
        }

        public async Task<T> NewCreateAsync(T entity)
        {
            var entry = this.RepositoryContext.Set<T>().Add(entity);
            await this.RepositoryContext.SaveChangesAsync();
            return entity;
        }

        public async Task CreateRangeAsync(IEnumerable<T> entities)
        {
            this.RepositoryContext.Set<T>().AddRange(entities);
            await this.RepositoryContext.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            this.RepositoryContext.Set<T>().UpdateRange(entities);
            await this.RepositoryContext.SaveChangesAsync();
        }

        public async Task UpdateManyToManyRelations(IEnumerable<T> currentItems, IEnumerable<T> newItems)
        {
            currentItems.ToList().ForEach(x =>
            {
                this.RepositoryContext.Entry(x).State = EntityState.Deleted;
            });

            newItems.ToList().ForEach(x =>
            {
                this.RepositoryContext.Entry(x).State = EntityState.Added;
            });

            await this.RepositoryContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            this.RepositoryContext.Entry(entity).State = EntityState.Modified;
            await this.RepositoryContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
            await this.RepositoryContext.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> expression)
        {
            return await this.RepositoryContext.Set<T>().AnyAsync(expression);
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
        {
            return await this.RepositoryContext.SaveChangesAsync();
        }
    }
    public interface IRepositoryBase<T>
    {
        T Find(int id);
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool suppressEntityTracking = true);
        T Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        bool Exist(Expression<Func<T, bool>> expression);
        int Save();
        Task<T> FindAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<T> NewCreateAsync(T entity);
        Task CreateRangeAsync(IEnumerable<T> entities);

        Task UpdateRangeAsync(IEnumerable<T> entities);

        Task UpdateManyToManyRelations(IEnumerable<T> currentItems, IEnumerable<T> newItems);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
        Task<int> SaveChangesAsync();
        Task<bool> ExistAsync(Expression<Func<T, bool>> expression);
    }
}
