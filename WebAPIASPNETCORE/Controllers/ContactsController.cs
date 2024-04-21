using Infrastructure.Contexts;
using Infrastructure.Dtos;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIASPNETCORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController(DataContext context) : ControllerBase
    {
        private readonly DataContext _context = context;

        [HttpPost]
        public async Task<IActionResult> SendContactForm(ContactForm form)
        {
            if (ModelState.IsValid)
            {
                var contactEntity = new ContactEntity
                {
                    FullName = form.FullName,
                    Email = form.Email,
                    Service = form.Service,
                    Message = form.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Contacts.Add(contactEntity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Your message has been received" });
               
            }

            return BadRequest();
        }
    }
}
