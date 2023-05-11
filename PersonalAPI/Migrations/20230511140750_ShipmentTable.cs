using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalAPI.Migrations
{
    /// <inheritdoc />
    public partial class ShipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracking_data",
                columns: table => new
                {
                    tracking_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    last_scan_location = table.Column<string>(type: "varchar(255)", nullable: false),
                    order_number = table.Column<string>(type: "varchar(255)", nullable: false),
                    destination = table.Column<string>(type: "varchar(255)", nullable: false),
                    scan_time = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracking_data", x => x.tracking_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracking_data");
        }
    }
}
