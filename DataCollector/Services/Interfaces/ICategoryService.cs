using DataCollector.Dtos.Category;
using DataCollector.Utilities;

namespace DataCollector.Services.Interfaces
{
    public interface ICategoryService
    {
        IQueryable<CategoryDto> GetAll();
        Task<(Result result, CategoryDto? data)> GetById(int id);
        Task<(Result result, int? id)> CreateAsync(CreateCategoryDto dto);
        Task<Result> Update(UpdateCategoryDto dto, int id);
        Task<Result> Delete(int id);
    }
}
