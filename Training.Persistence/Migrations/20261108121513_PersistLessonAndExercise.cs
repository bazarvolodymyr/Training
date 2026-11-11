using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Training.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PersistLessonAndExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Exercises_LessonId",
                table: "Exercises");

            migrationBuilder.RenameColumn(
                name: "Question",
                table: "Exercises",
                newName: "Source");

            migrationBuilder.AddColumn<string>(
                name: "ArticleHtml",
                table: "Lessons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Lessons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Lessons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SourceLang",
                table: "Lessons",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetLang",
                table: "Lessons",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExerciseModes",
                columns: table => new
                {
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseModes", x => new { x.ExerciseId, x.Difficulty });
                    table.ForeignKey(
                        name: "FK_ExerciseModes_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSynonyms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSynonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSynonyms_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseTokens_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseDistractors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseDistractors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseDistractors_ExerciseTokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "ExerciseTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "ArticleHtml", "IsPublished", "Name", "SortOrder", "SourceLang", "TargetLang" },
                values: new object[] { 1, "<p>Знати — <em>know</em>.</p>", true, "Базові дієслова", 1, "uk", "en" });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Answer", "LessonId", "SortOrder", "Source" },
                values: new object[] { 1, "Know", 1, 1, "Знати" });

            migrationBuilder.InsertData(
                table: "ExerciseModes",
                columns: new[] { "Difficulty", "ExerciseId" },
                values: new object[,]
                {
                    { "Easy", 1 },
                    { "Hard", 1 },
                    { "Medium", 1 }
                });

            migrationBuilder.InsertData(
                table: "ExerciseSynonyms",
                columns: new[] { "Id", "ExerciseId", "Text" },
                values: new object[] { 1, 1, "To know" });

            migrationBuilder.InsertData(
                table: "ExerciseTokens",
                columns: new[] { "Id", "ExerciseId", "SortOrder", "Text" },
                values: new object[] { 1, 1, 1, "Know" });

            migrationBuilder.InsertData(
                table: "ExerciseDistractors",
                columns: new[] { "Id", "Text", "TokenId" },
                values: new object[,]
                {
                    { 1, "Go", 1 },
                    { 2, "See", 1 },
                    { 3, "Make", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_IsPublished",
                table: "Lessons",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_SortOrder",
                table: "Lessons",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_LessonId_SortOrder",
                table: "Exercises",
                columns: new[] { "LessonId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseDistractors_TokenId",
                table: "ExerciseDistractors",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSynonyms_ExerciseId",
                table: "ExerciseSynonyms",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTokens_ExerciseId_SortOrder",
                table: "ExerciseTokens",
                columns: new[] { "ExerciseId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseDistractors");

            migrationBuilder.DropTable(
                name: "ExerciseModes");

            migrationBuilder.DropTable(
                name: "ExerciseSynonyms");

            migrationBuilder.DropTable(
                name: "ExerciseTokens");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_IsPublished",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_SortOrder",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_LessonId_SortOrder",
                table: "Exercises");

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "ArticleHtml",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "SourceLang",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "TargetLang",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Exercises");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Exercises",
                newName: "Question");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_LessonId",
                table: "Exercises",
                column: "LessonId");
        }
    }
}
