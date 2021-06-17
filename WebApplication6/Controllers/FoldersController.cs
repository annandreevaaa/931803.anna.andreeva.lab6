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
    public class FoldersController : Controller
    {
        private readonly AppdbContext _context;

        public FoldersController( )
        {
            _context = new AppdbContext();
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (_context.Folders.FirstOrDefault(m => m.Id == id) == null)
                    return LocalRedirect("~/Home/Index");
            ViewData["Title"] = _context.Folders.FirstOrDefault(m => m.Id == id).Name;
            ViewBag.FolderId = id;
            ViewBag.FolderPreId = _context.Folders.FirstOrDefault(m => m.Id == id).FoldersId;
            ViewBag.Picture = _context.Pictures.Where(m => m.FolderId == id);
            return View(await _context.Folders.Where(f => f.FoldersId == id).ToListAsync());
        }

        public async Task<IActionResult> Choose(int? id,int  number)
        {
            ViewBag.PostsId = id;
            id = _context.Folders.FirstOrDefault(m => m.Name == "root").Id;
            ViewData["Title"] = _context.Folders.FirstOrDefault(m => m.Id == id).Name;
            ViewBag.FolderId = id;
            ViewBag.FolderPreId = _context.Folders.FirstOrDefault(m => m.Id == id).FoldersId;
            ViewBag.Picture = _context.Pictures.Where(m => m.FolderId == id);
            ViewBag.Number = number;
            return View(await _context.Folders.Where(f => f.FoldersId == id).ToListAsync());
        }

        [Authorize]
        public IActionResult Create()
        {
            ViewData["FoldersId"] = new SelectList(_context.Folders, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Folder folder, int? id)
        {
            if (ModelState.IsValid)
            {
                if (_context.Folders.FirstOrDefault(m => m.Id == id) == null)
                {
                    id = _context.Folders.FirstOrDefault(m => m.Name == "root").Id;
                }
                folder.FoldersId = _context.Folders.FirstOrDefault(m => m.Id == id).Id;
                _context.Add(folder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = folder.FoldersId});
            }
            ViewData["FoldersId"] = new SelectList(_context.Folders, "Id", "Name", folder.FoldersId);
            return View(folder);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
            {
                return NotFound();
            }
            ViewData["FoldersId"] = new SelectList(_context.Folders, "Id", "Name", folder.FoldersId);
            return View(folder);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,FoldersId")] Folder folder)
        {
            if (id != folder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(folder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FolderExists(folder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["FoldersId"] = new SelectList(_context.Folders, "Id", "Name", folder.FoldersId);
            return View(folder);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var folder = await _context.Folders
                .Include(f => f.Folders)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (folder == null)
            {
                return NotFound();
            }
            int Folderid = (int)folder.FoldersId;
            DeleteSubFoldersandFiles(_context.Folders.Where(m=>m.FoldersId == folder.Id).ToList());
            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Folders", new { id = Folderid });

        }

        private void DeleteSubFoldersandFiles(List<Folder> folder)
        {
            foreach (var item in folder)
            {
                DeleteSubFoldersandFiles(_context.Folders.Where(m => m.FoldersId == item.Id).ToList());
                _context.Folders.Remove(item);
            }
        }

        private bool FolderExists(int id)
        {
            return _context.Folders.Any(e => e.Id == id);
        }
    }
}
