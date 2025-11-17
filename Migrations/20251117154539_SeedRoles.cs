using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicAndMind2.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
    table: "AspNetRoles",
    columns: new[] { "Id", "Name", "NormalizedName" },
    values: new object[,]
    {
        { Guid.NewGuid().ToString(), "Admin", "ADMIN" },
        { Guid.NewGuid().ToString(), "Moderator", "MODERATOR" },
        { Guid.NewGuid().ToString(), "Customer", "CUSTOMER" }
    }
);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
