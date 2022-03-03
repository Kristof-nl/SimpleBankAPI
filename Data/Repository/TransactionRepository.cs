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

        

        //public async Task<PaginatedList<BankAccount>> Filter(BankAccountFilter filter, int? pageNumber, string sortField, string sortOrder,
        //   int? pageSize)
        //{
        //    IQueryable<BankAccount> query = _mainDbContext.BankAccounts
        //        .Include(a => a.Transactions);


        //    //Country


        //    //Account balance
        //    if (filter.AccountBalanceFrom != null && filter.AccountBalanceTo != null)
        //    {
        //        query = query.Distinct().Where(e => e.BankAccounts.Any(x => x.AccountBalance >= filter.AccountBalanceFrom));
        //        query = query.Distinct().Where(e => e.BankAccounts.Any(x => x.AccountBalance <= filter.AccountBalanceTo));
        //    }

        //    //Account creation time
        //    if (filter.CreationDateFrom != null && filter.CreationDateTo == null)
        //    {
        //        filter.CreationDateTo = DateTime.Now;
        //    }
        //    if (filter.CreationDateFrom == null && filter.CreationDateTo != null)
        //    {
        //        filter.CreationDateTo = DateTime.Today.AddYears(-10);
        //    }
        //    if (filter.CreationDateFrom != null && filter.CreationDateTo != null)
        //    {
        //        query = query.Distinct().Where(e => e.BankAccounts.Any(x => x.CreationDate >= filter.CreationDateFrom));
        //        query = query.Distinct().Where(e => e.BankAccounts.Any(x => x.CreationDate <= filter.CreationDateTo));
        //    }

        //    //Age between

        //    if (filter.AgeFrom != null && filter.AgeTo == null)
        //    {
        //        filter.AgeTo = 100;
        //    }
        //    if (filter.AgeFrom == null && filter.AgeTo != null)
        //    {
        //        filter.AgeFrom = 0;
        //    }
        //    if (filter.AgeFrom != null && filter.AgeTo != null)
        //    {
        //        var minusAgeTo = filter.AgeTo * -1;
        //        var minusAgeFrom = filter.AgeFrom * -1;

        //        var ageTo = DateTime.Now.AddYears((int)minusAgeTo);
        //        var ageFrom = DateTime.Now.AddYears((int)minusAgeFrom);

        //        query = query.Where(e => e.DateOfBirth >= ageTo);
        //        query = query.Where(e => e.DateOfBirth <= ageFrom);
        //    }

        //    return await PaginatedList<Customer>
        //      .CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC");
        //}
    }



}
