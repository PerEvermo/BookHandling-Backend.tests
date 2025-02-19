using BookHandling.Models;
using BookHandling.Services;

namespace BookHandling.Tests;

public class BookServiceTests
{
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookService = new BookService();
    }

    [Fact]
    public void AddBook_ShouldAddBookSuccessfully()
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Testbok",
            Author = "Testförfattare",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        _bookService.AddBook(book);
        var books = _bookService.GetBooks().ToList();

        Assert.Single(books);
        Assert.Equal("Testbok", books[0].Title);
    }

    [Fact]
    public void GetBooks_ShouldReturnEmptyListInitially()
    {
        var books = _bookService.GetBooks().ToList();
        Assert.Empty(books);
    }

    [Fact]
    public void DeleteBook_ShouldRemoveBookIfExists()
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Testbok",
            Author = "Testförfattare",
            PublishedDate = DateTime.UtcNow.AddDays(-1)
        };

        _bookService.AddBook(book);
        var result = _bookService.DeleteBook(book.Id);

        Assert.True(result);
        Assert.Empty(_bookService.GetBooks());
    }

    [Fact]
    public void DeleteBook_ShouldReturnFalseIfBookDoesNotExist()
    {
        var result = _bookService.DeleteBook(Guid.NewGuid());
        Assert.False(result);
    }
    
    [Fact]
    public void UpdateBook_ShouldUpdateSuccessfully()
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Originaltitel",
            Author = "Originalförfattare",
            PublishedDate = DateTime.UtcNow.AddDays(-10)
        };

        _bookService.AddBook(book);

        var updatedBook = new Book
        {
            Id = book.Id,
            Title = "Uppdaterad Titel",
            Author = "Uppdaterad Författare",
            PublishedDate = DateTime.UtcNow.AddDays(-5)
        };

        var result = _bookService.UpdateBook(book.Id, updatedBook);

        var books = _bookService.GetBooks().ToList();
        var storedBook = books.FirstOrDefault(b => b.Id == book.Id);

        Assert.True(result);
        Assert.NotNull(storedBook);
        Assert.Equal("Uppdaterad Titel", storedBook.Title);
        Assert.Equal("Uppdaterad Författare", storedBook.Author);
        Assert.Equal(updatedBook.PublishedDate, storedBook.PublishedDate);
    }

}