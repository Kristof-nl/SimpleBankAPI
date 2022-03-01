using Logic.DataTransferObjects.BankAccount;
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
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;
        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [AllowAnonymous]
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var bankAccount = await _bankAccountService
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
                    await _bankAccountService
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
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateBankAccountDto createBankAccountDto)
        {
            try
            {
                var newBankAccount =
                    await _bankAccountService
                        .Create(createBankAccountDto)
                        .ConfigureAwait(false);

                return Ok(newBankAccount);

            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] BankAccountDto updateBankAccountDto)
        {
            try
            {
                var bankToUpdate = await _bankAccountService.Update(updateBankAccountDto).ConfigureAwait(true);
                if (bankToUpdate != null)
                {
                    return Ok(bankToUpdate);
                }
                return BadRequest("Bank account doesn't exist in the database.");

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
                var bank = await _bankAccountService.GetById(id).ConfigureAwait(false);

                if (bank == null)
                {
                    return BadRequest("Bank account doesn't exist in the database.");
                }
                await _bankAccountService.Delete(id).ConfigureAwait(true);
                return Ok("Bank account has been deleted");
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        

        //[AllowAnonymous]
        //[HttpGet("GetPagedList")]
        //public async Task<ActionResult<PaginatedList<ShortBankAccountDto>>> Get(
        //    int? pageNumber, string sortField, string sortOrder,
        //    int? pageSize)
        //{
        //    var list = await _bankAccountService.GetPagedList(pageNumber, sortField, sortOrder, pageSize);
        //    return list;
        //}
    }
}

