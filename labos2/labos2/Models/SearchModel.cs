
using labos2.Data;
using labos2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace YourNamespace.Pages.Home
{
    public class SearchModel : PageModel
    {
        private readonly labos2.Data.DbContext _dbContext;

        public SearchModel(labos2.Data.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; }

        public List<Klubovi> SearchResults { get; set; }

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Keyword))
            {
                var filter = Builders<Klubovi>.Filter.Regex(x => x.id, new BsonRegularExpression(Keyword, "i"));
                SearchResults = await _dbContext.Klubovi.Find(x => x.ime == Keyword).ToListAsync();
            }
        }
    }
}
