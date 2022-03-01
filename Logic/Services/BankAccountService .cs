using AutoMapper;
using CrossCuttingConcerns.PagingSorting;
using Data.DataObjects;
using Data.Repository;
using Logic.DataTransferObjects.BankAccount;
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
        Task<List<BankAccountDto>> GetAll();
        Task<BankAccountDto> Create(CreateBankAccountDto createUpdateBankDto);
        Task<BankAccountDto> Update(BankAccountDto updateBankAccountDto);
        Task Delete(int id);
        //Task<PaginatedList<ShortBankAccountDto>> GetPagedList(
        //int? pageNumber, string sortField, string sortOrder,
        //int? pageSize);
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

        public Task<List<BankAccountDto>> GetAll()
        {
            throw new NotImplementedException();
        }


        //  public async Task<PaginatedList<ShortBankAccountDto>> GetPagedList(
        //int? pageNumber, string sortField, string sortOrder,
        //int? pageSize)
        //  {
        //      PaginatedList<BankAccount> result =
        //          await _bankAccountRepository.GetSortList(pageNumber, sortField, sortOrder, pageSize);
        //      return new PaginatedList<ShortBankAccountDto>
        //      {
        //          CurrentPage = result.CurrentPage,
        //          From = result.From,
        //          PageSize = result.PageSize,
        //          To = result.To,
        //          TotalCount = result.TotalCount,
        //          TotalPages = result.TotalPages,
        //          Items = result.Items.Select(ua => new ShortBankAccountDto
        //          {
        //              Id = ua.Id,
        //              Type = ua.Type,
        //              AccountBalance = ua.AccountBalance,
        //              AccountNumber = ua.AccountNumber,
        //              Customer = _mapper.Map<ShortCustomerDto>(ua.Customer),
        //              Bank = _mapper.Map<ShortBankDto>(ua.Bank),
        //          }).ToList()
        //      };
        //  }
    }

}
