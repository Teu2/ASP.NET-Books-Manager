using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using Services;
using ServiceContracts.DTO;

namespace BooksApp.Controllers
{
    [Route("authors")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorsService _authorsService;

        public AuthorsController(IAuthorsService authorsService)
        {
            _authorsService = authorsService;
        }

        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFromExcel(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                ViewBag.ErrorMessage1 = "Please select an xlsx file";
                return View();
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage1 = "Unsupported file. 'xlsx' file is expected";
                return View();
            }

            int authorsInserted = await _authorsService.UploadAuthorsFromExcel(formFile);

            ViewBag.Message1 = $"{authorsInserted} authors Uploaded";
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadByFormInput(AuthorAddRequest authorAddRequest)
        {
            if (authorAddRequest.AuthorName == null) // check if user entered in an author
            {
                ViewBag.ErrorMessage2 = $"Please enter an author name";
                return View("UploadFromExcel");
            }

            // check is author already exists

            return RedirectToAction("UploadFromExcel", "Authors");

        }
    }
}
