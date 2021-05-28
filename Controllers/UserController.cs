using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testeef.Data;
using testeef.Models;

namespace testeef.Controllers
{
    [ApiController]
    [Route("v1/users")]

    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]

        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var Users = await context.Users.ToListAsync();
            return Users;
        }

        [HttpPost]
        [Route("")]

        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            if (ModelState.IsValid)
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}