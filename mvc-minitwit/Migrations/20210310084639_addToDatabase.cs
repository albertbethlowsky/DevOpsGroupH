using Microsoft.EntityFrameworkCore.Migrations;

namespace mvc_minitwit.Migrations
{
    public partial class addToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "follower",
                columns: table => new
                {
                    who_id = table.Column<int>(type: "int", nullable: false),
                    whom_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_follower", x => new { x.who_id, x.whom_id });
                });

            migrationBuilder.CreateTable(
                name: "TimelineData",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pub_date = table.Column<int>(type: "int", nullable: false),
                    flagged = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pw_hash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineData", x => x.message_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pw_hash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pub_date = table.Column<int>(type: "int", nullable: false),
                    flagged = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_message_user_author_id",
                        column: x => x.author_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_message_author_id",
                table: "message",
                column: "author_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "follower");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "TimelineData");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
