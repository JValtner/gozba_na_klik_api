using Gozba_na_klik.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gozba_na_klik.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        //private readonly UsersDbRepository _usersRepository;
        private readonly IUserService _userService;

        public MealsController(IMealService mealService)
        {
            _mealService = mealService;
        }
    }
}