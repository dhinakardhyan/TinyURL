using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;
using TinyUrlApi.Models;

namespace TinyUrlApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public UrlController(AppDbContext context,IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("public")]
        public async Task<IActionResult>GetPublicUrls()
        {
            var urls = await _context.Urls
                         .Where(x => x.IsPrivate==false)
                         .Select(x => new
                         {
                             x.Id,
                             x.OriginalUrl,
                             x.ShortCode,
                             x.Clicks
                         })
                         .ToListAsync();

            return Ok(urls);
        }



      

        [HttpPost("shorten")]
        public async Task<IActionResult> CreateShortUrl([FromBody] Url url)
        {
            // Generate short code
            var shortCode = GenerateShortCode();

            url.ShortCode = shortCode;
            url.Clicks = 0;

            _context.Urls.Add(url);
            await _context.SaveChangesAsync();

            return Ok(url);
        }

        private string GenerateShortCode()
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();

            return new string(Enumerable.Range(0, 6)
                .Select(x => chars[random.Next(chars.Length)]).ToArray());
        }

       


    }


}
