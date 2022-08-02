using api_backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_backend.Controllers
{
   

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly LibDB_context _context;

        private List<DisplayBook> q;
        public HomeController(LibDB_context context)
        {
            _context = context;
        }
        //Student Methods
        //Get
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }
            var temp = _context.Students.Where(x => x.Id == id).FirstOrDefault();

            if (student.BookId != temp.BookId)
            {
                var book = _context.Books.Where(x => x.BookId == student.BookId).FirstOrDefault();
                if (book.count <= 0) { return BadRequest(); }
                if (book != null)
                {

                    book.count--;

                    var book2 = _context.Books.Where(x => x.BookId == temp.BookId).FirstOrDefault();
                    if (book2 != null)
                    {
                        book2.count++;

                        _context.Entry(book2).State = EntityState.Modified;
                    }

                    _context.Entry(book).State = EntityState.Modified;
                    temp.Isreturned = false;

                }

            }

            else
            {
                if (student.returnDate.Date <= DateTime.UtcNow.Date)
                {
                    var book = _context.Books.Where(x => x.BookId == student.BookId).FirstOrDefault();
                    if (book != null)
                    {
                        book.count++;
                        temp.Isreturned = true;
                        _context.Entry(book).State = EntityState.Modified;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                
                if (student.returnDate.Date > DateTime.UtcNow.Date && temp.Isreturned)
                {
                    var book = _context.Books.Where(x => x.BookId == student.BookId).FirstOrDefault();
                    if (book != null)
                    {
                        book.count--;
                        temp.Isreturned = false;
                        _context.Entry(book).State = EntityState.Modified;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            temp.StudentId = student.StudentId;
            temp.returnDate = student.returnDate;
            temp.StudentName = student.StudentName;
            temp.BorrowDate = student.BorrowDate;
            temp.BookId = student.BookId;
            if (student.Isreturned)
            {
                var book = _context.Books.Where(x => x.BookId == student.BookId).FirstOrDefault();
                if (book != null)
                {
                    book.count++;
                    temp.Isreturned = student.Isreturned;
                    _context.Entry(book).State = EntityState.Modified;
                }
                else
                {
                    return NotFound();
                }
            }
            
            _context.Entry(temp).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Content("" + student.BookId);
        }
       
        //Search by StudentId
        [Route("Search/Student/{StudentId}")]
        [HttpGet]
        public async Task<ActionResult<Student>> GetByStudentId(string StudentId)
        {
            var q = await _context.Students.Where(x => x.StudentId == StudentId).FirstOrDefaultAsync();
           if (q == null)
            {
                return BadRequest();
            }
            return q;
        }
        //Sorted with student and book
        [Route("Sorted")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisplayBook>>> GetstudentBooks()
        {

            var stud = await _context.Students.ToListAsync();
            var counts = _context.Books.OrderBy(x => x.count).ToList<Book>();
            var query = from s in stud join bok in counts on s.BookId equals bok.BookId where s.BookId == bok.BookId select new DisplayBook { BookId = s.BookId, count = bok.count, StudentId = s.Id, Title = bok.BookName };
            q = query.ToList();
            return q;
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            
            var book = _context.Books.Where(model => model.BookId == student.BookId).FirstOrDefault();
            if (book == null)
            {
                return BadRequest();
            }
            else if (book.count <= 0)
            {
                return BadRequest();
            }
            book.count = book.count - 1;
            _context.Update(book);
            _context.Entry(book).State = EntityState.Modified;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetStudent", new { id = student.StudentId }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        //Post Book
        [Route("Books")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return Content(""+ _context.Books.Find(book.BookId).BookId);
        }
        //Book post
        [Route("Books/Get")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }
        [Route("Books/Get/{BookId}")]
        [HttpGet]
        public async Task<ActionResult<Book>> GetBook(int BookId)
        {
            var book = await _context.Books.Where(x=>x.BookId==BookId).FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        //Book Put
        [Route("Books/Put/{id}")]
        [HttpPost]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        //Book Delete
        [Route("Books/Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Route("Books/Getbyname/{name}")]
        [HttpGet]
        public async Task<ActionResult<Book>> searchBook( string name) {
            Book c;
            int n;
            bool isNumeric = int.TryParse(name, out n);
            if (isNumeric)
            {
                c = await _context.Books.FindAsync(n);
                return c;
            }
            else
            {
                var bookname = _context.Books.Select(x => x.BookName).ToList<string>();
                foreach (string bname in bookname)
                {
                    string trimmed = String.Concat(name.Where(c => !Char.IsWhiteSpace(c)));
                    string bname1 = String.Concat(bname.Where(c => !Char.IsWhiteSpace(c)));
                    if (bname.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {

                        c = await _context.Books.Where(x => x.BookName == bname).FirstOrDefaultAsync();
                        return c;
                    }
                    else if(trimmed.Equals(bname1, StringComparison.OrdinalIgnoreCase))
                    {
                        c = await _context.Books.Where(x => x.BookName == bname).FirstOrDefaultAsync();
                        return c;
                    }

                }
            }
      
            
                return NotFound();
            


        }
        //Administrators
        [Route("Administrator/{UserId}")]
        [HttpGet]
        public async Task<ActionResult<Administrator>> GetAdmin(string UserId)
        {
            return await _context.Administrators.Where(x => x.UserId == UserId).FirstOrDefaultAsync();
        }
        [Route("Administrator/Post")]
        [HttpPost]
        public async Task<ActionResult<Administrator>> PutAdmin(Administrator admin)
        {


            _context.Administrators.Add(admin);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }

            return Ok(admin);

        }
        [Route("Administrator/Login")]
        [HttpPost]
        public async Task<ActionResult<Administrator>> PutAdmin(Login admin)
        {
            var logins = await _context.Administrators.Where(x => x.UserId == admin.UserId).FirstOrDefaultAsync();
          if(logins == null)
            {
                return BadRequest();
            }
            else if(logins.Password==admin.Password)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
            
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
