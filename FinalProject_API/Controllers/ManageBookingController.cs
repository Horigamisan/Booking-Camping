using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalProject_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject_API.Controllers
{
    [Route("api/manage/booking")]
    [ApiController]
    [Authorize(Roles = "manager")]
    public class ManageBookingController : ControllerBase
    {
        private readonly FinalProject_SOAContext _context;

        public ManageBookingController(FinalProject_SOAContext context)
        {
            _context = context;
        }

        // GET: api/ManageBooking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatCho>>> GetDatChos()
        {
          if (_context.DatChos == null)
          {
              return NotFound();
          }
            return await _context.DatChos.ToListAsync();
        }

        // GET: api/ManageBooking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DatCho>> GetDatCho(int id)
        {
          if (_context.DatChos == null)
          {
              return NotFound();
          }
            var datCho = await _context.DatChos.FindAsync(id);

            if (datCho == null)
            {
                return NotFound();
            }

            return datCho;
        }

        // PUT: api/ManageBooking/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatCho(int id, DatCho datCho)
        {
            if (id != datCho.DatchoId)
            {
                return BadRequest();
            }

            _context.Entry(datCho).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatChoExists(id))
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

        // POST: api/ManageBooking
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DatCho>> PostDatCho(DatCho datCho)
        {
          if (_context.DatChos == null)
          {
              return Problem("Entity set 'FinalProject_SOAContext.DatChos'  is null.");
          }
            _context.DatChos.Add(datCho);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatCho", new { id = datCho.DatchoId }, datCho);
        }

        // DELETE: api/ManageBooking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatCho(int id)
        {
            if (_context.DatChos == null)
            {
                return NotFound();
            }
            var datCho = await _context.DatChos.FindAsync(id);
            if (datCho == null)
            {
                return NotFound();
            }

            _context.DatChos.Remove(datCho);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DatChoExists(int id)
        {
            return (_context.DatChos?.Any(e => e.DatchoId == id)).GetValueOrDefault();
        }
    }
}
