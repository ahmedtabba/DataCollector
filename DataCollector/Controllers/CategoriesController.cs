using DataCollector.Models.Api.Category;
using DataCollector.Models.Api.Identity;
using DataCollector.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataCollector.Extensions;
using DataCollector.Models.Identity;
using DataCollector.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DataCollector.Utilities;
using DataCollector.Dtos.Category;

namespace DataCollector.Controllers
{
    [ApiController]
    public class CategoriesController : BaseController
    {
        readonly private ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        [Route(ApiRoutes.Category.GetAllCategories)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Get([FromQuery] GetAllCategoriesRequestParams filter)
        {
            try
            {
                var query = _categoryService.GetAll();

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    query = query.Where(x => x.Name.Contains(filter.SearchText) || x.Code.Contains(filter.SearchText));

                if (!string.IsNullOrWhiteSpace(filter.Sort))
                    query = query.OrderBy(filter.Sort!);


                var pagedData = query.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                var resList = pagedData.ToList();

                if (filter.PrevId is not null && !resList.Where(x => x.Id == filter.PrevId.Value).Any())
                {
                    if (!string.IsNullOrWhiteSpace(filter.SearchText))
                        resList.AddRange(_categoryService.GetAll().Where(x => x.Id == filter.PrevId.Value && (x.Name.Contains(filter.SearchText) || x.Code.Contains(filter.SearchText))));
                    else
                        resList.AddRange(_categoryService.GetAll().Where(x => x.Id == filter.PrevId.Value));

                }

                var totalRecords = query.Count();
                var totalPages = Math.Ceiling((decimal)totalRecords / filter.PageSize);

                return Ok(new PagedResponse<List<CategoryDto>>(resList, filter.PageNumber, filter.PageSize)
                { TotalRecords = totalRecords, TotalPages = (int)totalPages });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route(ApiRoutes.Category.GetCategoryById)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var _res = await _categoryService.GetById(id);

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
        [Route(ApiRoutes.Category.CreateCategory)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var res = await _categoryService.CreateAsync(dto);
                if (!res.result.Succeeded)
                    return BadRequest(res.result.Errors);

                var createdCategory = await _categoryService.GetById(res.id!.Value);
                return Ok(createdCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route(ApiRoutes.Category.UpdateCategory)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryDto dto, int id)
        {
            try
            {
                var _res = await _categoryService.Update(dto, id);
                if (!_res.Succeeded)
                    return BadRequest(_res.Errors);

                var createdCategory = await _categoryService.GetById(id);
                return Ok(createdCategory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route(ApiRoutes.Category.DeleteCategory)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var _res = await _categoryService.Delete(id);
                if (!_res.Succeeded)
                    return BadRequest(_res.Errors);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
