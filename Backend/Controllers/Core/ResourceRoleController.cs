using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Context;
using Share.EntityModels.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers.Core
{
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceRoleController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public ResourceRoleController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/RoleResourceRole
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResource>>> GetRolesResources()
        {
            return await _context.RolesResources.ToListAsync();
        }

        // GET: api/RoleResourceRole/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResource>> GetRoleResource(int id)
        {
            var roleResource = await _context.RolesResources.FindAsync(id);

            if (roleResource == null)
            {
                return NotFound();
            }

            return roleResource;
        }

        // PUT: api/RoleResourceRole/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoleResource(int id, RoleResource roleResource)
        {
            if (id != roleResource.IdRoleResource)
            {
                return BadRequest();
            }

            _context.Entry(roleResource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleResourceExists(id))
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

        // POST: api/RoleResourceRole
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoleResource>> PostRoleResource(RoleResource roleResource)
        {
            _context.RolesResources.Add(roleResource);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoleResource", new { id = roleResource.IdRoleResource }, roleResource);
        }

        // DELETE: api/RoleResourceRole/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleResource(int id)
        {
            var roleResource = await _context.RolesResources.FindAsync(id);
            if (roleResource == null)
            {
                return NotFound();
            }

            _context.RolesResources.Remove(roleResource);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleResourceExists(int id)
        {
            return _context.RolesResources.Any(e => e.IdRoleResource == id);
        }
    }
}
