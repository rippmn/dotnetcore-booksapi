using Microsoft.AspNetCore.Mvc;  
using BooksApi.Services;
using BooksApi.Models;
using System;  
using System.Collections.Generic;  
  
namespace BooksApi.Controllers  
{  
    [Route("api/[controller]")]  
    public class AuthorsController : ControllerBase  
    {  
        private readonly IAuthorService _authorService;  
  
        public AuthorsController(IAuthorService authorService)  
        {  
            _authorService = authorService;  
        }  
  
        [HttpGet]  
        public IEnumerable<Author> Get()  
        {  
            return _authorService.GetAuthors();  
        }  
  
        [HttpPost]  
        public IActionResult Create([FromBody]Author author)  
        {  
            if (ModelState.IsValid)  
            {  
                Guid obj = Guid.NewGuid();  
                author.id = obj.ToString();  
                _authorService.AddAuthor(author);  
                return Ok();  
            }  
            return BadRequest();  
        }  
  
        [HttpGet("{id}")]  
        public Author Details(string id)  
        {  
            return _authorService.GetAuthorSingleRecord(id);  
        }  
  
        [HttpPut]  
        public IActionResult Edit([FromBody]Author author)  
        {  
            if (ModelState.IsValid)  
            {  
                _authorService.UpdateAuthor(author);  
                return Ok();  
            }  
            return BadRequest();  
        }  
  
        [HttpDelete("{id}")]  
        public IActionResult DeleteConfirmed(string id)  
        {  
            var data = _authorService.GetAuthorSingleRecord(id);  
            if (data == null)  
            {  
                return NotFound();  
            }  
            _authorService.DeleteAuthor(id);  
            return Ok();  
        }  
    }  
}  
