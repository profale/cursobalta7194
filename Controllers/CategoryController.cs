using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        //cachear metodo
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")] //restricao na rota
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post([FromBody] Category category, [FromServices] DataContext context)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                context.Categories.Add(category);    
                //salvando de forma assincrona
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (System.Exception)
            {
                return BadRequest(new {message = "Não foi possível criar a categoria!"});
            }
            
            
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(int id, [FromBody] Category category
        , [FromServices] DataContext context)
        {
            if(id != category.Id)
                return NotFound(new {message = "Categoria não encontrada."});
            
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                context.Entry<Category>(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new{message = "Este registro já foi atualizado."});
            }
            catch (Exception)
            {
                return BadRequest(new{message = "Não foi possível atualizar a categoria."});
            }
            
            
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Delete(int id, [FromServices] DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
                return NotFound(new {message = "Categoria não encontrada"});

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new {message = "Categoria removida com sucesso."});
            }
            catch (System.Exception)
            {
                return BadRequest(new {messsage = "Não foi possível remover a categoria."});
            }
            
        }
    }
}