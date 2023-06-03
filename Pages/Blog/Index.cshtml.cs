using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razor07.Models;

namespace razor07.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly razor07.Models.MyBlogContext _context;

        public IndexModel(razor07.Models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; }
        public const int itemPerPage = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            // Article = await _context.articles.ToListAsync();

            int totalPages = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalPages/itemPerPage);
            if(currentPage < 1)
            {
                currentPage = 1;
            }
            if(currentPage>countPages)
            {
                currentPage = countPages;
            }
            var qr = await (from a in _context.articles
                      orderby a.CreatedAt descending
                      select a).Skip((currentPage-1)*itemPerPage).Take(itemPerPage).ToListAsync();
            if(!string.IsNullOrEmpty(searchString))
            {
                Article = qr.Where(a => a.Title.Contains(searchString)).ToList();
                
            }
            else{
                Article = qr;
            }
        }
    }
}
