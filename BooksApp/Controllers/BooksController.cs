using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace BooksApp.Controllers
{
    [Route("books")] // prefix to url - nice litte tid bit!
    public class BooksController : Controller
    {
        // properites
        private readonly IBooksService _booksService;
        private readonly IAuthorsService _authorsService;

        public BooksController(IBooksService booksService, IAuthorsService authorsService)
        {
            _booksService = booksService;
            _authorsService = authorsService;
        }

        [Route("/")]
        [Route("index")] // books/index (book prefix)
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(BookResponse.BookName), SortOrderOptions sortOrder = SortOrderOptions.Asc)
        {

            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(BookResponse.BookName), "Book Name"},
                { nameof(BookResponse.BookRating), "Book Rating"},
                { nameof(BookResponse.Publisher), "Publisher"},
                { nameof(BookResponse.PublishedDate), "Published Date"},
                { nameof(BookResponse.Genres), "Genres"},
                { nameof(BookResponse.IsOngoing), "Is Ongoing"},
                { nameof(BookResponse.AuthorId), "Author"},
            };

            List<BookResponse> books = _booksService.GetFilteredBooks(searchBy, searchString);
            ViewBag.SearchString = searchString;
            ViewBag.SearchBy = searchBy;

            List<BookResponse> sortedBooks = _booksService.GetSortedBooks(books, sortBy, sortOrder);
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder.ToString();

            List<BookResponse> returnedBooks = _booksService.GetAllBooks();
            return View(sortedBooks); // Views/Books/Index.cshtml
        }

        [Route("create")]
        [HttpGet] // Action method only accepts GET requets
        public IActionResult Create()
        {
            List<AuthorResponse> authorRes = _authorsService.GetAllAuthors();
            List<string> availableGenres = GetGenres();

            ViewBag.AvailableGenres = availableGenres;
            ViewBag.Authors = authorRes;

            return View();
        }

        [Route("create")]
        [HttpPost] // Action method only accepts POST requets
        public IActionResult Create(BookAddRequest book)
        {
            if (!ModelState.IsValid)
            {
                List<AuthorResponse> authorRes = _authorsService.GetAllAuthors();
                List<string> availableGenres = GetGenres();

                ViewBag.AvailableGenres = availableGenres;
                ViewBag.Authors = authorRes;
                ViewBag.Errors = ModelState.Values.SelectMany(n => n.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }

            BookResponse bookRes = _booksService.AddBook(book);
            
            return RedirectToAction("Index", "Books");
        }

        public List<string> GetGenres()
        {
            List<string> availableGenres = new List<string>
            {
                "Fiction",
                "Non-fiction",
                "Science Fiction",
                "Mystery",
                "Action",
                "Adventure",
                "Comedy",
                "Drama",
                "Fantasy",
                "Fantasy",
                "Horror",
                "Isekai",
                "Sci-Fi",
                "Shounen",
                "Sports"
            };

            return availableGenres;
        }
    }
}
