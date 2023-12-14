using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;
using Pustok.DAL;
using Pustok.Models;
using Pustok.Repositories.Interfaces;
using Pustok.ViewModels;

namespace Pustok.Controllers;

public class ProductController : Controller
{
    private readonly IBookService _bookService;
	private readonly UserManager<User> _userManager;
	private readonly PustokContext _context;
	private readonly IBookRepository _bookRepository;

    public ProductController(
                        IBookRepository bookRepository, 
                        IBookService bookService,
                        UserManager<User> userManager,
                        PustokContext context)
    {
        _bookRepository = bookRepository;
        _bookService = bookService;
		_userManager = userManager;
		_context = context;
	}
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Detail(int id)
    {
        Book book = await _bookService.GetByIdAsync(id);
        ProductDetailViewModel productDetailViewModel = new ProductDetailViewModel()
        {
            Book = book,
            RelatedBooks = await _bookService.GetAllRelatedBooksAsync(book)
        };

        return View(productDetailViewModel);
    }

    public async Task<IActionResult> GetBookModal(int id)
    {
        var book = await _bookService.GetByIdAsync(id);

        return PartialView("_BookModalPartial",book);
    }

    //public IActionResult SetSession(string name)
    //{
    //    HttpContext.Session.SetString("UserName", name);

    //    return Content("Added to session");
    //}

    //public IActionResult GetSession()
    //{
    //    string username = HttpContext.Session.GetString("UserName");

    //    return Content(username);
    //}

    //public IActionResult RemoveSession()
    //{
    //    HttpContext.Session.Remove("UserName");

    //    return RedirectToAction("GetSession");
    //}

    //public IActionResult SetCookie(int id)
    //{
    //    List<int> ids = new List<int>();

    //    string idsStr = HttpContext.Request.Cookies["UserId"];

    //    if(idsStr is not null)
    //    {
    //        ids = JsonConvert.DeserializeObject<List<int>>(idsStr);
    //    }

    //    ids.Add(id);

    //    idsStr = JsonConvert.SerializeObject(ids);

    //    HttpContext.Response.Cookies.Append("UserId", idsStr);

    //    return Content("Added to cookie");
    //}

    //public IActionResult GetCookie()
    //{
    //    List<int> ids = new List<int>();

    //    string idsStr = HttpContext.Request.Cookies["UserId"];
    //    if(idsStr is not null)
    //        ids = JsonConvert.DeserializeObject<List<int>>(idsStr);


    //    return Json(ids);
    //}


    public async Task<IActionResult> AddToBasket(int bookId)
    {

        if (!_bookRepository.Table.Any(x => x.Id == bookId)) return NotFound(); // 404

        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
        BasketItemViewModel basketItem = null;
        BasketProduct userBasketProduct = null;
        User user = null;

        if (HttpContext.User.Identity.IsAuthenticated)
        {
            user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        }

        if (user == null)
        {
			string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

			if (basketItemListStr != null)
			{
				basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

				basketItem = basketItemList.FirstOrDefault(x => x.BookId == bookId);

				if (basketItem != null)
				{
					basketItem.Count++;
				}
				else
				{
					basketItem = new BasketItemViewModel()
					{
						BookId = bookId,
						Count = 1
					};

					basketItemList.Add(basketItem);
				}
			}
			else
			{
				basketItem = new BasketItemViewModel()
				{
					BookId = bookId,
					Count = 1
				};

				basketItemList.Add(basketItem);
			}

			basketItemListStr = JsonConvert.SerializeObject(basketItemList);

			HttpContext.Response.Cookies.Append("BasketItems", basketItemListStr);
		}
        else
        {
            userBasketProduct = await _context.BasketItems.FirstOrDefaultAsync(x => x.BookId == bookId && x.AppUserId == user.Id);
            if (userBasketProduct != null)
            {
                userBasketProduct.Count++;
            }
            else
            {
                userBasketProduct = new BasketProduct
                {
                    BookId = bookId,
                    Count = 1,
                    AppUserId = user.Id,
                    IsDeleted = false
                };
                _context.BasketItems.Add(userBasketProduct);
            }
            await _context.SaveChangesAsync();
        }

        return Ok(); //200
    }

    public IActionResult GetBasketItems()
    {
        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();

        string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];

        if (basketItemListStr != null)
        {
            basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);
        }

        return Json(basketItemList);
    }

    public async Task<IActionResult> Checkout()
    {
        List<CheckoutViewModel> checkoutItemList = new List<CheckoutViewModel>();
        List<BasketItemViewModel> basketItemList = new List<BasketItemViewModel>();
        List<BasketProduct> userBasketItems = new List<BasketProduct>();
        CheckoutViewModel checkoutItem = null;
		User user = null;

		if (HttpContext.User.Identity.IsAuthenticated)
		{
			user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
		}


		if(user == null)
        {
			string basketItemListStr = HttpContext.Request.Cookies["BasketItems"];
			if (basketItemListStr != null)
			{
				basketItemList = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemListStr);

				foreach (var item in basketItemList)
				{
					checkoutItem = new CheckoutViewModel
					{
						Book = await _bookRepository.GetByIdAsync(x => x.Id == item.BookId),
						Count = item.Count
					};
					checkoutItemList.Add(checkoutItem);
				}
			}
        }
        else
        {
            userBasketItems = await _context.BasketItems.Include(x=>x.Book).Where(x=> x.AppUserId == user.Id).ToListAsync();

            foreach (var item in userBasketItems)
            {
                checkoutItem = new CheckoutViewModel
                {
                    Book = item.Book,
                    Count = item.Count
                };
                checkoutItemList.Add(checkoutItem);
            }
        }

        return View(checkoutItemList);
    }

    public async Task<IActionResult> SearchBooks(string value)
    {
        List<Book> searchedBooks = await _bookService.GetAllAsync(x=>x.Name.ToLower().Contains(value.Trim().ToLower()));

        return Ok(searchedBooks);
    }

}
