using Pustok.DAL;
using Pustok.Models;
using Pustok.Repositories.Interfaces;

namespace Pustok.Data.Repositories.Implementations
{
    public class BookGenreRepository : GenericRepository<Genre>, IBookGenreRepository
    {
        public BookGenreRepository(PustokContext context) : base(context)
        {
        }
    }
}
