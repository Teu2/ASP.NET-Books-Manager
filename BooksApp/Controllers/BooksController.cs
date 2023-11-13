using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [Route("[action]")] // --> books/index (book prefix) | [action] refers to whatever name we give the IActionResult E.g [action] == 'index'
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(BookResponse.BookName), SortOrderOptions sortOrder = SortOrderOptions.Asc)
        {

            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(BookResponse.BookName), "Book Name"},
                { nameof(BookResponse.BookRating), "Book Rating"},
                { nameof(BookResponse.Publisher), "Publisher"},
                { nameof(BookResponse.PublishedDate), "Published Date"},
                { nameof(BookResponse.Genress), "Genres"},
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
            ViewBag.Authors = authorRes.Select(x =>
                new SelectListItem() { Text = x.AuthorName, Value = x.AuthorId.ToString() }
            ).ToList(); // Convert the IEnumerable to List<SelectListItem>

            return View();
        }

        [Route("[action]")]
        [HttpPost] // Action method only accepts POST requets
        public IActionResult Create(BookAddRequest book)
        {
            if (!ModelState.IsValid)
            {
                List<AuthorResponse> authorRes = _authorsService.GetAllAuthors();
                List<string> availableGenres = GetGenres();

                ViewBag.AvailableGenres = availableGenres;
                ViewBag.Authors = authorRes.Select(x =>
                    new SelectListItem() { Text = x.AuthorName, Value = x.AuthorId.ToString() }
                ).ToList(); // Convert the IEnumerable to List<SelectListItem>
                ViewBag.Errors = ModelState.Values.SelectMany(n => n.Errors).Select(e => e.ErrorMessage).ToList();

                return View();
            }

            BookResponse bookRes = _booksService.AddBook(book);
            
            return RedirectToAction("Index", "Books");
        }

        [HttpGet] // loading create view
        [Route("[action]/{bookId}")]
        public IActionResult Edit(Guid? bookId) // Edit.cshtml
        {
            List<AuthorResponse> authorRes = _authorsService.GetAllAuthors();
            List<string> availableGenres = GetGenres();

            ViewBag.AvailableGenres = availableGenres;
            ViewBag.Authors = authorRes.Select(x =>
                new SelectListItem() { Text = x.AuthorName, Value = x.AuthorId.ToString() }
            ).ToList(); // Convert the IEnumerable to List<SelectListItem>

            BookResponse? bookRes = _booksService.GetBookById(bookId);
            if (bookRes == null) return RedirectToAction("Index"); // if no valid book response return to index.cshtml

            BookUpdateRequest bookUpdateReq = bookRes.ToBookUpdateRequest();

            return View(bookUpdateReq);
        }

        [HttpPost] // handling submit for post - edit.cshtml
        [Route("[action]/{bookId}")]
        public IActionResult Edit(BookUpdateRequest bookUpdateReq, Guid bookId) // Edit.cshtml
        {
            BookResponse? bookRes = _booksService.GetBookById(bookUpdateReq.BookId);
            if (bookRes == null) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                BookResponse bookUpdated = _booksService.UpdateBook(bookUpdateReq);
                return RedirectToAction("Index");
            }
            else
            {
                List<AuthorResponse> authorRes = _authorsService.GetAllAuthors();
                List<string> availableGenres = GetGenres();

                ViewBag.AvailableGenres = availableGenres;
                ViewBag.Authors = authorRes.Select(x =>
                    new SelectListItem() { Text = x.AuthorName, Value = x.AuthorId.ToString() }
                ).ToList(); // Convert the IEnumerable to List<SelectListItem>
                ViewBag.Errors = ModelState.Values.SelectMany(n => n.Errors).Select(e => e.ErrorMessage).ToList();

                return View(bookRes.ToBookUpdateRequest());
            }
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

        [HttpGet]
        [Route("[action]/{bookId}")]
        public IActionResult Delete(Guid? bookId)
        {
            BookResponse? booksRes = _booksService.GetBookById(bookId);
            if (booksRes == null) return RedirectToAction("index");

            return View(booksRes);
        }

        [HttpPost]
        [Route("[action]/{bookId}")]
        public IActionResult Delete(BookUpdateRequest bookUpdateReq)
        {
            BookResponse? bookRes = _booksService.GetBookById(bookUpdateReq.BookId);
            if (bookRes == null) return RedirectToAction("index");

            _booksService.DeleteBook(bookUpdateReq.BookId);

            return RedirectToAction("index");
        }
    }
}
