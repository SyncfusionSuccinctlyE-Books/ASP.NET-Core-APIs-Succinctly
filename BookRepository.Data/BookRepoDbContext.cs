using BookRepository.Core;
using Microsoft.EntityFrameworkCore;

namespace BookRepository.Data
{
    public class BookRepoDbContext : DbContext
    {
        public BookRepoDbContext(DbContextOptions<BookRepoDbContext> dbContextOptns) : base(dbContextOptns)
        {

        }

        public DbSet<Book> Books { get; set; }
    }
}
