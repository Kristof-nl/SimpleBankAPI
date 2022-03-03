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

        [AllowAnonymous]
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var bankAccount = await _transactionService
                    .GetById(id)
                    .ConfigureAwait(false);

                return Ok(bankAccount);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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


        [AllowAnonymous]
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


        [AllowAnonymous]
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



        [AllowAnonymous]
        [HttpPost("Transfer")]
        public async Task<ActionResult> BankTransfer(int accountId, string accountNumber, double amount)
        {
            try
            {
                var newTransaction = await _transactionService.BankTransfer(accountId, accountNumber, amount);
                return Ok(newTransaction);
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

