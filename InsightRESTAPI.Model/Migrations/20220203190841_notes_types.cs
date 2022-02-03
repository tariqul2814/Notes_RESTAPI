using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InsightRESTAPI.Model.Migrations
{
    public partial class notes_types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "due_date",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "is_complete",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "reminder_date",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "text_note",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "web_url",
                table: "Notes");

            migrationBuilder.CreateTable(
                name: "BookmarkNotes",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url = table.Column<string>(nullable: false),
                    NoteID = table.Column<long>(nullable: false),
                    NotesID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookmarkNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookmarkNotes_Notes_NotesID",
                        column: x => x.NotesID,
                        principalTable: "Notes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegularNotes",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    text_note = table.Column<string>(maxLength: 100, nullable: false),
                    NoteID = table.Column<long>(nullable: false),
                    NotesID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegularNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RegularNotes_Notes_NotesID",
                        column: x => x.NotesID,
                        principalTable: "Notes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReminderNotes",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    text_note = table.Column<string>(maxLength: 100, nullable: false),
                    reminder_date = table.Column<DateTime>(nullable: false),
                    NoteID = table.Column<long>(nullable: false),
                    NotesID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReminderNotes_Notes_NotesID",
                        column: x => x.NotesID,
                        principalTable: "Notes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskNotes",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    text_note = table.Column<string>(maxLength: 100, nullable: false),
                    due_date = table.Column<DateTime>(nullable: false),
                    is_complete = table.Column<bool>(nullable: false),
                    NoteID = table.Column<long>(nullable: false),
                    NotesID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TaskNotes_Notes_NotesID",
                        column: x => x.NotesID,
                        principalTable: "Notes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookmarkNotes_NotesID",
                table: "BookmarkNotes",
                column: "NotesID");

            migrationBuilder.CreateIndex(
                name: "IX_RegularNotes_NotesID",
                table: "RegularNotes",
                column: "NotesID");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderNotes_NotesID",
                table: "ReminderNotes",
                column: "NotesID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskNotes_NotesID",
                table: "TaskNotes",
                column: "NotesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookmarkNotes");

            migrationBuilder.DropTable(
                name: "RegularNotes");

            migrationBuilder.DropTable(
                name: "ReminderNotes");

            migrationBuilder.DropTable(
                name: "TaskNotes");

            migrationBuilder.AddColumn<DateTime>(
                name: "due_date",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_complete",
                table: "Notes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "reminder_date",
                table: "Notes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "text_note",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "web_url",
                table: "Notes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
