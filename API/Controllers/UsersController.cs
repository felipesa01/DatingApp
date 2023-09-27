using System.Reflection.Metadata.Ecma335;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<IEnumerable<MemberDto>> GetUsers()
        {
            return await _userRepository.GetMembersAsync();

        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);

        }

        [HttpGet("nomedafuncao")]
        public async Task<Dictionary<string, string>> GetUsers2(string par1, string par2)
        {
            return new Dictionary<string, string>
            {
                {"teste", par1},
                {"teste2", par2}
            };

        }

    }
}