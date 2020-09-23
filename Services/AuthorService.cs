using BooksApi.Models;  
using System.Collections.Generic;  
using System.Linq;  
  
namespace BooksApi.Services
{  
    public interface IAuthorService
    {  
        void AddAuthor(Author author);  
        void UpdateAuthor(Author author);  
        void DeleteAuthor(string id);  
        Author GetAuthorSingleRecord(string id);  
        List<Author> GetAuthors();  
    }  
    
    public class AuthorService: IAuthorService 
    {  
        private readonly PostgreSqlContext _context;  
  
        public AuthorService(PostgreSqlContext context)  
        {  
            _context = context;  
        }  
  
        public void AddAuthor(Author author)  
        {  
            _context.authors.Add(author);  
            _context.SaveChanges();  
        }  
  
        public void UpdateAuthor(Author author)
        {  
            _context.authors.Update(author);  
            _context.SaveChanges();  
        }  
  
        public void DeleteAuthor(string id)
        {  
            var entity = _context.authors.FirstOrDefault(t => t.id == id);  
            _context.authors.Remove(entity);  
            _context.SaveChanges();  
        }  
  
        public Author GetAuthorSingleRecord(string id)
        {  
            return _context.authors.FirstOrDefault(t => t.id == id);  
        }  
  
        public List<Author> GetAuthors()
        {  
            return _context.authors.ToList();  
        }  
    }  
}  

