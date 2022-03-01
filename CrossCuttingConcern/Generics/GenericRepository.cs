using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CrossCuttingConcerns.PagingSorting;
using Microsoft.EntityFrameworkCore;

namespace CrossCuttingConcerns.Generics
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class, IEntity
    {
        private const int PageSize = 10;
        private readonly DbContext _mainDbContext;

        protected GenericRepository(DbContext mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _mainDbContext.Set<TEntity>().AsNoTracking();
        }

        public async Task<PaginatedList<TEntity>> GetList(
            int? pageNumber,
            string sortField,
            string sortOrder,
            int? pageSize)
        {
            var list = _mainDbContext
                .Set<TEntity>();

            
            return await PaginatedList<TEntity>
                .CreateAsync(list.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC"  );
            
        }


        public virtual async Task<TEntity> GetById(int id)
        {
            return await _mainDbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<TEntity> GetByIdWithTracking(int id)
        {
            return await _mainDbContext.Set<TEntity>()
                .SingleOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<TEntity> Create(TEntity entity)
        {
            await _mainDbContext.Set<TEntity>().AddAsync(entity);
            await _mainDbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task Update(TEntity entity)
        {
            try
            {
                _mainDbContext.Set<TEntity>().Update(entity);
                await _mainDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public virtual async Task UpdateBulk(List<TEntity> entity)
        {
            try
            {
                _mainDbContext.Set<TEntity>().UpdateRange(entity);
                await _mainDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public virtual async Task Delete(int id)
        {
            var entity = await GetByIdWithTracking(id).ConfigureAwait(false);
            _mainDbContext.Set<TEntity>().Remove(entity);
            await _mainDbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteBulk(List<TEntity> entity)
        {
            _mainDbContext.Set<TEntity>().RemoveRange(entity);
            await _mainDbContext.SaveChangesAsync();
        }

        public virtual async Task AddList(List<TEntity> entityList)
        {
            await _mainDbContext.AddRangeAsync(entityList);
            await _mainDbContext.SaveChangesAsync();
        }
    }
}