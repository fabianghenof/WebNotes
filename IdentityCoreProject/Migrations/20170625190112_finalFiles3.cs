using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityCoreProject.Migrations
{
    public partial class finalFiles3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachments_WebNotes_WebNoteId",
                table: "FileAttachments");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachments_WebNoteId",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "WebNoteId",
                table: "FileAttachments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WebNoteId",
                table: "FileAttachments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_WebNoteId",
                table: "FileAttachments",
                column: "WebNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachments_WebNotes_WebNoteId",
                table: "FileAttachments",
                column: "WebNoteId",
                principalTable: "WebNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
