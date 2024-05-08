using DataCollector.Data;
using DataCollector.Dtos.Category;
using DataCollector.Dtos.Store;
using DataCollector.Helpers;
using DataCollector.Models;
using DataCollector.Services.Interfaces;
using DataCollector.Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace DataCollector.Services
{
    public class StoreService : IStoreService
    {
        readonly private AppDbContext _context;
        readonly private IMapper _mapper;
        public static IWebHostEnvironment _webHostEnvironment;

        public StoreService(AppDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(Result result, int? id)> CreateAsync(CreateStoreDto dto)
        {
            try
            {
                var obEntity = _mapper.Map<CreateStoreDto, Store>(dto);
                var errors = ValidateObject(obEntity, dto.Assets);

                if (errors.Any())
                    return (Result.Failure(errors), id: null);

                await _context.Database.BeginTransactionAsync();

                var nearbyStores = GetNearbyStores(obEntity.Latitude, obEntity.Longitude, 5d);


                if (nearbyStores.Any())
                    obEntity.IsThereANearbyStore = true;


                // Add the Store to the database first
                _context.Stores.Add(obEntity);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Now that the Store has been saved and has an Id, 
                // you can add the StorePhotos
                foreach (var asset in dto.Assets)
                {
                    string url = UploadPhoto(asset);

                    // Create a new StorePhoto object and add it to the store's Photos collection
                    obEntity.StorePhotos.Add(new StorePhoto
                    {
                        PhotoURL = url
                    });
                }

                // Save the changes to the database again
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

        public List<Store> GetNearbyStores(string newStoreLatitude, string newStoreLongitude, double distanceInMeters, int storeId = 0)
        {
            var nearbyStores = _context.Stores
                .AsEnumerable()
                .Where(store =>
                store.Id != storeId
                && !store.IsThereANearbyStore
                && Helper.CalculateDistance(newStoreLatitude, newStoreLongitude, store.Latitude, store.Longitude) <= distanceInMeters)
                .ToList();

            return nearbyStores;
        }





        private string UploadPhoto(IFormFile photos)
        {
            var url = string.Empty;
            if (photos?.Length > 0)
            {
                var path = _webHostEnvironment.WebRootPath + "\\" + "Uploads\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //TODO::Handel User Photo
                using (FileStream fileStream = System.IO.File.Create(path + photos.FileName))
                {
                    photos.CopyTo(fileStream);
                    fileStream.Flush();
                    if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
                    {
                        _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }
                    // Construct the URL for the uploaded file
                    url = $"/Uploads/{photos.FileName}";
                }
            }

            return url;
        }

        private IEnumerable<string> ValidateObject(Store obEntity, List<IFormFile> newAssets = null, List<int> oldAssets = null)
        {
            var errors = new List<string>();
            //Todo:Put Validation Logic

            if (string.IsNullOrWhiteSpace(obEntity.Name))
                errors.Add("Store name is required!!");

            //if (_context.Stores.Where(x => x.Name == obEntity.Name  && x.Id != obEntity.Id).Any())
            //    errors.Add("Store name is required!!");

            if ((newAssets is null || !newAssets.Any()) && (oldAssets is null || !oldAssets.Any()))
                    errors.Add("Add at least one photo!!");


            return errors;
        }

        public IQueryable<StoreDto> GetAll()
        {
            return _mapper.ProjectTo<StoreDto>(_context.Stores.Include(x => x.StorePhotos));
        }

        public IQueryable<StoreForExportDto> GetAllForExport()
        {
            return _mapper.ProjectTo<StoreForExportDto>(_context.Stores.Include(x => x.StorePhotos));
        }

        public async Task<(Result result, StoreDto? data)> GetById(int id)
        {
            var dto = await (_mapper.ProjectTo<StoreDto>(_context.Stores)).FirstOrDefaultAsync(x => x.Id == id);
            if (dto is null)
                return (Result.Failure(new List<string>() { "Store not found" }), data: null);

            return (Result.Success(), data: dto);
        }

        public async Task<Result> SetNoteToStore(AddCollectorNote note, int id)
        {
            try
            {
                var obEntity = _context.Stores.FirstOrDefault(x => x.Id == id);


                if (obEntity is null)
                    return await Task.FromResult(Result.Failure(new List<string>() { "Store not found" }));

                obEntity.CollectorNote = note.Note;

                await _context.Database.BeginTransactionAsync();
                _context.Stores.Update(obEntity);
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

        public async Task<Result> Delete(int id)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                var obEntity = _context.Stores.Include(x => x.StorePhotos).FirstOrDefault(x => x.Id == id);
                if (obEntity is null)
                    return Result.Failure(["Store Not Found!"]);

                _context.StorePhotos.RemoveRange(obEntity.StorePhotos);
                _context.Remove(obEntity);
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

        public async Task<Result> UpdateAsync(UpdateStoreDto dto, int id)
        {
            await _context.Database.BeginTransactionAsync();
            try
            {
                var obEntity = _context.Stores.Find(id);
                if (obEntity is null)
                    return Result.Failure(["Store not found!!"]);

                _mapper.Map<UpdateStoreDto, Store>(dto, obEntity);
                var errors = ValidateObject(obEntity, dto.NewAssets, dto.AssetsIds);

                if (errors.Any())
                    return Result.Failure(errors);


                var nearbyStores = GetNearbyStores(obEntity.Latitude, obEntity.Longitude, 5d, id);


                if (nearbyStores.Any())
                    obEntity.IsThereANearbyStore = true;

                // Add the Store to the database first
                _context.Stores.Update(obEntity);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                //get deleted assets from Db
                var deletedAssets = _context.StorePhotos.Where(x => x.StoreId == id && !dto.AssetsIds.Contains(x.Id)).ToList();

                _context.StorePhotos.RemoveRange(deletedAssets);
                await _context.SaveChangesAsync();

                // Now that the Store has been saved and has an Id, 
                // you can add the StorePhotos
                foreach (var asset in dto.NewAssets)
                {
                    string url = UploadPhoto(asset);

                    // Create a new StorePhoto object and add it to the store's Photos collection
                    obEntity.StorePhotos.Add(new StorePhoto
                    {
                        PhotoURL = url
                    });
                }

                // Save the changes to the database again
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

     
    }
}
