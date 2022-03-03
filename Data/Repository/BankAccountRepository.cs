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
    public interface IBankAccountRepository
    {
        IQueryable<BankAccount> GetAll();

        Task<PaginatedList<BankAccount>> GetSortList(
           int? pageNumber,
           string sortField,
           string sortOrder,
           int? pageSize);

        Task<BankAccount> GetById(int id);
        Task<BankAccount> Create(BankAccount entity);
        Task Update(BankAccount entity);
        Task Delete(int id);
        Task BankTransfer(BankAccount bankAccountFrom, string accountNumber, double amount);

        Task<PaginatedList<BankAccount>> Filter(BankAccountFilter filter, int? pageNumber, string sortField, string sortOrder,
           int? pageSize);

        Task Withdraw(BankAccount bankAccountFrom, double amount);


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
            int first = rnd.Next(1000, 9999);
            int second = rnd.Next(1000, 9999);
            int third = rnd.Next(1000, 9999);

            entity.AccountNumber = $"0012 {first} 0078 {second} 0000 {third}";

            

            //Create transaction and add it to bank account
            Transaction transaction = new()
            {
                Name = "Account creation",
                TransactionDate = DateTime.Now,
                TransactionAmount = entity.AccountBalance,
                AmountBefore = 0,
                AmountAfter = entity.AccountBalance,
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
            var accountToDelete = await _mainDbContext.BankAccounts.Include(t => t.Transactions).FirstOrDefaultAsync(p => p.Id == id);

            _mainDbContext.BankAccounts.Remove(accountToDelete);
            await _mainDbContext.SaveChangesAsync();
        }



        public async Task<PaginatedList<BankAccount>> GetSortList(
          int? pageNumber,
          string sortField,
          string sortOrder,
          int? pageSize)
        {
            IQueryable<BankAccount> query = _mainDbContext.BankAccounts
                .Include(p => p.Transactions);
               
            return await PaginatedList<BankAccount>
               .CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC");
        }


        public async Task<PaginatedList<BankAccount>> Filter(BankAccountFilter filter, int? pageNumber, string sortField, string sortOrder,
           int? pageSize)
        {
            IQueryable<BankAccount> query = _mainDbContext.BankAccounts
                .Include(a => a.Transactions);

            //Account balance
            if (filter.AccountBalanceFrom != null && filter.AccountBalanceTo == null)
            {
                filter.AccountBalanceTo = 10000000000000000000;
            }
            if (filter.AccountBalanceFrom == null && filter.AccountBalanceTo != null)
            {
                filter.AccountBalanceFrom = -1000000000000000000;
            }
            if (filter.AccountBalanceFrom != null && filter.AccountBalanceTo != null)
            {
                query = query.Distinct().Where(e => e.AccountBalance >= filter.AccountBalanceFrom);
                query = query.Distinct().Where(e => e.AccountBalance <= filter.AccountBalanceTo);
            }

            //Account creation time
            if (filter.CreationDateFrom != null && filter.CreationDateTo == null)
            {
                filter.CreationDateTo = DateTime.Now;
            }
            if (filter.CreationDateFrom == null && filter.CreationDateTo != null)
            {
                filter.CreationDateTo = new DateTime(2000, 01, 01);
            }
            if (filter.CreationDateFrom != null && filter.CreationDateTo != null)
            {
                query = query.Distinct().Where(e => e.CreationDate >= filter.CreationDateFrom);
                query = query.Distinct().Where(e => e.CreationDate <= filter.CreationDateTo);
            }


            return await PaginatedList<BankAccount>
              .CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize ?? PageSize, sortField ?? "Id", sortOrder ?? "ASC");

        }

        public async Task BankTransfer(BankAccount bankAccountFrom, string accountNumber, double amount)
        {
            var bankAccountTo = await _mainDbContext.BankAccounts.Include(t => t.Transactions).AsNoTracking().FirstOrDefaultAsync(p => p.AccountNumber == accountNumber);
            
            Transaction transactionFrom = new()
            {
                Name = $"Transfer to bank account {bankAccountTo.AccountNumber}",
                TransactionDate = DateTime.Now,
                TransactionAmount = amount,
                From = bankAccountFrom.AccountNumber,
                To = bankAccountTo.AccountNumber,
                AmountAfter = bankAccountFrom.AccountBalance - amount,
                AmountBefore = bankAccountFrom.AccountBalance,
            };

            Transaction transactionTo = new()
            {
                Name = $"Transfer from bank account {bankAccountFrom.AccountNumber}",
                TransactionAmount = amount,
                TransactionDate = DateTime.Now,
                From = bankAccountFrom.AccountNumber,
                To = bankAccountTo.AccountNumber,
                AmountAfter = bankAccountTo.AccountBalance += amount,
                AmountBefore = bankAccountTo.AccountBalance,

            };
            
            bankAccountTo.AccountBalance += amount;
            bankAccountFrom.AccountBalance -= amount;

            await _mainDbContext.Transactions.AddAsync(transactionFrom);
            await _mainDbContext.Transactions.AddAsync(transactionTo);
 
            transactionFrom.BankAccount = bankAccountFrom;
            transactionTo.BankAccount = bankAccountTo;

            await _mainDbContext.SaveChangesAsync();

        }


        public async Task Withdraw(BankAccount bankAccount, double amount)
        {
            Random rnd = new();
            int atmNumber = rnd.Next(0000, 9999);

            Transaction transaction = new()
            {
                Name = $"Withdraw from ATM {atmNumber}",
                TransactionDate = DateTime.Now,
                TransactionAmount = amount,
                From = bankAccount.AccountNumber,
                To = $"ATM {atmNumber}",
                AmountAfter = bankAccount.AccountBalance - amount,
                AmountBefore = bankAccount.AccountBalance,
            };

            bankAccount.AccountBalance -= amount;

            await _mainDbContext.Transactions.AddAsync(transaction);


            transaction.BankAccount = bankAccount;
            

            await _mainDbContext.SaveChangesAsync();

        }
    }
}
