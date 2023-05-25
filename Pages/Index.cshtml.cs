using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razor07.Models;

namespace razor07.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MyBlogContext _context;

    public IndexModel(ILogger<IndexModel> logger, MyBlogContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void OnGet()
    {
        var posts = (from article in _context.articles
                    orderby article.CreatedAt descending
                    select article).ToList();
        ViewData["posts"] = posts;
    }
}
