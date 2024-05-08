using DataCollector.Data;
using DataCollector.Dtos.Category;
using DataCollector.Models;
using DataCollector.Services.Interfaces;
using DataCollector.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DataCollector.Services
{

    public class CategoryService : ICategoryService
    {
        readonly private AppDbContext _context;
        readonly private IMapper _mapper;
        public CategoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<(Result result, int? id)> CreateAsync(CreateCategoryDto dto)
        {
            try
            {
                var obEntity = _mapper.Map<CreateCategoryDto, Category>(dto);
                var errors = ValidateObject(obEntity);

                if (errors.Any())
                    return (Result.Failure(errors), id: null);

                await _context.Database.BeginTransactionAsync();
                _context.Categories.Add(obEntity);

                // Ensure that EntityState is correctly set to Added
                _context.Entry(obEntity).State = EntityState.Added;

                await _context.SaveChangesAsync();
                await _context.Database.CurrentTransaction!.CommitAsync();
                return (Result.Success(), obEntity.Id);
            }
            catch
            {
                await _context.Database.CurrentTransaction!.RollbackAsync();
                throw;
            }
        }


        private List<string> ValidateObject(Category obEntity)
        {
            List<string> errors = new();
            if(string.IsNullOrWhiteSpace(obEntity.Name))
                errors.Add("Category Name is required");

            if (string.IsNullOrWhiteSpace(obEntity.Code))
                errors.Add("Category Code is required");

            if (_context.Categories.Any(x => x.Name == obEntity.Name && x.Id != obEntity.Id))
                errors.Add("Category Name Exists");

            if (_context.Categories.Any(x => x.Code == obEntity.Code && x.Id != obEntity.Id))
                errors.Add("Category Code Exists");

            return errors;
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                var obEntity = _context.Categories.FirstOrDefault(x => x.Id == id);

                if (obEntity is null)
                    return Result.Failure(new List<string>() { "Category not found" });

                //Todo:: Check if there are related stores
                if(_context.Stores.Where(x=>x.CategoryId== id).Any())
                    return Result.Failure(new List<string>() { "Can not delete this category,there are stores related with it!!" });

                _context.Categories.Remove(obEntity);

                await _context.SaveChangesAsync();
                await _context.Database.CurrentTransaction!.CommitAsync();

                return Result.Success();
            }
            catch
            {
                await _context.Database.CurrentTransaction!.RollbackAsync();
                throw;
            }

        }

        public IQueryable<CategoryDto> GetAll()
        {
            return _mapper.ProjectTo<CategoryDto>(_context.Categories);
        }

        public async Task<(Result result, CategoryDto? data)> GetById(int id)
        {
            var dto = await (_mapper.ProjectTo<CategoryDto>(_context.Categories)).FirstOrDefaultAsync(x => x.Id == id);
            if (dto is null)
                return (Result.Failure(new List<string>() { "Category not found" }), data: null);

            return (Result.Success(), data: dto);
        }

        public async Task<Result> Update(UpdateCategoryDto dto, int id)
        {
            try
            {
                var obEntity = _context.Categories.FirstOrDefault(x => x.Id == id);


                if (obEntity is null)
                    return await Task.FromResult(Result.Failure(new List<string>() { "Category not found" }));

                _mapper.Map<UpdateCategoryDto, Category>(dto, obEntity);
                var errors = ValidateObject(obEntity);

                if (errors.Any())
                    return await Task.FromResult(Result.Failure(errors));

                await _context.Database.BeginTransactionAsync();
                _context.Categories.Update(obEntity);
                await _context.SaveChangesAsync();
                await _context.Database.CurrentTransaction!.CommitAsync();
                return await Task.FromResult(Result.Success());
            }
            catch
            {
                await _context.Database.CurrentTransaction!.RollbackAsync();
                throw;
            }
        }
    }
}
