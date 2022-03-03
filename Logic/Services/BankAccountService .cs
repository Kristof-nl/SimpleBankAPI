using AutoMapper;
using CrossCuttingConcern.Filters;
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
    public interface IBankAccountService
    {
        Task<BankAccountDto> GetById(int bankId);
        Task<List<ShortBankAccountDto>> GetAll();
        Task<BankAccountDto> Create(CreateBankAccountDto createUpdateBankDto);
        Task<BankAccountDto> Update(BankAccountDto updateBankAccountDto);
        Task Delete(int id);
        Task BankTransfer(int accountId, string accountNumber, double amount);
        Task<PaginatedList<ShortBankAccountDto>> GetPagedList(
        int? pageNumber, string sortField, string sortOrder,
        int? pageSize);

        Task<PaginatedList<ShortBankAccountDto>> Filter(BankAccountFilter profileFilterDto, int? pageNumber, string sortField, string sortOrder,
          int? pageSize);
    }


    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IMapper _mapper;

        public BankAccountService(
            IBankAccountRepository bankAccountRepository,
            IMapper mapper)

        {
            _bankAccountRepository = bankAccountRepository;
            _mapper = mapper;
        }

        public async Task<BankAccountDto> GetById(int bankAccountId)
        {
            var bankAccountFromDb = await _bankAccountRepository.GetById(bankAccountId).ConfigureAwait(false);


            if (bankAccountFromDb == null)
            {
                throw new NotFoundException("Bank Account not found");
            }

            return _mapper.Map<BankAccount, BankAccountDto>(bankAccountFromDb);
        }

        //Create
        public async Task<BankAccountDto> Create(CreateBankAccountDto createBankAccountDto)
        {
            var newBankAccount = _mapper.Map<BankAccount>(createBankAccountDto);
            await _bankAccountRepository.Create(newBankAccount);

            return _mapper.Map<BankAccount, BankAccountDto>(newBankAccount);
        }


        //Update
        public async Task<BankAccountDto> Update(BankAccountDto updateBankAccountDto)
        {
            var bankAccountToUpdate = _mapper.Map<BankAccount>(updateBankAccountDto);

            var checkBankAccountInDataBase = await _bankAccountRepository.GetById(bankAccountToUpdate.Id).ConfigureAwait(false);
            if (checkBankAccountInDataBase != null)
            {
                await _bankAccountRepository.Update(bankAccountToUpdate);
                return _mapper.Map<BankAccount, BankAccountDto>(bankAccountToUpdate);
            }

            return null;
        }

        public async Task Delete(int id)
        {
            await _bankAccountRepository.Delete(id);
        }

        public async Task<List<ShortBankAccountDto>> GetAll()
        {

            var allBanksFromDb = await _bankAccountRepository.GetAll().ToListAsync().ConfigureAwait(false);

            return _mapper.Map<List<BankAccount>, List<ShortBankAccountDto>>(allBanksFromDb);
        }

        public async Task BankTransfer(int accountId, string accountNumber, double amount)
        {
            var bankAccountFromDb = await _bankAccountRepository.GetById(accountId).ConfigureAwait(false);

            if (bankAccountFromDb == null)
            {
                throw new NotFoundException("Bank account not found");
            }

            await _bankAccountRepository.BankTransfer(bankAccountFromDb, accountNumber, amount);
        }



        public async Task<PaginatedList<ShortBankAccountDto>> GetPagedList(
      int? pageNumber, string sortField, string sortOrder,
      int? pageSize)
        {
            PaginatedList<BankAccount> result =
                await _bankAccountRepository.GetSortList(pageNumber, sortField, sortOrder, pageSize);
            return new PaginatedList<ShortBankAccountDto>
            {
                CurrentPage = result.CurrentPage,
                From = result.From,
                PageSize = result.PageSize,
                To = result.To,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Items = result.Items.Select(ua => new ShortBankAccountDto
                {
                    Id = ua.Id,
                    CreationDate = ua.CreationDate,
                    AccountNumber = ua.AccountNumber,
                    AccountBalance = ua.AccountBalance,

                }).ToList()
            };
        }

        public async Task<PaginatedList<ShortBankAccountDto>> Filter(BankAccountFilter profileFilterDto, int? pageNumber, string sortField, string sortOrder,
          int? pageSize)
        {
            PaginatedList<BankAccount> result =
                await _bankAccountRepository.Filter(profileFilterDto, pageNumber, sortField, sortOrder, pageSize);
            return new PaginatedList<ShortBankAccountDto>
            {
                CurrentPage = result.CurrentPage,
                From = result.From,
                PageSize = result.PageSize,
                To = result.To,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
                Items = result.Items.Select(ua => new ShortBankAccountDto
                {
                    Id = ua.Id,
                    AccountNumber = ua.AccountNumber,
                    CreationDate = ua.CreationDate,
                    AccountBalance = ua.AccountBalance,
                  
                }).ToList()
            };
        }
    }

}
