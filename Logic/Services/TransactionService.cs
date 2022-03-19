using AutoMapper;
using CrossCuttingConcern.Filters;
using CrossCuttingConcerns.PagingSorting;
using Data.DataObjects;
using Data.Repository;
using Logic.DataTransferObjects.Transaction;
using Logic.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
        Task<PaginatedList<TransactionDto>> Filter(TransactionFilter filterDto, int? pageNumber, string sortField, string sortOrder,
          int? pageSize);
    }


    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IMapper mapper)


        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<TransactionDto> GetById(int bankAccountId)
        {
            var bankAccountFromDb = await _transactionRepository.GetById(bankAccountId).ConfigureAwait(false);


            if (bankAccountFromDb == null)
            {
                throw new NotFoundException("Bank Account not found");
            }

            return _mapper.Map<TransactionDto>(bankAccountFromDb);
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
                    TransactionAmount = ua.TransactionAmount,
                    AmountBefore = ua.AmountBefore,
                    AmountAfter = ua.AmountAfter,
                    From = ua.From,
                    To = ua.To

                }).ToList()
            };
        }

        public async Task<PaginatedList<TransactionDto>> Filter(TransactionFilter filterDto, int? pageNumber, string sortField, string sortOrder,
          int? pageSize)
        {
            PaginatedList<Transaction> result =
                await _transactionRepository.Filter(filterDto, pageNumber, sortField, sortOrder, pageSize);
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
                    TransactionAmount = ua.TransactionAmount,
                    AmountBefore = ua.AmountBefore,
                    AmountAfter = ua.AmountAfter,
                    From = ua.From,
                    To = ua.To

                }).ToList()
            };
        }


    }

}
