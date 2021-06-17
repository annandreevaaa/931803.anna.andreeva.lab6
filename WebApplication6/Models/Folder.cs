using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? FoldersId { get; set; }
        public Folder Folders { get; set; }
       
    }
}
