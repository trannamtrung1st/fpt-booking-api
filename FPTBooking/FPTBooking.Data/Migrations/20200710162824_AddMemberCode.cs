using Microsoft.EntityFrameworkCore.Migrations;

namespace FPTBooking.Data.Migrations
{
    public partial class AddMemberCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Member",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberCode",
                table: "AspNetUsers",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Administrator",
                column: "ConcurrencyStamp",
                value: "b6110651-9ef8-4353-8bac-511a01a4587a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Manager",
                column: "ConcurrencyStamp",
                value: "dcc76927-4095-43f7-b1d9-dc0d5e04661e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "RoomChecker",
                column: "ConcurrencyStamp",
                value: "f018d3c2-3da3-4033-8da5-5f0aaf32abd0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "User",
                column: "ConcurrencyStamp",
                value: "ab32b32d-b799-4248-b3cc-806557ea6b10");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "MemberCode",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Administrator",
                column: "ConcurrencyStamp",
                value: "e313d30b-1fa4-4ecb-9a70-abf56033bbf0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Manager",
                column: "ConcurrencyStamp",
                value: "40a97dba-d2af-40df-97c0-1569f9f26e39");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "RoomChecker",
                column: "ConcurrencyStamp",
                value: "10ff3668-7a75-4d29-9b2d-6a687afb7945");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "User",
                column: "ConcurrencyStamp",
                value: "234c06a2-9526-4f8f-817d-9a416f237a5b");
        }
    }
}
