using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        static private List<Models.Book> Books = new List<Models.Book>
        {
            new Models.Book { Id = 1, Title = "The Great Gatsby", Description = "A novel by F. Scott Fitzgerald", Author = "F. Scott Fitzgerald", YearPublished = 1925 },
            new Models.Book { Id = 2, Title = "To Kill a Mockingbird", Description = "A novel by Harper Lee", Author = "Harper Lee", YearPublished = 1960 },
            new Models.Book { Id = 3, Title = "1984", Description = "A novel by George Orwell", Author = "George Orwell", YearPublished = 1949 }
        };
        [HttpGet]
        public ActionResult<IEnumerable<Models.Book>> GetBooks()
        {
            return Ok(Books);
        }

        [HttpGet("{id}")]
        public ActionResult<Models.Book> GetBookById(int id)
        {
            var book = Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpPost]
        public ActionResult<Models.Book> CreateBook(Models.Book book)
        {
            if (book == null)
            {
                return BadRequest();
            }
            book.Id = Books.Max(x => x.Id) + 1;
            Books.Add(book);
            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);

        }
}
