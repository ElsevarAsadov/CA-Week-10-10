using Pustok.Models;

namespace Pustok.Core.Models;

public class BasketProduct : BaseEntity
{
    public string AppUserId { get; set; }
    public int BookId { get; set; }
    public int Count { get; set; }

    public Book Book { get; set; }
    public User User { get; set; }
}
