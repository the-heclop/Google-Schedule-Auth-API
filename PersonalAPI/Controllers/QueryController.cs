using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalAPI.Data;

namespace PersonalAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly DataContext _context;
        public QueryController(DataContext context)
        {
            _context = context;
        }

        [Route("queries")]
        [HttpPost]
        public async Task<ActionResult> Queries(string[] ids)
        {
            var query = await _context.tracking_data.Where(u => ids.Contains(u.tracking_id))
             .Select(x => new { x.tracking_id, x.last_scan_location, x.order_number, x.destination, x.scan_time }).ToListAsync();

            if (!query.Any())
            {
                return NotFound();
            }

            return Ok(query);

        }

    }
}
