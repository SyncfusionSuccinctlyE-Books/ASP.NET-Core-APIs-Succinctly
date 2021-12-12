using BookRepository.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookRepository.Data
{
    public class SqlData : IBookData
    {
        private readonly BookRepoDbContext _database;

        public SqlData(BookRepoDbContext database)
        {
            _database = database;
        }

        public void AddBook(Book newBook)
        {
            _ = _database.Add(newBook);
        }

        public Book GetBook(int Id)
        {
            return _database.Books.Find(Id);
        }

        public async Task<Book> GetBookAsync(int Id)
        {
            return await _database.Books.FindAsync(Id);
        }

        public IEnumerable<Book> ListBooks()
        {
            return _database.Books.OrderBy(b => b.Title);
        }

        public async Task<IEnumerable<Book>> ListBooksAsync()
        {
            return await _database.Books
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public int Save()
        {
            return _database.SaveChanges();
        }

        public async Task<int> SaveAsync<T>(T entity) where T : IEntity
        {
            var addedEntity = _database.Add(entity);
            var entityId = -1;

            if (await _database.SaveChangesAsync() > -1)
            {
                entityId = Convert.ToInt32(addedEntity.Property("Id").CurrentValue);
            }

            return entityId;

            #region ...
            //var state = EntityState.Detached;
            //if (entity.Id > 0)
            //    state = EntityState.Modified;

            //EntityEntry entityEntry;

            //switch (state)
            //{
            //    case EntityState.Detached:
            //        entityEntry = _database.Add(entity);                    
            //        break;
            //    case EntityState.Modified:
            //        entityEntry = _database.Attach(entity);
            //        entityEntry.State = state;
            //        break;
            //    case EntityState.Unchanged:
            //    case EntityState.Deleted:
            //    case EntityState.Added:
            //    default:
            //        entityEntry = default;
            //        break;
            //}

            ////var entityEntry = _database.Attach(entity);
            ////entityEntry.State = state;



            ////var addedEntity = _database.Add(entity);
            //var entityId = -1;

            //if (await _database.SaveChangesAsync() > -1)
            //{
            //    entityId = Convert.ToInt32(entityEntry.Property("Id").CurrentValue);
            //}            

            //return entityId;            
            #endregion
        }

        public async Task<bool> UpdateAsync<T>(T entity) where T : IEntity
        {
            var updatedEntity = _database.Attach(entity);
            updatedEntity.State = EntityState.Modified;

            return (await _database.SaveChangesAsync() > 0);
        }

        public Book UpdateBook(Book bookData)
        {
            var entity = _database.Books.Attach(bookData);
            entity.State = EntityState.Modified;
            return bookData;
        }


        public async Task<bool> DeleteAsync<T>(T entity) where T : IEntity
        {
            var updatedEntity = _database.Remove(entity);
            updatedEntity.State = EntityState.Deleted;

            return (await _database.SaveChangesAsync() > 0);
        }

    }
}
