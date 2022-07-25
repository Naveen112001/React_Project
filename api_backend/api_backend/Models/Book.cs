using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace api_backend.Models
{
    public class Book
    {
        
        [Key]
        [Required]
        public int BookId { get; set; }
        public string BookName { get; set; }
        [Range(0, int.MaxValue)]
        public int count { get; set; }
        public string author { get; set; }
        //public virtual  ICollection<Student> Student{get;set;}
        
    }
}
