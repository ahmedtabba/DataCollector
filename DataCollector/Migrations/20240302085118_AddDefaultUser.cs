using DataCollector.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollector.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //var hasher = new PasswordHasher<ApplicationUser>();
            //var user = new ApplicationUser
            //{
            //    UserName = "admin@admin.com",
            //    Email = "admin@admin.com",
            //    EmailConfirmed = true,
            //    PasswordHash = hasher.HashPassword(null, "Admin=123"),
            //    SecurityStamp = Guid.NewGuid().ToString()
            //};

            //migrationBuilder.InsertData(
            //    table: "AspNetUsers",
            //    columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount" },
            //    values: new object[] { user.Id, user.UserName, user.NormalizedUserName, user.Email, user.NormalizedEmail, user.EmailConfirmed, user.PasswordHash, user.SecurityStamp, Guid.NewGuid().ToString(), null, false, false, null, false, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


        }
    }
}
