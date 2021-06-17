using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TopicId { get; set; }
        public Topic Topic { get; set;}
        [Required]
        public string AccountCreatorName { get; set; }
        [Required]
        public string Title { get; set; }
        public string DateCreate { get; set; }
        public string DateEdit { get; set; }
        [Required]
        public string Text { get; set; }

        public byte[] Picture1 { get; set; }
     
        public byte[] Picture2 { get; set; }

        public byte[] Picture3 { get; set; }
       
    }
}
