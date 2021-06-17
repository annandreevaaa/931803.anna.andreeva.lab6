using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public byte[] PictureFile { get; set; }
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
    }
    public class PictureViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IFormFile PictureFile { get; set; }
    }
}
