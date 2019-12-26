using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrackMyStuff.DevicesService.Migrations
{
    public partial class HeartBeatPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HeartBeat",
                table: "HeartBeat");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "HeartBeat",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "HeartBeat",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HeartBeat",
                table: "HeartBeat",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HeartBeat",
                table: "HeartBeat");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "HeartBeat");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "HeartBeat",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_HeartBeat",
                table: "HeartBeat",
                column: "DeviceId");
        }
    }
}
