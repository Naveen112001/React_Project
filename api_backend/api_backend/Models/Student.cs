using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using Microsoft.EntityFrameworkCore;

namespace api_backend.Models
{
    public class Student

    {
        [Key]
        [Required]

        public int Id { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.Schema.Index(IsUnique =true)]
        public string StudentId { get; set; }
        public string StudentName { get; set; }
       
        public int BookId { get; set; }
        [DataType(DataType.Date)]
     
        public DateTime BorrowDate{ get; set; }
        [DataType(DataType.Date)]
        public DateTime returnDate { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool Isreturned { get; set; }
        [ForeignKey("BookId")]
        public  Book Book { get; set; }

    }
    public enum status
    {
        blocked=0,
        unblocked=1
    }
}
