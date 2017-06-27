using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityCoreProject.Migrations
{
    public partial class filesAzure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "FileAttachments");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "FileAttachments",
                newName: "URI");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "FileAttachments",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "FileAttachments");

            migrationBuilder.RenameColumn(
                name: "URI",
                table: "FileAttachments",
                newName: "Type");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "FileAttachments",
                nullable: true);
        }
    }
}
