using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class Forum
    {
        [Key]
        public  int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public List<Topic> Topics { get; set; }
        public Forum()
        {
            Topics = new List<Topic>();
        }
    }
}
