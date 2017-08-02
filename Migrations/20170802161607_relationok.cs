using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiEFCore.Migrations
{
    public partial class relationok : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Stories_UserStoryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserStoryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserStoryId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UsuariosId",
                table: "Stories",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_UsuariosId",
                table: "Stories",
                column: "UsuariosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_UsuariosId",
                table: "Stories",
                column: "UsuariosId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_UsuariosId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_UsuariosId",
                table: "Stories");

            migrationBuilder.AddColumn<int>(
                name: "UserStoryId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuariosId",
                table: "Stories",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserStoryId",
                table: "AspNetUsers",
                column: "UserStoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Stories_UserStoryId",
                table: "AspNetUsers",
                column: "UserStoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
