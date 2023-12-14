using Humanizer.Localisation;
using Pustok.Models;
using Pustok.Repositories.Interfaces;
using Pustok.Business.Services.Interfaces;

namespace Pustok.Business.Services.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly IBookGenreRepository _bookGenreRepository;

        public GenreService(IBookGenreRepository bookGenreRepository)
        {
            _bookGenreRepository = bookGenreRepository;
        }

        public async Task CreateAsync(Genre entity)
        {
            if (_bookGenreRepository.Table.Any(x => x.Name == entity.Name))
                throw new NullReferenceException();

            await _bookGenreRepository.CreateAsync(entity);
            await _bookGenreRepository.CommitAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _bookGenreRepository.GetByIdAsync(x=>x.Id == id && x.IsDeleted == false);
            if (entity is null) throw new NullReferenceException();

            _bookGenreRepository.Delete(entity);
            await _bookGenreRepository.CommitAsync(); 
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _bookGenreRepository.GetAllAsync();
        }

        public async Task<Genre> GetByIdAsync(int id)
        {
            var entity = await _bookGenreRepository.GetByIdAsync(x => x.Id == id && x.IsDeleted == false);
            if (entity is null) throw new NullReferenceException();
            return entity;
        }

        public async Task UpdateAsync(Genre genre)
        {
            var existEntity = await _bookGenreRepository.GetByIdAsync(x => x.Id == genre.Id && x.IsDeleted == false);

            if (_bookGenreRepository.Table.Any(x => x.Name == genre.Name && existEntity.Id != genre.Id))
                throw new NullReferenceException();

            existEntity.Name = genre.Name;
            await _bookGenreRepository.CommitAsync();
        }
    }
}
