using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FondsSocial.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaireMensuelToAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SalaireMensuel",
                table: "Agents",
                type: "decimal(18,3)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaireMensuel",
                table: "Agents");
        }
    }
}
