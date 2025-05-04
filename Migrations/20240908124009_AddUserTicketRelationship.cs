using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace My_ticket.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTicketRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // التحقق مما إذا كان المفتاح الأجنبي موجودًا قبل حذفه
            migrationBuilder.Sql(@"
        IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Tickets_user1_personId')
        ALTER TABLE Tickets DROP CONSTRAINT FK_Tickets_user1_personId;
    ");

            migrationBuilder.AlterColumn<int>(
                name: "personId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_user1_personId",
                table: "Tickets",
                column: "personId",
                principalTable: "user1",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_user1_personId",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "personId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_user1_personId",
                table: "Tickets",
                column: "personId",
                principalTable: "user1",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
