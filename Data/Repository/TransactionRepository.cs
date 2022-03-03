using CrossCuttingConcern.Filters;
using CrossCuttingConcerns.Generics;
using CrossCuttingConcerns.PagingSorting;
using Data.DataObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface ITransactionRepository
    {
        IQueryable<Transaction> GetAll();

        Task<PaginatedList<Transaction>> GetSortList(
           int? pageNumber,
           string sortField,
           string sortOrder,
           int? pageSize);

        Task<Transaction> GetById(int id);
        Task Update(Transaction entity);
        Task Delete(int id);
        Task<PaginatedList<Transaction>> Filter(TransactionFilter filter, int? pageNumber, string sortField, string sortOrder,
           int? pageSize);

    }

    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly MainDbContext _mainDbContext;

        public TransactionRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }

        private const int PageSize = 10;

        public override async Task<Transaction> GetById(int id)
        {
            return await GetAll()
                .Include(b => b.BankAccount)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)
                .ConfigureAwait(false);
        }


        //Update
        public override async Task<Transaction> Update(Transaction entity)
        {
            _mainDbContext.Update(entity);
            await _mainDbContext.SaveChangesAsync();
            return entity;
        }


        //Delete
        public override async Task Delete(int id)
        {
            var accountToDelete = await _mainDbContext.Transactions.Include(t => t.BankAccount).FirstOrDefaultAsync(p => p.Id == id);

            _mainDbContext.Transactions.Remove(accountToDelete);
            await _mainDbContext.SaveChangesAsync();
        }



        public async Task<PaginatedList<Transaction>> GetSortList(
          int? pageNumber,
          string sortField,
          string sortOrder,
          int? pageSize)
        {
            IQueryable<Transaction> query = _mainDbContext.Transactions
                .Include(b => b.BankAccount);

            return await PaginatedList<Transaction>
               .CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC");
        }



        public async Task<PaginatedList<Transaction>> Filter(TransactionFilter filter, int? pageNumber, string sortField, string sortOrder,
           int? pageSize)
        {
            IQueryable<Transaction> query = _mainDbContext.Transactions
                .Include(a => a.BankAccount);

            //Transaction amount
            if (filter.AmountFrom!= null && filter.AmountTo == null)
            {
                filter.AmountTo = 10000000000000000000;
            }
            if (filter.AmountFrom == null && filter.AmountTo != null)
            {
                filter.AmountFrom = -1000000000000000000;
            }
            if (filter.AmountFrom != null && filter.AmountTo != null)
            {
                query = query.Distinct().Where(e => e.TransactionAmount >= filter.AmountFrom);
                query = query.Distinct().Where(e => e.TransactionAmount <= filter.AmountTo);
            }

            //Transaction time
            if (filter.TransactionDateFrom != null && filter.TransactionDateTo == null)
            {
                filter.TransactionDateTo = DateTime.Now;
            }
            if (filter.TransactionDateFrom == null && filter.TransactionDateTo != null)
            {
                filter.TransactionDateFrom = new DateTime(2000, 01, 01);
            }
            if (filter.TransactionDateFrom != null && filter.TransactionDateTo != null)
            {
                query = query.Distinct().Where(e => e.TransactionDate >= filter.TransactionDateFrom);
                query = query.Distinct().Where(e => e.TransactionDate <= filter.TransactionDateTo);
            }

            //Transaction type
            if (filter.Type != null)
            {
                query = query.Distinct().Where(e => e.Name.Contains(filter.Type));
                
            }
           

            return await PaginatedList<Transaction>
              .CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC");

        }

    }

}
