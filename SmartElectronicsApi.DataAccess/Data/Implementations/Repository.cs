using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartElectronicsApi.Core.Entities.Common;
using SmartElectronicsApi.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.DataAccess.Data.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly SmartElectronicsDbContext _context;
        private readonly DbSet<T> _table;
        public Repository(SmartElectronicsDbContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

    

        public async Task Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task Create(T entity)
        {

            try
            {
                var result = _context.Entry(entity);
                result.State = EntityState.Added;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task Delete(T entity)
        {

            try
            {
                var result = _context.Entry(entity);
                result.State = EntityState.Deleted;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<List<T>> GetAll(
      Expression<Func<T, bool>> predicate = null,
      params Func<IQueryable<T>, IQueryable<T>>[] includes)
        {
            try
            {
                IQueryable<T> query = _table;

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = include(query);
                    }
                }

                return predicate == null ? await query.ToListAsync() : await query.Where(predicate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<T> GetEntity(
            Expression<Func<T, bool>> predicate = null,
            params Func<IQueryable<T>, IQueryable<T>>[] includes)
        {
            try
            {
                IQueryable<T> query = _table;

                if (includes != null)
                {
                    foreach (var include in includes)
                    {
                        query = include(query);
                    }
                }

                return predicate == null ? await query.FirstOrDefaultAsync() : await query.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<bool> isExists(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                return predicate is null ? false : await _table.AnyAsync(predicate);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task Update(T entity)
        {
            try
            {
                var result = _context.Entry(entity);
                result.State = EntityState.Modified;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
