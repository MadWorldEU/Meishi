using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadWorldEU.Meishi.Migrations;

/// <inheritdoc />
public partial class _20261115175832_AddHobbyToCurriculaVitae : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string[]>(
            name: "Hobbies",
            table: "CurriculaVitae",
            type: "text[]",
            nullable: false,
            defaultValue: new string[0]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Hobbies",
            table: "CurriculaVitae");
    }
}
