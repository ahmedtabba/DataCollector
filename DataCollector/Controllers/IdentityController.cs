using DataCollector.Identity;
using DataCollector.Models.Api.Identity;
using DataCollector.Models.Shared;
using DataCollector.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DataCollector.Extensions;
using DataCollector.Models.Identity;
using DataCollector.Services.Interfaces;
using static DataCollector.Utilities.Enums;
using Microsoft.AspNetCore.Cors;

namespace DataCollector.Controllers
{
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _IdentityService;
        private readonly UserManager<ApplicationUser> _userManager;
        public static IWebHostEnvironment _webHostEnvironment;
        public IdentityController(IIdentityService IdentityService, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _IdentityService = IdentityService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var authResponse = await _IdentityService.RegisterAsync(model);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }


        [HttpPost(ApiRoutes.Identity.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> CreateUserByAdmin([FromForm] AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            if (model.Photo?.Length > 0)
            {
                var path = _webHostEnvironment.WebRootPath + "\\" + "Uploads\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //TODO::Handel User Photo
                using (FileStream fileStream = System.IO.File.Create(path + model.Photo.FileName))
                {
                    model.Photo.CopyTo(fileStream);
                    fileStream.Flush();
                    model.HasPhoto = true;

                    // Construct the URL for the uploaded file
                    string url = _webHostEnvironment.WebRootPath.Replace("\\", "/") + "/Uploads/" + model.Photo.FileName;
                    model.PhotoURL = url;
                }
            }


            var authResponse = await _IdentityService.CreateUserAsync(model);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new RequestResult { Success = true });
        }

        [HttpGet(ApiRoutes.Identity.GetAll)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult GetAll([FromQuery] IdentityRequestParams filter)
        {
            var query = _userManager.Users;

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
                query = query.Where(x => x.UserName.Contains(filter.SearchText) || x.FullName.Contains(filter.SearchText));

            query = query.OrderBy(filter.Sort);


            var pagedData = query.Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var totalRecords = query.Count();
            var totalPages = Math.Ceiling((decimal)totalRecords / filter.PageSize);



            return Ok(new PagedResponse<List<ApplicationUserResponsVM>>(pagedData.Select(x => new ApplicationUserResponsVM
            {
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                FullName = x.FullName,
                HasPhoto = x.HasPhoto,
                PhotoURL = x.PhotoURL,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                UserName = x.UserName,
                LockoutEnabled = x.LockoutEnabled,
                LockoutEnd = x.LockoutEnd,
                Target = x.Target,
                IsActive = x.IsActive,
                UserRole = (UserRole)x.UserRole,
                Language = x.Language
            }).ToList(), filter.PageNumber, filter.PageSize)
            { TotalRecords = totalRecords, TotalPages = (int)totalPages });
        }

        [HttpGet(ApiRoutes.Identity.Get)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableCors("AllowSpecificOrigin")]
        public IActionResult Get([FromRoute] string userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new RequestResult
                {
                    Errors = new[] { "Invalid userId" }
                });

            var user = _userManager.Users.Where(x => x.Id == userId);

            if (!user.Any())
                return BadRequest(new RequestResult
                {
                    Errors = new[] { "User is not exists!" }
                });



            return Ok(user.Select(x => new ApplicationUserResponsVM
            {
                Email = x.Email,
                EmailConfirmed = x.EmailConfirmed,
                FullName = x.FullName,
                HasPhoto = x.HasPhoto,
                PhotoURL = x.PhotoURL,
                Id = x.Id,
                PhoneNumber = x.PhoneNumber,
                UserName = x.UserName,
                LockoutEnabled = x.LockoutEnabled,
                LockoutEnd = x.LockoutEnd,
                Target = x.Target,
                IsActive = x.IsActive,
                UserRole = (UserRole)x.UserRole,
                Language = x.Language
            }).FirstOrDefault());
        }


        [HttpPut(ApiRoutes.Identity.Update)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Edit([FromForm] EditUserRequestVM model, [FromRoute] string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest(new RequestResult
                {
                    Errors = new[] { "User is not exists!" }
                });

            EditUserVM userVM = new EditUserVM
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                HasPhoto = user.HasPhoto,
                PhotoURL = user.PhotoURL,
                Target = model.Target,
                IsActive = model.IsActive,
                Language = model.Language,
                UserRole = model.UserRole
            };


            //TODO::Handel Photo Upload
            if (model.Photo?.Length > 0)
            {
                var path = _webHostEnvironment.WebRootPath + "\\" + "Uploads\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (FileStream fileStream = System.IO.File.Create(path + model.Photo.FileName))
                {
                    model.Photo.CopyTo(fileStream);
                    fileStream.Flush();
                    if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
                    {
                        _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }
                    userVM.HasPhoto = true;

                    userVM.PhotoURL = $"/Uploads/{model.Photo.FileName}";
                }
            }



            var response = await _IdentityService.UpdateUserAsync(userVM, userId);

            if (!response.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = response.Errors
                });
            }

            return Ok(new RequestResult { Success = true });
        }


        [HttpPut(ApiRoutes.Identity.UpdateMyProfile)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> UpdateCurrentUserProfile([FromForm] EditCurrentUserProfileRequestVM model)
        {
            var userId = GetCurrentUser.UserId;
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest(new RequestResult
                {
                    Errors = new[] { "User  is not exists!" }
                });



            EditUserVM userVM = new EditUserVM
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                HasPhoto = user.HasPhoto,
                PhotoURL = user.PhotoURL,
                Target = user.Target,
                Language = model.Language
            };


            //TODO::Handel Photo Upload
            if (model.Photo?.Length > 0)
            {
                var path = _webHostEnvironment.WebRootPath + "\\" + "Uploads\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (FileStream fileStream = System.IO.File.Create(path + model.Photo.FileName))
                {
                    model.Photo.CopyTo(fileStream);
                    fileStream.Flush();
                    if (string.IsNullOrWhiteSpace(_webHostEnvironment.WebRootPath))
                    {
                        _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }
                    userVM.HasPhoto = true;

                    userVM.PhotoURL = $"/Uploads/{model.Photo.FileName}";
                }
            }



            var response = await _IdentityService.UpdateUserAsync(userVM, userId);

            if (!response.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = response.Errors
                });
            }

            return Ok(new RequestResult { Success = true });
        }




        [HttpPost(ApiRoutes.Identity.Login)]
        [EnableCors("AllowSpecificOrigin")]
        //[AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var authResponse = await _IdentityService.LoginAsync(model.Email, model.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [HttpDelete(ApiRoutes.Identity.Delete)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Delete([FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { "userId is not valid!!" }
                });

            }

            if (GetCurrentUser.UserId == userId)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { "User can not delete him self!!" }
                });
            }

            var response = await _IdentityService.DeleteAsync(userId);

            if (!response.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = response.Errors
                });
            }

            return Ok(response);
        }



        [HttpPost(ApiRoutes.Identity.ResetMyPassword)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> ResetMyPassword([FromBody] MyPasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }



            var authResponse = await _IdentityService.ResetPasswordAsync(GetCurrentUser.Email, model.Password, GetCurrentUser.UserId);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }



        [HttpPost(ApiRoutes.Identity.ResetPassword)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Admin"*/)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                });

            }

            var user = await _userManager.FindByEmailAsync(model.Email);


            var authResponse = await _IdentityService.ResetPasswordAsync(model.Email, model.Password, user.Id);

            if (!authResponse.Success)
            {
                return BadRequest(new FailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }

        [HttpGet(ApiRoutes.Identity.GetMyProfile)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> GetMyProfile()
        {

            var userId = GetCurrentUser.UserId;

            var user = _IdentityService.GetUserBtId(userId);


            var res = new UserProfileVM
            {
                Email = user.Email,
                UserRole = ((UserRole)Convert.ToInt32(user.UserRole)).ToString(),
                UserName = user.UserName,
                FullName = user.FullName,
                Target = user.Target ?? 0,
                Language = user.Language ?? "en"
            };



            return Ok(res);
        }
    }
}
