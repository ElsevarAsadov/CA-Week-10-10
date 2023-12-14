using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pustok.Core.Models;
using Pustok.Models;

namespace Pustok.DAL;

public class PustokContext : IdentityDbContext
{
    public PustokContext(DbContextOptions<PustokContext> options) : base(options) { }

    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<BookTag> BookTags { get; set; }
    public DbSet<BookImage> BookImages { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<User> AppUsers { get; set; }
    public DbSet<BasketProduct> BasketItems { get; set; }
}
