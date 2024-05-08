using DataCollector.Dtos.Category;
using DataCollector.Dtos.Store;
using DataCollector.Helpers;
using DataCollector.Models.Api.Category;
using DataCollector.Models.Api.Store;
using DataCollector.Models.Shared;
using DataCollector.Services;
using DataCollector.Services.Interfaces;
using DataCollector.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataCollector.Extensions;
using static DataCollector.Utilities.Enums;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static System.Net.WebRequestMethods;
using NuGet.ContentModel;

namespace DataCollector.Controllers
{
    [ApiController]
    public class StoresController : BaseController
    {
        readonly private IStoreService _storeService;
        private readonly string _domainName;


        public StoresController(IStoreService storeService, IConfiguration config)
        {
            _storeService = storeService;
            _domainName = "http://dev.data.boulevard.solutions";// config.GetValue<string>("Domain");
        }

        [HttpGet]
        [Route(ApiRoutes.Store.GetAllStores)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Get([FromQuery] GetAllStoresRequestParams filter)
        {
            try
            {
                var query = _storeService.GetAll();
                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    query = query.Where(x => x.Name.Contains(filter.SearchText) || (x.CollectorNote != null && x.CollectorNote.Contains(filter.SearchText)) || x.CreatorName.Contains(filter.SearchText));

                if (filter.CategoryId is not null)
                    query = query.Where(x => x.CategoryId == filter.CategoryId);

                if (filter.CreatedByCurrentUser.HasValue && filter.CreatedByCurrentUser.Value)
                {
                    var currentUserId = GetCurrentUser.UserId;
                    query = query.Where(x => x.CreatedBy == currentUserId);
                }

                if (!string.IsNullOrWhiteSpace(filter.Sort))
                    query = query.OrderBy(filter.Sort!);

                IQueryable<StoreDto> pagedData ;

                if (filter.PageSize != -1)
                    pagedData = query.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);
                else
                    pagedData = query;





                var totalRecords = query.Count();
                var totalPages = filter.PageSize != -1 ? Math.Ceiling((decimal)totalRecords / filter.PageSize) : 1;

                return Ok(new PagedResponse<List<StoreDto>>(pagedData.ToList(), filter.PageNumber, filter.PageSize)
                { TotalRecords = totalRecords, TotalPages = (int)totalPages });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route(ApiRoutes.Store.GetCreatedStoresCountByUser)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCreatedStoresCountByUser()
        {
            try
            {
                var userId = GetCurrentUser.UserId;
                var count = _storeService.GetAll().Where(x => x.CreatedBy == userId).Count();

                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route(ApiRoutes.Store.GetStoreById)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var _res = await _storeService.GetById(id);

                if (!_res.result.Succeeded)
                    return BadRequest(_res.result.Errors);

                return Ok(_res.data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(ApiRoutes.Store.CreateStore)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromForm] CreateStoreDto dto)
        {
            try
            {
                var dd = GetCurrentUser.UserId;
                var res = await _storeService.CreateAsync(dto);
                if (!res.result.Succeeded)
                    return BadRequest(res.result.Errors);

                var model = await _storeService.GetById(res.id!.Value);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut]
        [Route(ApiRoutes.Store.UpdateStore)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        public async Task<IActionResult> Update([FromForm] UpdateStoreDto dto, int id)
        {
            try
            {
                var res = await _storeService.UpdateAsync(dto, id);
                if (!res.Succeeded)
                    return BadRequest(res.Errors);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route(ApiRoutes.Store.DeleteStore)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var res = await _storeService.Delete(id);
                if (!res.Succeeded)
                    return BadRequest(res.Errors);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route(ApiRoutes.Store.AddCollectorNote)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddCollectorNote([FromBody] AddCollectorNote dto, int id)
        {
            try
            {
                var _res = await _storeService.SetNoteToStore(dto, id);
                if (!_res.Succeeded)
                    return BadRequest(_res.Errors);

                var obFromDB = await _storeService.GetById(id);
                return Ok(obFromDB);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route(ApiRoutes.Store.ExportToExcel)]
        public async Task<IActionResult> ExportStores([FromQuery] ExportStoresRequestParams filter)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var query = _storeService.GetAllForExport();

            if (filter.CategoryId is not null)
                query = query.Where(x => x.CategoryId == filter.CategoryId);

            if (!string.IsNullOrWhiteSpace(filter.CreatorId))
                query = query.Where(x => x.CreatedBy == filter.CreatorId);

            if (filter.From is not null)
                query = query.Where(x => x.CreationDate.Date >= filter.From.Value.Date);

            if (filter.To is not null)
                query = query.Where(x => x.CreationDate.Date <= filter.To.Value.Date);

            var stores = await query.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Stores");
                int row = 2; // Start from row 2 to leave space for headers

                // Define headers
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Category";
                worksheet.Cells[1, 3].Value = "Collector Note";
                worksheet.Cells[1, 4].Value = "Latitude";
                worksheet.Cells[1, 5].Value = "Longitude";
                worksheet.Cells[1, 6].Value = "Is There A Nearby Store";
                worksheet.Cells[1, 7].Value = "User";
                worksheet.Cells[1, 8].Value = "Assets URLs";

                foreach (var store in stores)
                {
                    // Add store data to the worksheet
                    worksheet.Cells[row, 1].Value = store.Name;
                    worksheet.Cells[row, 2].Value = store.CategoryName;
                    worksheet.Cells[row, 3].Value = store.CollectorNote;
                    worksheet.Cells[row, 4].Value = store.Latitude;
                    worksheet.Cells[row, 5].Value = store.Longitude;
                    worksheet.Cells[row, 6].Value = store.IsThereANearbyStore;
                    worksheet.Cells[row, 7].Value = store.CreatorName;
                    // Assuming Assets is a list of URLs
                    worksheet.Cells[row, 8].Value = string.Join(", ", store.Assets.Select(a => $"{_domainName}{a.AssetURL}"));


                    row++; // Move to the next row for the next store
                }

                // Auto-size columns
                worksheet.Cells[1, 1, stores.Count + 1, 8].AutoFitColumns();

                // Save the package to a MemoryStream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                var file = stream.ToArray();
                // Return the Excel file
                return File(file, "application/octet-stream", "stores.xlsx");
            }
        }

    }
}
