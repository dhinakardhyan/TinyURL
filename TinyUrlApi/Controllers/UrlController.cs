using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;
using TinyUrlApi.Models;

namespace TinyUrlApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        public UrlController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicUrls()
        {
            var urls = await _context.Urls
                         .Where(x => x.IsPrivate == false)
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



        [HttpGet("/r/{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode, [FromQuery] string? token)
        {
            var url = await _context.Urls
                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (url == null)
                return NotFound("Short URL not found");

            if (url.IsPrivate)
            {
                var validToken = _config["AppSettings:SecretToken"];

                if (token != validToken)
                    return Unauthorized("Private URL");
            }

            url.Clicks++;
            await _context.SaveChangesAsync();

            return Redirect(url.OriginalUrl);
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

        [HttpGet("Search")]
        public async Task<IActionResult> SearchUrls(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Searched word is required");
            }
            var results = await _context.Urls
                .Where(x => x.ShortCode.Contains(query.ToLower()) || x.OriginalUrl.Contains(query.ToLower()))
                .Select(x => new
                {
                    x.Id,
                    x.ShortCode,
                    x.OriginalUrl,
                    x.Clicks
                })
                .ToListAsync();

            return Ok(new { success = true, data = results });
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUrl(int id)
        {
            var url = await _context.Urls.FindAsync(id);
            if (url == null)
                return NotFound("Url Not Found");

            _context.Urls.Remove(url);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Url Deleted Succesfully" });
        }


    }


}
