using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace My_ticket.Migrations
{
    /// <inheritdoc />
    public partial class AddGameForeignKeyToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Address",
            //    table: "user1",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Age",
            //    table: "Tickets",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Tickets",
                type: "int",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ImageUrl",
            //    table: "Tickets",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Entertainment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entertainment", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "Games",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        personId = table.Column<int>(type: "int", nullable: false),
            //        ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ExtraImage1Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ExtraImage2Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ExtraImage3Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ExtraImage4Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
                //constraints: table =>
                //{
                //    table.PrimaryKey("PK_Games", x => x.Id);
                //    table.ForeignKey(
                //        name: "FK_Games_user1_personId",
                //        column: x => x.personId,
                //        principalTable: "user1",
                //        principalColumn: "Id",
                //        onDelete: ReferentialAction.Cascade);
                // });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_GameId",
                table: "Tickets",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_personId",
                table: "Games",
                column: "personId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Games_GameId",
                table: "Tickets",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Games_GameId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Entertainment");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_GameId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "user1");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tickets");
        }
    }
}
