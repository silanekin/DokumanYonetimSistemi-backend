using AutoMapper;
using DokumanYonetimSistemi.DomainModels;
using DokumanYonetimSistemi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DokumanYonetimSistemi.Controllers
{
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserController(IUserRepository userRepository , IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        [Route("[controller]")]

        public async Task<IActionResult> GetAllUsersAsync()
        {
           var users = await userRepository.GetUsersAsync();

            return Ok(mapper.Map<List<User>>(users));
        }
    }
}
