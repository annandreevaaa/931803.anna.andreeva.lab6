using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        AppdbContext _context;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = new AppdbContext();
            _context.SaveChanges();
            if (_context.Folders.FirstOrDefault(m => m.Id > 0) == null)
                _context.Folders.Add(new Folder() { Id = 0 , Name = "root", });
            if (_context.Forums.FirstOrDefault(m => m.Id > 0) == null)
            {
                _context.Forums.Add(new Forum()
                {
                    Name = "Root Category", 
                    Description = "Main Category" 
                });
                _context.SaveChanges();
                _context.Topics.Add(new Topic()
                {
                    Name = "Root Topic",
                    AccountCreatorName = "admin",
                    AccountEditorName = "admin",
                    ForumID = _context.Forums.FirstOrDefault(m => m.Id >= 0).Id,
                    DateCreate = "Yesterday"
                });
            }
            _context.SaveChanges();
        }

        public IActionResult Index()
        {
            
            return View();
        }

    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}