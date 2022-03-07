using AutoMapper;
using Data.DataObjects;
using Logic.DataTransferObjects.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IMapper _mapper;
        //private readonly IAuthManager _authManager;

        public AccountController(UserManager<ApiUser> userManager, IMapper mapper
            /*IAuthManager authManager*/)
        {
            _userManager = userManager;
            _mapper = mapper;
            //_authManager = authManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDto.Roles);
                return Accepted();
            }
            catch (Exception ex)
            {
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }

        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        //{


        //    try
        //    {
        //        //if (!await _authManager.ValidateUser(userDto))
        //        //{
        //        //    return Unauthorized();
        //        //}

        //        //return Accepted(new { Token = await _authManager.CreateToken() });

        //        var result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, false, false);

        //        if(!result.Succeeded)
        //        {
        //            return Unauthorized(userDto);
        //        }

        //        return Accepted(userDto);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
        //    }
        //}

    }
}