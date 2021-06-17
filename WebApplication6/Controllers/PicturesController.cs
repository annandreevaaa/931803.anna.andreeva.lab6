using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Controllers
{
    public class PicturesController : Controller
    {
        private readonly AppdbContext _context;

        public PicturesController()
        {
            _context = new AppdbContext();
        }

        // GET: Pictures
        public async Task<IActionResult> Index()
        {
            var appdbContext = _context.Pictures.Include(p => p.Folder);
            return View(await appdbContext.ToListAsync());
        }

        // GET: Pictures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures
                .Include(p => p.Folder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (picture == null)
            {
                return NotFound();
            }

            return View(picture);
        }

        // GET: Pictures/Create
        [Authorize]
        public IActionResult Create(int? id)
        {
            ViewBag.FolderId = id;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PictureFile")] PictureViewModel picture, int? id)
        {
            if (ModelState["Name"].ValidationState == ModelValidationState.Valid &
                ModelState["PictureFile"].ValidationState == ModelValidationState.Valid)
            {
                if (_context.Folders.FirstOrDefault(m => m.Id == id) == null)
                {
                    id = _context.Folders.FirstOrDefault(m => m.Name == "root").Id;
                }
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader((picture.PictureFile).OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)(picture.PictureFile).Length);
                }
                _context.Add(new Picture {Name = picture.Name, PictureFile = imageData, FolderId= (int)id});
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Folders", new { id = (int)id });
            }
            return View(picture);
        }

        // GET: Pictures/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Picture picture)
        {
            if (ModelState["Name"].ValidationState==ModelValidationState.Valid)
            {
                Picture Newpicture = _context.Pictures.FirstOrDefault(m => m.Id == id);
                Newpicture.Name = picture.Name;
                try
                {
                    _context.Update(Newpicture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PictureExists(picture.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Folders", new { id = Newpicture.FolderId });
            }
            return View(picture);
        }

        // GET: Pictures/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id, int Folderid)
        {
            if (id == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures
                .Include(p => p.Folder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (picture == null)
            {
                return NotFound();
            }
            _context.Pictures.Remove(picture);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Folders", new { id = Folderid });
        }


        private bool PictureExists(int id)
        {
            return _context.Pictures.Any(e => e.Id == id);
        }
    }
}
