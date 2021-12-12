using BookRepository.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookRepository.Data
{
    public interface IBookData
    {
        IEnumerable<Book> ListBooks();
        Task<IEnumerable<Book>> ListBooksAsync();
        Book GetBook(int Id);
        Task<Book> GetBookAsync(int Id);
        Book UpdateBook(Book bookData);
        void AddBook(Book newBook);
        int Save();
        Task<int> SaveAsync<T>(T entity) where T : IEntity;
        Task<bool> UpdateAsync<T>(T entity) where T : IEntity;

        Task<bool> DeleteAsync<T>(T entity) where T : IEntity;
    }
}