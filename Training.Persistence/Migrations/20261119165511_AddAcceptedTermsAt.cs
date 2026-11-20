using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Training.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcceptedTermsAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AcceptedTermsAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedTermsAt",
                table: "AspNetUsers");
        }
    }
}
