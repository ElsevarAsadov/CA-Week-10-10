using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Business.Exceptions;
using Pustok.Business.Extensions;
using Pustok.Models;
using Pustok.Business.Services.Interfaces;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IBookService _bookService;

        public BookController(PustokContext context, IWebHostEnvironment env, IBookService bookService)
        {
            _context = context;
            _env = env;
            _bookService = bookService;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllAsync();
            //var books = await _context.Books.Include(x => x.Author).Include(x => x.BookImages).Where(x => x.IsDeleted == false).ToListAsync();
            return View(books);
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await _bookService.CreateAsync(book);
            }
            catch (NotFoundException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (InvalidImageContentException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();


            Book existBook = await _bookService.GetByIdAsync(id);

            if (existBook == null) return NotFound();

            foreach (var item in existBook.BookTags)
            {
                existBook.TagIds.Add(item.TagId);
            }

            existBook.TagIds = existBook.BookTags.Select(x => x.TagId).ToList();

            return View(existBook);
        }

        [HttpPost]
        public IActionResult Update(Book book)
        {
            return Ok(book.BookImageIds);
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            if (!ModelState.IsValid)
            {
                return View();
            }

            Book existBook = _context.Books
                            .Include(x => x.BookTags)
                            .Include(x => x.BookImages)
                            .Include(x => x.BookTags).ThenInclude(x => x.Tag)
                            .FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null) return NotFound();


            var destination = existBook.GetType().GetProperties();
            var source = book.GetType().GetProperties();

            for (int i = 0; i < destination.Length; i++)
            {
                destination[i].SetValue(existBook, source[i].GetValue(book));
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre not found!");
                return View();
            }

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author not found!");
                return View();
            }


            existBook.BookTags.RemoveAll(bt => !book.TagIds.Contains(bt.TagId));
            foreach (var tagId in book.TagIds.Where(x => !existBook.BookTags.Any(bt => bt.TagId == x)))
            {
                BookTag bookTag = new BookTag
                {
                    Book = existBook,
                    TagId = tagId
                };
                existBook.BookTags.Add(bookTag);
            }


            if (book.BookPosterImageFile != null)
            {
                if (book.BookPosterImageFile.ContentType != "image/jpeg" && book.BookPosterImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("BookPosterImageFile", "File must be .png or .jpeg (.jpg)");
                    return View();
                }
                if (book.BookPosterImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("BookPosterImageFile", "File size must be lower than 2mb!");
                    return View();
                }

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/Books", book.BookPosterImageFile),
                    IsPoster = true
                };

                existBook.BookImages.Add(bookImage);
            }

            if (book.BookHoverImageFile != null)
            {
                if (book.BookHoverImageFile.ContentType != "image/jpeg" && book.BookHoverImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("BookHoverImageFile", "File must be .png or .jpeg (.jpg)");
                    return View();
                }
                if (book.BookHoverImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("BookHoverImageFile", "File size must be lower than 2mb!");
                    return View();
                }

                BookImage bookImage = new BookImage
                {
                    Book = book,
                    ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", book.BookHoverImageFile),
                    IsPoster = false
                };

                existBook.BookImages.Add(bookImage);
            }


            existBook.BookImages.RemoveAll(bi => !book.BookImageIds.Contains(bi.Id) && bi.IsPoster == null);
            if (book.ImageFiles != null)
            {
                foreach (var imageFile in book.ImageFiles)
                {
                    if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png")
                    {
                        ModelState.AddModelError("ImageFiles", "File must be .png or .jpeg (.jpg)");
                        return View();
                    }
                    if (imageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFiles", "File size must be lower than 2mb!");
                        return View();
                    }

                    BookImage bookImage = new BookImage
                    {
                        Book = book,
                        ImageUrl = Helper.SaveFile(_env.WebRootPath, "uploads/books", imageFile),
                        IsPoster = null
                    };

                    _context.BookImages.Add(bookImage);
                    existBook.BookImages.Add(bookImage);
                }
            }



            existBook.Name = book.Name;
            existBook.Description = book.Description;
            existBook.CostPrice = book.CostPrice;
            existBook.SalePrice = book.SalePrice;
            existBook.Code = book.Code;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.IsAvailable = book.IsAvailable;
            existBook.Tax = book.Tax;
            existBook.AuthorId = book.AuthorId;
            existBook.GenreId = book.GenreId;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
