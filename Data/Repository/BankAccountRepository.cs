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
    public interface IBankAccountRepository
    {
        IQueryable<BankAccount> GetAll();

        //Task<PaginatedList<BankAccount>> GetSortList(
        //   int? pageNumber,
        //   string sortField,
        //   string sortOrder,
        //   int? pageSize);

        Task<BankAccount> GetById(int id);
        Task<BankAccount> Create(BankAccount entity);
        Task Update(BankAccount entity);
        Task Delete(int id);


    }

    public class BankAccountRepository : GenericRepository<BankAccount>, IBankAccountRepository
    {
        private readonly MainDbContext _mainDbContext;

        public BankAccountRepository(MainDbContext mainDbContext) : base(mainDbContext)
        {
            _mainDbContext = mainDbContext;
        }

        private const int PageSize = 10;

        public override async Task<BankAccount> GetById(int id)
        {
            return await GetAll()
                .Include(t => t.Transactions)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id)
                .ConfigureAwait(false);
        }


        //Create
        public async Task<BankAccount> Create(BankAccount entity)
        {
            //Make random account number
            Random rnd = new();
            int first = rnd.Next(0000, 9999);
            int second = rnd.Next(0000, 9999);
            int third = rnd.Next(0000, 9999);

            entity.AccountNumber = $"0012 {first} 1678 {second} 8764 {third}";

            

            //Create transaction and add it to bank account
            Transaction transaction = new()
            {
                Name = $"Account creation {DateTime.Now.ToShortTimeString()}",
                TransactionDate = DateTime.Now,
                Ammount = entity.AccountBalance,
                AmmountBefore = entity.AccountBalance,
                AammountAfter = entity.AccountBalance,
                BankAccount = entity,
            };

            await _mainDbContext.Transactions.AddAsync(transaction);

            
            
            await _mainDbContext.SaveChangesAsync();
            
            return entity;
        }


        //Update
        public override async Task<BankAccount> Update(BankAccount entity)
        {
            _mainDbContext.Update(entity);
            await _mainDbContext.SaveChangesAsync();
            return entity;
        }


        //Delete
        public override async Task Delete(int id)
        {
            var accountToDelete = await _mainDbContext.BankAccounts.FirstOrDefaultAsync(p => p.Id == id);

            _mainDbContext.BankAccounts.Remove(accountToDelete);
            await _mainDbContext.SaveChangesAsync();
        }

        

        //public async Task<PaginatedList<BankAccount>> GetSortList(
        //  int? pageNumber,
        //  string sortField,
        //  string sortOrder,
        //  int? pageSize)
        //{
        //    IQueryable<BankAccount> query = _mainDbContext.BankAccounts
        //        .Include(p => p.Transactions)
        //        .Include(p => p.Customer)
        //        .Include(x => x.Bank);

        //    return await PaginatedList<BankAccount>
        //       .CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC");
        //}
    }
}
