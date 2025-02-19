using Moq;
using BookHandling.Controllers;
using BookHandling.Services;
using BookHandling.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookHandling.Tests;

public class BooksControllerTests
{
    private readonly BooksController _controller;
    private readonly Mock<IBookService> _mockBookService;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public void GetBooks_ShouldReturnOkWithList()
    {
        var books = new List<Book>
        {
            new Book { Id = Guid.NewGuid(), Title = "Testbok", Author = "Testförfattare", PublishedDate = DateTime.UtcNow.AddDays(-1) }
        };

        _mockBookService.Setup(s => s.GetBooks()).Returns(books);

        var actionResult = _controller.GetBooks();
        var result = Assert.IsType<ActionResult<IEnumerable<Book>>>(actionResult);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedBooks = Assert.IsAssignableFrom<IEnumerable<Book>>(okResult.Value);
        Assert.NotNull(returnedBooks);
        Assert.Single(returnedBooks);
    }

    [Fact]
    public void AddBook_ShouldReturnCreatedAtAction()
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Testbok",
            Author = "Testförfattare",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        _mockBookService.Setup(s => s.AddBook(It.IsAny<Book>()));

        var result = _controller.AddBook(book) as CreatedAtActionResult;

        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
    }

    [Fact]
    public void DeleteBook_ShouldReturnNoContent()
    {
        var bookId = Guid.NewGuid();
        _mockBookService.Setup(s => s.DeleteBook(bookId)).Returns(true);

        var result = _controller.DeleteBook(bookId) as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }

    [Fact]
    public void DeleteBook_ShouldReturnNotFound()
    {
        _mockBookService.Setup(s => s.DeleteBook(It.IsAny<Guid>())).Returns(false);

        var result = _controller.DeleteBook(Guid.NewGuid()) as NotFoundResult;

        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public void UpdateBook_ShouldReturnBadRequest_WhenDataIsInvalid()
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "",
            Author = "Testförfattare",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        _controller.ModelState.AddModelError("Title", "Titel får ej vara tom"); // Lägg till fel i ModelState

        var result = _controller.UpdateBook(book.Id, book);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    
    
}
