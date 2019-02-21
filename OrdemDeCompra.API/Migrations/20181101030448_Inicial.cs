using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<string>(maxLength: 50, nullable: false),
                    ClienteNome = table.Column<string>(nullable: false),
                    ClienteEmail = table.Column<string>(nullable: false),
                    ClienteTelefone = table.Column<string>(nullable: false),
                    ClienteEndereco = table.Column<string>(nullable: false),
                    ClienteComplemento = table.Column<string>(nullable: false),
                    ClienteBairro = table.Column<string>(nullable: false),
                    ClienteMunicipio = table.Column<string>(nullable: false),
                    ClienteUF = table.Column<string>(nullable: false),
                    ClienteCEP = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ProductCodigo = table.Column<string>(nullable: false),
                    ProductNome = table.Column<string>(nullable: false),
                    ProductPreco = table.Column<decimal>(nullable: false),
                    ProductQuantidade = table.Column<int>(nullable: false),
                    ProductPrecoUnitario = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Order");
        }
    }
}
