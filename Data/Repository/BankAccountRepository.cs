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
        Task<Transaction> BankTransfer(BankAccount bankAccountFrom, string accountNumber, double amount);

        Task<PaginatedList<BankAccount>> Filter(BankAccountFilter filter, int? pageNumber, string sortField, string sortOrder,
           int? pageSize);


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
            string first = rnd.Next(0000, 9999).ToString();
            string second = rnd.Next(0000, 9999).ToString();
            string third = rnd.Next(0000, 9999).ToString();

            entity.AccountNumber = $"0012 {first} 1678 {second} 8764 {third}";

            

            //Create transaction and add it to bank account
            Transaction transaction = new()
            {
                Name = "Account creation",
                TransactionDate = DateTime.Now,
                Ammount = entity.AccountBalance,
                AmmountBefore = 0,
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

        public async Task<Transaction> BankTransfer(BankAccount bankAccountFrom, string accountNumber, double amount)
        {
            var bankAccountTo = await _mainDbContext.BankAccounts.Include(t => t.Transactions).AsNoTracking().FirstOrDefaultAsync(p => p.AccountNumber == accountNumber);
            bankAccountTo.AccountBalance += amount;
            bankAccountFrom.AccountBalance -= amount;

            //bankAccountTo.Transactions.Add(new()
            //{
            //    Name = $"Transfer to bank account {bankAccountTo.AccountNumber}",
            //    TransactionDate = DateTime.Now,
            //    Ammount = amount,
            //    From = bankAccountFrom.AccountNumber,
            //    To = bankAccountTo.AccountNumber,
            //    AammountAfter = bankAccountFrom.AccountBalance - amount,
            //    AmmountBefore = bankAccountFrom.AccountBalance,
            //});

            Transaction transactionFrom = new()
            {
                Name = $"Transfer to bank account {bankAccountTo.AccountNumber}",
                TransactionDate = DateTime.Now,
                Ammount = amount,
                From = bankAccountFrom.AccountNumber,
                To = bankAccountTo.AccountNumber,
                AammountAfter = bankAccountFrom.AccountBalance - amount,
                AmmountBefore = bankAccountFrom.AccountBalance,
                //BankAccount = bankAccountFrom

            };

            Transaction transactionTo = new()
            {
                Name = $"Transfer from bank account {bankAccountFrom.AccountNumber}",
                Ammount = amount,
                TransactionDate = DateTime.Now,
                From = bankAccountFrom.AccountNumber,
                To = bankAccountTo.AccountNumber,
                AammountAfter = bankAccountTo.AccountBalance += amount,
                AmmountBefore = bankAccountTo.AccountBalance,
                //BankAccount = bankAccountTo

            };
            

            _mainDbContext.Add(transactionFrom);
            _mainDbContext.Add(transactionTo);

            bankAccountTo.Transactions.Add(transactionFrom);
            bankAccountTo.Transactions.Add(transactionTo);

            await _mainDbContext.SaveChangesAsync();

            return transactionFrom;
        }
    }
}
