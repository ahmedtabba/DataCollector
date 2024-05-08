using DataCollector.Data;
using DataCollector.Data.Interceptors;
using DataCollector.Filters;
using DataCollector.Identity;
using DataCollector.Identity.AppContext;
using DataCollector.Middlewares;
using DataCollector.Services;
using DataCollector.Services.Interfaces;
using DataCollector.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;
using static DataCollector.Utilities.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

var builder = WebApplication.CreateBuilder(args);


// 👇 Add the authorization services
builder.Services.AddAuthorization();


var key = Encoding.UTF8.GetBytes("AAAidSecurityCode20_WithHappy_AS");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                        var userId = context.Principal.Identity.Name;
                        var user = userService.FindByIdAsync(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            });


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IApplicationContext, ApplicationContext>();
//builder.Services.AddSingleton<AuditableEntityInterceptor>();
builder.Services.AddScoped<AuditableEntityInterceptor>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});





//builder.Services
//    .AddIdentityApiEndpoints<ApplicationUser>()
//    .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("*")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
          .AddDefaultTokenProviders()
          .AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStoreService, StoreService>();

builder.Services.AddTransient<UserManager<ApplicationUser>>();
builder.Services.AddTransient<RoleManager<ApplicationRole>>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();


builder.Services.AddControllers(options =>
{
    options.Filters.Add<RequestInspector>();
}).AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});



var app = builder.Build();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseMiddleware<RequestPayloadLoggingMiddleware>();


app.UseRouting();
app.UseCors("AllowSpecificOrigin");
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<TokenVersionMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{

    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //db.Database.EnsureCreated();
    // db.Database.Migrate();
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        Console.WriteLine($"You have {pendingMigrations.Count()} pending migrations to apply.");
        Console.WriteLine("Applying pending migrations now");
        await context.Database.MigrateAsync();
    }

    var lastAppliedMigration = (await context.Database.GetAppliedMigrationsAsync()).Last();

    Console.WriteLine($"You're on schema version: {lastAppliedMigration}");

    

  // SeedingData(context);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    // Create a default admin user if it doesn't exist
    var defaultAdminEmail = "admin11@admin.com";
    var defaultAdminPassword = "Admin=123";

    if (userManager.FindByEmailAsync(defaultAdminEmail).Result == null/*!userManager.Users.Any()*/)
    {
        var adminUser = new ApplicationUser { UserName = defaultAdminEmail, Email = defaultAdminEmail,FullName=defaultAdminEmail, EmailConfirmed = true, IsActive = true, UserRole=(int)UserRole.Admin, Language ="en"};
   
        var result = userManager.CreateAsync(adminUser, defaultAdminPassword).Result;

        if (result.Succeeded)
        {
            // Check if the role exists
            var roleExists = await roleManager.RoleExistsAsync(adminUser.UserRole.ToString());

            // If the role doesn't exist, create it
            if (!roleExists)
            {
                await roleManager.CreateAsync(new ApplicationRole(UserRole.Admin.ToString()));
            }

            // Add the user to the role
            await userManager.AddToRoleAsync(adminUser, UserRole.Admin.ToString());
        }
    }
}



//app.MapGroup("/account").MapIdentityApi<ApplicationUser>();

app.Run();


//void SeedingData(AppDbContext context)
//{
//    if (context.Users.Any())
//        return;

//    var adminRole=context.Roles.FirstOrDefault(x=>x.Name==UserRole.Admin.ToString());
//    if(adminRole is null)
//    {
       
//    }
//}

