using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadWorldEU.Meishi.Migrations;

/// <inheritdoc />
public partial class _20261115173730_AddLanguageToCurriculaVitae : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Languages",
            table: "CurriculaVitae",
            type: "jsonb",
            nullable: false,
            defaultValue: "[]");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Languages",
            table: "CurriculaVitae");
    }
}
