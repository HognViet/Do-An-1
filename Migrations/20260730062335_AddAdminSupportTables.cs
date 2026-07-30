using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace San_Pham_Do_An1.Migrations
{
    public partial class AddAdminSupportTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_BlogComment",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_BlogComment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_tb_BlogComment_tb_Blog_BlogId",
                        column: x => x.BlogId,
                        principalTable: "tb_Blog",
                        principalColumn: "BlogId");
                    table.ForeignKey(
                        name: "FK_tb_BlogComment_tb_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "tb_Customer",
                        principalColumn: "CustomerId");
                });

            migrationBuilder.CreateTable(
                name: "tb_ChatMessage",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    GuestToken = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Message = table.Column<string>(type: "ntext", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ChatMessage", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_tb_ChatMessage_tb_Account_UserId",
                        column: x => x.UserId,
                        principalTable: "tb_Account",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "tb_Contact",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Contact", x => x.ContactId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_BlogComment_BlogId",
                table: "tb_BlogComment",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_BlogComment_CustomerId",
                table: "tb_BlogComment",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ChatMessage_UserId",
                table: "tb_ChatMessage",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_BlogComment");

            migrationBuilder.DropTable(
                name: "tb_ChatMessage");

            migrationBuilder.DropTable(
                name: "tb_Contact");
        }
    }
}
