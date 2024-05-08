using DataCollector.Identity.AppContext;
using DataCollector.Identity;
using DataCollector.Models.Identity;
using DataCollector.Models.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DataCollector.Data;
using System.Security.Cryptography;
using DataCollector.Services.Interfaces;

namespace DataCollector.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, AppDbContext context,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User with this email does not exists!" }
                    };
                }

                if (!user.IsActive)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "Your account is currently inactive. Please contact our support team for assistance." }
                    };
                }

                var userHavealidPassword = await _userManager.CheckPasswordAsync(user, password);

                if (!userHavealidPassword)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User/password combination is wrong!" }
                    };
                }

                if (string.IsNullOrWhiteSpace(user.TokenVersion))
                    user.TokenVersion = Guid.NewGuid().ToString();

                await _userManager.UpdateAsync(user);

                return await GenerateAuthenticationResultForUser(user);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User with this email is already exists!" }
                    };
                }

                var newUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FullName = model.Email,
                    EmailConfirmed = true,
                    PhoneNumber = model.PhoneNumber,
                    Target = model.Target,
                    IsActive = true,
                    UserRole = (int)model.UserRole,
                    Language = model.Language ?? "en",
                    TokenVersion = Guid.NewGuid().ToString()
                };

                //List<string> userGroups = model.GroupIds.ToList();
                //foreach (string uGroup in userGroups)
                //    newUser.Groups.Add(new ApplicationUserGroup() { ApplicationGroupId = uGroup, ApplicationUserId = newUser.Id });

                var createdUser = await _userManager.CreateAsync(newUser, model.Password);


                if (createdUser.Succeeded)
                {
                    // Check if the role exists
                    var roleExists = await _roleManager.RoleExistsAsync(model.UserRole.ToString());

                    // If the role doesn't exist, create it
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(model.UserRole.ToString()));
                    }

                    // Add the user to the role
                    await _userManager.AddToRoleAsync(newUser, model.UserRole.ToString());
                }
                else
                {
                    return new AuthenticationResult
                    {
                        Errors = createdUser.Errors.Select(x => x.Description).ToList()
                    };
                }

                return await GenerateAuthenticationResultForUser(newUser);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUser(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("AAAidSecurityCode20_WithHappy_AS");


            //byte[] keyBytes = new byte[32]; // 256 bits
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(keyBytes);
            //}
            var keySecret = Convert.ToBase64String(key);


            if (ValidateCurrentToken(user.Token))
            {
                return new AuthenticationResult
                {
                    Success = true,
                    Token = user.Token
                };
            }



            var claimsIdentity = new List<Claim>()
            {
                new Claim(type:JwtRegisteredClaimNames.Sub,value:user.Email),
                        new Claim(type:JwtRegisteredClaimNames.Jti,value:Guid.NewGuid().ToString()),
                        new Claim(type:JwtRegisteredClaimNames.Email,value:user.Email),
                        new Claim(type:"PhoneNumber",value:user.PhoneNumber==null?"":user.PhoneNumber),
                        new Claim(type:"id",value:user.Id),
                        new Claim(type:"PhotoURL",value:user.PhotoURL?.ToString()??""),
                        new Claim(type:"Target",value:user.Target?.ToString()??""),
                        new Claim(type:"FullName",value:user.FullName??""),
                        new Claim(type:"IsActive",value:user.IsActive.ToString()??""),
                        new Claim(type:"UserRole",value:user.UserRole.ToString()??""),
                        new Claim(type:"Language",value:user.Language??"en"),
                        new Claim(type:"TokenVersion",value:user.TokenVersion??""),
                        new Claim(type:JwtRegisteredClaimNames.Name,value:user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claimsIdentity.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimsIdentity),
                Expires = DateTime.UtcNow.AddHours(720),
                Audience = "",
                Issuer = "",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            user.Token = tokenHandler.WriteToken(token);
            //user.TokenVersion = Guid.NewGuid().ToString();



            await _userManager.UpdateAsync(user);

            return new AuthenticationResult
            {
                Success = true,
                Token = user.Token
            };
        }

        public async Task<AuthenticationResult> ResetPasswordAsync(string email, string password, string userId)
        {
            ApplicationUser cUser = await _userManager.FindByIdAsync(userId);
            var token = await _userManager.
            GeneratePasswordResetTokenAsync(cUser);
            var result = await _userManager.ResetPasswordAsync(cUser, token, password);
            if (result.Succeeded)
            {
                cUser.TokenVersion = Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(cUser);
            }
            return await GenerateAuthenticationResultForUser(cUser);

        }


        private bool ValidateCurrentToken(string token)
        {
            //var key = Encoding.UTF8.GetBytes("AAAidSecurityCode20_WithHappy");
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("AAAidSecurityCode20_WithHappy"));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = "",
                    ValidAudience = "",
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task<RequestResult> DeleteAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new RequestResult
                    {
                        Errors = new[] { "User is not exists!" }
                    };
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in userRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role!);
                }

                var res = await _userManager.DeleteAsync(user);

                return new RequestResult
                {
                    Success = true
                };

            }
            catch
            {

                throw;
            }
        }

        public async Task<RequestResult> UpdateUserAsync(EditUserVM model, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new RequestResult
                    {
                        Errors = new[] { "User is not exists!" }
                    };

                if (user.Email != model.Email || !model.IsActive)
                    user.TokenVersion = Guid.NewGuid().ToString();

                user.Email = model.Email;
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.HasPhoto = model.HasPhoto;
                user.PhotoURL = model.PhotoURL;
                user.UserName = model.Email;
                user.Target = model.Target;
                user.IsActive = model.IsActive;
                user.UserRole = (int)model.UserRole;
                user.Language = model.Language;

                var res = await _userManager.UpdateAsync(user);

                if (!res.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = res.Errors.Select(x => x.Description).ToList()
                    };
                }


                #region Remove From Old Roles
                var userRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in userRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role!);
                }
                #endregion

                #region Add To New Roles
                // Check if the role exists
                var roleExists = await _roleManager.RoleExistsAsync(model.UserRole.ToString());

                // If the role doesn't exist, create it
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new ApplicationRole(model.UserRole.ToString()));
                }

                // Add the user to the role
                await _userManager.AddToRoleAsync(user, model.UserRole.ToString());
                #endregion

                return new RequestResult
                {
                    Success = true
                };

            }
            catch
            {

                throw;
            }
        }

        public async Task<AuthenticationResult> CreateUserAsync(AddUserViewModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User with this email is already exists!" }
                    };
                }

                var newUser = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    EmailConfirmed = true,
                    FullName = model.FullName,
                    NormalizedEmail = model.Email,
                    NormalizedUserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    HasPhoto = model.HasPhoto.HasValue ? model.HasPhoto.Value : false,
                    PhotoURL = model.PhotoURL,
                    IsActive = model.IsActive,
                    Target = model.Target,
                    UserRole = (int)model.UserRole,
                    Language = model.Language ?? "en",
                    TokenVersion = Guid.NewGuid().ToString(),
                };

                var createdUser = await _userManager.CreateAsync(newUser, model.Password);

                var user = await _userManager.FindByEmailAsync(model.Email);


                #region Add To New Roles
                // Check if the role exists
                var roleExists = await _roleManager.RoleExistsAsync(model.UserRole.ToString());

                // If the role doesn't exist, create it
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new ApplicationRole(model.UserRole.ToString()));
                }

                // Add the user to the role
                await _userManager.AddToRoleAsync(user, model.UserRole.ToString());
                #endregion


                if (!createdUser.Succeeded)
                {
                    return new AuthenticationResult
                    {
                        Errors = createdUser.Errors.Select(x => x.Description).ToList()
                    };
                }

                return await GenerateAuthenticationResultForUser(newUser);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public ApplicationUser? GetUserBtId(string userId)
        {
            return _userManager.Users.Where(x => x.Id == userId).FirstOrDefault();
        }
    }
}
