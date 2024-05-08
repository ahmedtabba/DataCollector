using DataCollector.Dtos;
using DataCollector.Dtos.Store;
using DataCollector.Utilities;

namespace DataCollector.Services.Interfaces
{
    public interface IStoreService
    {
        IQueryable<StoreDto> GetAll();
        IQueryable<StoreForExportDto> GetAllForExport();
        Task<(Result result, StoreDto? data)> GetById(int id);
        Task<(Result result, int? id)> CreateAsync(CreateStoreDto dto);
        //Task<Result> Update(UpdateCategoryDto dto, int id);
        Task<Result> SetNoteToStore(AddCollectorNote note, int id);
        Task<Result> Delete(int id);
        Task<Result> UpdateAsync(UpdateStoreDto dto, int id);
        //Task<Result> Delete(int id);
    }
}
