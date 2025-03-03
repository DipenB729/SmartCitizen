using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCitizen.Migrations
{
    /// <inheritdoc />
    public partial class newmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
     name: "Comment",
     columns: table => new
     {
         Id = table.Column<int>(nullable: false)
             .Annotation("SqlServer:Identity", "1, 1"),
         UserId = table.Column<string>(nullable: false),
         ComplaintId = table.Column<int>(nullable: false),
         CreatedAt = table.Column<DateTime>(nullable: false),
         Content = table.Column<string>(nullable: false)
     },
     constraints: table =>
     {
         table.PrimaryKey("PK_Comment", x => x.Id);
         table.ForeignKey(
             name: "FK_Comment_AspNetUsers_UserId",
             column: x => x.UserId,
             principalTable: "AspNetUsers",
             principalColumn: "Id",
             onDelete: ReferentialAction.NoAction); // Change to NoAction

         table.ForeignKey(
             name: "FK_Comment_Complaints_ComplaintId",
             column: x => x.ComplaintId,
             principalTable: "Complaints",
             principalColumn: "Id",
             onDelete: ReferentialAction.NoAction); // Change to NoAction
     });


            migrationBuilder.CreateIndex(
                name: "IX_Comment_ComplaintId",
                table: "Comment",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");
        }
    }
}
