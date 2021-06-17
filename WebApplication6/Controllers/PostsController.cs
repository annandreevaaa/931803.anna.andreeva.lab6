using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppdbContext _context;

        public PostsController()
        {
            _context = new AppdbContext();
        }

        // GET: Posts
        public async Task<IActionResult> Index(int? id)
        {
            if (_context.Topics.FirstOrDefault(m => m.Id > 0) == null)
                return NotFound();
            if (_context.Topics.FirstOrDefault(m => m.Id == id) == null)
                id = _context.Topics.FirstOrDefault(m => m.Id > 0).Id;
            ViewBag.TopicId = id;
            ViewData["Title"] = _context.Topics.FirstOrDefault(m => m.Id == id).Name;
            return View(await _context.Posts.Where(m => m.TopicId == id).ToListAsync());
        }


        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
        [Authorize]
        // GET: Posts/Create
        public IActionResult Create(int TopicId)
        {
            ViewBag.TopicId = TopicId;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Text")] Post post, string Create, int? TopicId)
        {
            DateTime date1 = DateTime.Now;
            post.DateCreate = "" + date1.DayOfWeek + ", " + date1.ToLongDateString() + ", " + date1.ToLongTimeString();
            
            if (TopicId == null)
            {
                post.TopicId = _context.Topics.FirstOrDefault(m => m.Id > 0).Id;
            }
            else post.TopicId = (int)TopicId;

            Topic topic = _context.Topics.FirstOrDefault(m => m.Id == post.TopicId);// post.TopicId);

            topic.AccountEditorName = User.Identity.Name;
            post.AccountCreatorName = User.Identity.Name;
           
            _context.Update(topic);
            _context.Add(post);

            await _context.SaveChangesAsync();
            int id = _context.Posts.Where(m => m.TopicId == post.TopicId).ToList().LastOrDefault().Id;
            if (Create == "Add photo")
            {
                return RedirectToAction("Choose", "Folders", new { id, Number = 1 });
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id, int PictureId, int Number)
        {
            if (id == null)
            {
                return NotFound();
            }

            Post p = _context.Posts.FirstOrDefault(m => m.Id == id);
            switch (Number)
            {
                case 1:
                    p.Picture1 = _context.Pictures.FirstOrDefault(m => m.Id == PictureId).PictureFile;
                    break;
                case 2:
                    p.Picture2 = _context.Pictures.FirstOrDefault(m => m.Id == PictureId).PictureFile;
                    break;
                case 3:
                    p.Picture3 = _context.Pictures.FirstOrDefault(m => m.Id == PictureId).PictureFile;
                    break;
                default:
                    break;
            }
            Topic topic = _context.Topics.Find(p.TopicId);
            topic.AccountEditorName = User.Identity.Name;
            _context.Update(topic);
            _context.Update(p);
            await _context.SaveChangesAsync();
             var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(p);
        }
        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Text")] Post post, string Create)
        {
            switch (Create)
            {
                case "Add Photo 1":
                    return RedirectToAction("Choose", "Folders", new { id, Number = 1 });
                case "Add Photo 2":
                    return RedirectToAction("Choose", "Folders", new { id, Number = 2 });
                case "Add Photo 3":
                    return RedirectToAction("Choose", "Folders", new { id, Number = 3 });
                default:
                    break;
            }
            if (id != post.Id)
            {
                return NotFound();
            }

            Post p = _context.Posts.FirstOrDefault(m => m.Id == post.Id);
            p.Title = post.Title;
            p.Text = post.Text;
            switch (Create)
            {
                case "Delete Photo 1":
                    p.Picture1 = null;
                    break;
                case "Delete Photo 2":
                    p.Picture2 = null;
                    break;
                case "Delete Photo 3":
                    p.Picture3 = null;
                    break;
                default:
                    break;
            }
            try
            {
                _context.Update(p);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(post.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (Create =="Save")
                return RedirectToAction(nameof(Index),new {id = post.TopicId });
            return View(p);
        }

        [Authorize]
        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            int TopicId = post.TopicId;
            if (post == null)
            {
                return NotFound();
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Posts",new { id = TopicId });
        }

   
        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
