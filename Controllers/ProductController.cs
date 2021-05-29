using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testeef.Data;
using testeef.Models;

namespace testeef.Controllers
{
    [ApiController]
    [Route("v1/products")]

    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("users/{id}")]

        public async Task<ActionResult<List<Product>>> GetByIdUser([FromServices] DataContext context, string id)
        {
            var products = await context.Products
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.UserId_email == id)
                .ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model)
        {
            if (ModelState.IsValid)
            {
                Random randNum = new Random();

                var Product = new Product
                {
                    Title = model.Title,
                    Number_card = randNum.Next(),
                    UserId_email = model.UserId_email,
                };
                
                context.Products.Add(Product);
                await context.SaveChangesAsync();
                return Product;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}