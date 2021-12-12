using BookRepository.Data;
using BookRepository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BookRepository.Core;
using Microsoft.AspNetCore.Routing;

namespace BookRepository.Controllers
{
    //[Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class Book2Controller : ControllerBase
    {
        private readonly IBookData _service;
        private readonly LinkGenerator _linkGenerator;

        public Book2Controller(IBookData service, LinkGenerator linkGenerator)
        {
            _service = service;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                var books = await _service.ListBooksAsync();

                var results = new
                {
                    Count = books.Count(),
                    Books = books
                };

                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a database failure");
            }
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<BookModel>> GetBook(int Id)
        {
            try
            {
                var result = await _service.GetBookAsync(Id);
                return result == null
                    ? NotFound($"The book with ID {Id} was not found")
                    : new BookModel()
                    {
                        Author = result.Author,
                        Description = result.Description,
                        Title = result.Title,
                        Publisher = result.Publisher,
                        ISBN = result.ISBN
                    };
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a database failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<BookModel>>> SearchIsbn(string Isbn)
        {
            try
            {
                var books = await _service.ListBooksAsync();
                var results = books.Where(b => b.ISBN.StartsWith(Isbn));

                return !results.Any()
                    ? NotFound()
                    : (from book in results
                       let model = new BookModel()
                       {
                           Author = book.Author,
                           Description = book.Description,
                           Title = book.Title,
                           Publisher = book.Publisher,
                           ISBN = book.ISBN
                       }
                       select model).ToList();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a database failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> Post(BookModel model)
        {
            try
            {
                var entityLocation = "";

                var entity = new Book()
                {
                    Author = model.Author,
                    Description = model.Description,
                    Title = model.Title,
                    Publisher = model.Publisher,
                    ISBN = model.ISBN
                };

                var createdBookId = await _service.SaveAsync(entity);
                if (createdBookId > 0)
                {
                    entityLocation = _linkGenerator.GetPathByAction("GetBook", "Book", new { Id = createdBookId });
                    return Created(entityLocation, model);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was a database failure");
            }

            return BadRequest();
        }


        [HttpPut("{Id}")]
        public async Task<ActionResult<BookModel>> Put(int Id, BookModel model)
        {
            try
            {
                var bookToUpdate = await _service.GetBookAsync(Id);

                if (bookToUpdate != null)
                {
                    bookToUpdate.Author = model.Author;
                    bookToUpdate.Description = model.Description;
                    bookToUpdate.Title = model.Title;
                    bookToUpdate.Publisher = model.Publisher;
                    bookToUpdate.ISBN = model.ISBN;

                    return await _service.UpdateAsync(bookToUpdate) ? model : BadRequest();
                }
                else
                {
                    return NotFound($"Can't find book with Id {Id}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"There was a database failure: {ex.Message}");
            }
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                var bookToDelete = await _service.GetBookAsync(Id);

                return bookToDelete != null
                    ? await _service.DeleteAsync(bookToDelete) ? Ok() : BadRequest()
                    : NotFound($"Can't find book with Id {Id}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"There was a database failure: {ex.Message}");
            }
        }
    }
}