using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Share.Models;

namespace Backend.Controllers.Core.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersRoleController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public UsersRoleController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/UsersRole
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersRole>>> GetUsersRoles()
        {
            return await _context.UsersRoles.ToListAsync();
        }

        // GET: api/UsersRole/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsersRole>> GetUsersRole(int id)
        {
            var usersRole = await _context.UsersRoles.FindAsync(id);

            if (usersRole == null)
            {
                return NotFound();
            }

            return usersRole;
        }

        // PUT: api/UsersRole/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsersRole(int id, UsersRole usersRole)
        {
            if (id != usersRole.IdUserRole)
            {
                return BadRequest();
            }

            _context.Entry(usersRole).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersRoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UsersRole
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UsersRole>> PostUsersRole(UsersRole usersRole)
        {
            _context.UsersRoles.Add(usersRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsersRole", new { id = usersRole.IdUserRole }, usersRole);
        }

        // DELETE: api/UsersRole/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsersRole(int id)
        {
            var usersRole = await _context.UsersRoles.FindAsync(id);
            if (usersRole == null)
            {
                return NotFound();
            }

            _context.UsersRoles.Remove(usersRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersRoleExists(int id)
        {
            return _context.UsersRoles.Any(e => e.IdUserRole == id);
        }
    }
}
