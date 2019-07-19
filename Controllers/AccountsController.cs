using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NG_Core_Auth.Helpers;
using NG_Core_Auth.Models.DAL;
using NG_Core_Auth.Models.Entities;
using NG_Core_Auth.ViewModels;

namespace NG_Core_Auth.Controllers {
    
    [Route ("api/[controller]")]
    public class AccountsController : Controller {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SecurityDbContext _context;

        public AccountsController (UserManager<AppUser> userManager, IMapper mapper, SecurityDbContext context) {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] RegistrationViewModel model) {
            if (!ModelState.IsValid) {
                return BadRequest (ModelState);
            }

            var userIdentity = _mapper.Map<AppUser> (model);

            var result = await _userManager.CreateAsync (userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult (Errors.AddErrorsToModelState (result, ModelState));

            await _context.Customers.AddAsync (new Customer {  user_id = userIdentity.Id, Location = model.Location });
            await _context.SaveChangesAsync ();

            return new OkObjectResult ("Account created");
        }
    }
}