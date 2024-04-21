using Infrastructure.Contexts;
using Infrastructure.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIASPNETCORE.Filters;

namespace WebAPIASPNETCORE.Controllers;

[Route("api/[controller]")]
[ApiController]
[UseApiKey]
public class SubscribersController(DataContext context) : ControllerBase
{
    private readonly DataContext _context = context;

    #region CREATE
    [HttpPost]
    public async Task<IActionResult> Subscribe(Subscriber dto)
    {
        if (ModelState.IsValid)
        {
            if (!await _context.Subscribers.AnyAsync(x => x.Email == dto.Email))
            {
                _context.Subscribers.Add(dto);
                await _context.SaveChangesAsync();
                return Created("", null);
            }
            else
            {
                return Conflict();
            }
        }
        return BadRequest();
    }
    #endregion

    #region READ

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subscribers = await _context.Subscribers.ToListAsync();
        if (subscribers.Count != 0)
        {
            return Ok(subscribers);
        }

        return NotFound();
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(string id)
    {
        var subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
        if (subscriber != null)
        {
            return Ok(subscriber);
        }

        return NotFound();
    }

    #endregion

    #region UPDATE 

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOne(string id, string email)
    {

        var subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
        if (subscriber != null)
        {
            subscriber.Email = email;
            _context.Subscribers.Update(subscriber);
            await _context.SaveChangesAsync();

            return Ok(subscriber);
        }

        return NotFound();
    }

    #endregion

    #region DELETE 

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOne(string id)
    {
        var subscriber = await _context.Subscribers.FirstOrDefaultAsync(x => x.Id == id);
        if (subscriber != null)
        {
            _context.Subscribers.Remove(subscriber);
            await _context.SaveChangesAsync();

            return Ok();
        }

        return NotFound();
    }

    #endregion
}


