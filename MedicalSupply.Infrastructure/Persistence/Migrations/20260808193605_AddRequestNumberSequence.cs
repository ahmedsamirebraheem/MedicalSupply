using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalSupply.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "RequestNumberSequence");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "RequestNumberSequence");
        }
    }
}
