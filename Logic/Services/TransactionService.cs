using AutoMapper;
using CrossCuttingConcerns.PagingSorting;
using Data.DataObjects;
using Data.Repository;
using Logic.DataTransferObjects.BankAccount;
using Logic.DataTransferObjects.Transaction;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto> GetById(int bankId);
        Task<List<TransactionDto>> GetAll();
        Task<TransactionDto> Update(TransactionDto updateBankAccountDto);
        Task Delete(int id);

        Task<PaginatedList<TransactionDto>> GetPagedList(
        int? pageNumber, string sortField, string sortOrder,
        int? pageSize);


        Task<TransactionDto> BankTransfer(int accountId, string accountNumber, double amount);
    }


    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IMapper _mapper;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IMapper mapper,
            IBankAccountRepository bankAccountRepository)

        {
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
            _mapper = mapper;
        }

        public async Task<TransactionDto> GetById(int bankAccountId)
        {
            var bankAccountFromDb = await _transactionRepository.GetById(bankAccountId).ConfigureAwait(false);


            if (bankAccountFromDb == null)
            {
                throw new NotFoundException("Bank Account not found");
            }

            return _mapper.Map<Transaction, TransactionDto>(bankAccountFromDb);
        }


        //Update
        public async Task<TransactionDto> Update(TransactionDto updateBankAccountDto)
        {
            var bankAccountToUpdate = _mapper.Map<Transaction>(updateBankAccountDto);

            var checkBankAccountInDataBase = await _transactionRepository.GetById(bankAccountToUpdate.Id).ConfigureAwait(false);
            if (checkBankAccountInDataBase != null)
            {
                await _transactionRepository.Update(bankAccountToUpdate);
                return _mapper.Map<Transaction, TransactionDto>(bankAccountToUpdate);
            }

            return null;
        }

        public async Task Delete(int id)
        {
            await _transactionRepository.Delete(id);
        }

        public async Task<List<TransactionDto>> GetAll()
        {

            var allBanksFromDb = await _transactionRepository.GetAll().ToListAsync().ConfigureAwait(false);

            return _mapper.Map<List<Transaction>, List<TransactionDto>>(allBanksFromDb);
        }


        public async Task<PaginatedList<TransactionDto>> GetPagedList(
      int? pageNumber, string sortField, string sortOrder,
      int? pageSize)
        {
            PaginatedList<Transaction> result =
                await _transactionRepository.GetSortList(pageNumber, sortField, sortOrder, pageSize);
            return new PaginatedList<TransactionDto>
            {
                CurrentPage = result.CurrentPage,
                From = result.From,
                PageSize = result.PageSize,
                To = result.To,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Items = result.Items.Select(ua => new TransactionDto
                {
                    Id = ua.Id,
                    Name = ua.Name,
                    TransactionDate = ua.TransactionDate,
                    Ammount = ua.Ammount,
                    AmountBefore = ua.AmmountBefore,
                    AmountAfter = ua.AammountAfter,
                    From = ua.From,
                    To = ua.To

                }).ToList()
            };
        }


        public async Task<TransactionDto> BankTransfer(int accountId, string accountNumber, double amount)
        {
            var bankAccountFromDb = await _bankAccountRepository.GetById(accountId).ConfigureAwait(false);

            if (bankAccountFromDb == null)
            {
                throw new NotFoundException("Bank account not found");
            }

            var newTransaction = _transactionRepository.BankTransfer(bankAccountFromDb, accountNumber, amount);

            return _mapper.Map<TransactionDto>(newTransaction);
        }
    }

}
