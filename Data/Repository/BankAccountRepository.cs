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
    }
}
