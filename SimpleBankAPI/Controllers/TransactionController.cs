using CrossCuttingConcern.Filters;
using CrossCuttingConcerns.PagingSorting;
using Logic.DataTransferObjects.BankAccount;
using Logic.DataTransferObjects.Transaction;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace SimpleBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize(Roles = "User, Administrator")]
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var transaction = await _transactionService
                    .GetById(id)
                    .ConfigureAwait(false);

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var bankAccounts =
                    await _transactionService
                    .GetAll()
                    .ConfigureAwait(false);

                return Ok(bankAccounts);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] TransactionDto updateBankAccountDto)
        {
            try
            {
                var bankToUpdate = await _transactionService.Update(updateBankAccountDto).ConfigureAwait(true);
                if (bankToUpdate != null)
                {
                    return Ok(bankToUpdate);
                }
                return BadRequest("Transaction doesn't exist in the database.");

            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [Authorize(Roles = "Administrator")]
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var bank = await _transactionService.GetById(id).ConfigureAwait(false);

                if (bank == null)
                {
                    return BadRequest("Bank account doesn't exist in the database.");
                }
                await _transactionService.Delete(id).ConfigureAwait(true);
                return Ok("Transaction has been deleted");
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        
        [HttpGet("GetPagedList")]
        public async Task<ActionResult<PaginatedList<TransactionDto>>> Get(
            int? pageNumber, string sortField, string sortOrder,
            int? pageSize)
        {
            try
            {
                var list = await _transactionService.GetPagedList(pageNumber, sortField, sortOrder, pageSize);
                return list;
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        
        [HttpPost("Filter")]
        public async Task<ActionResult<PaginatedList<TransactionDto>>> Filter([FromBody] TransactionFilter filterDto, int? pageNumber, string sortField, string sortOrder,
            int? pageSize)
        {
            try
            {
                var query = await _transactionService.Filter(filterDto, pageNumber, sortField, sortOrder, pageSize).ConfigureAwait(false);

                if (query.Items.Count < 1)
                {
                    return BadRequest("Any results in the database");
                }
                return query;

            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

