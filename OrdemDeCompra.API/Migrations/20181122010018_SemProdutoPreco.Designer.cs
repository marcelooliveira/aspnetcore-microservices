﻿// <auto-generated />
using CasaDoCodigo.Ordering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ordering.API.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20181122010018_SemProdutoPreco")]
    partial class SemProdutoPreco
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CasaDoCodigo.Ordering.Models.ItemPedido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PedidoId");

                    b.Property<string>("ProdutoCodigo")
                        .IsRequired();

                    b.Property<string>("ProdutoNome")
                        .IsRequired();

                    b.Property<decimal>("ProdutoPrecoUnitario");

                    b.Property<int>("ProdutoQuantidade");

                    b.HasKey("Id");

                    b.HasIndex("PedidoId");

                    b.ToTable("ItemPedido");
                });

            modelBuilder.Entity("CasaDoCodigo.Ordering.Models.Pedido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClienteBairro")
                        .IsRequired();

                    b.Property<string>("ClienteCEP")
                        .IsRequired();

                    b.Property<string>("ClienteComplemento")
                        .IsRequired();

                    b.Property<string>("ClienteEmail")
                        .IsRequired();

                    b.Property<string>("ClienteEndereco")
                        .IsRequired();

                    b.Property<string>("ClienteId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ClienteMunicipio")
                        .IsRequired();

                    b.Property<string>("ClienteNome")
                        .IsRequired();

                    b.Property<string>("ClienteTelefone")
                        .IsRequired();

                    b.Property<string>("ClienteUF")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Pedido");
                });

            modelBuilder.Entity("CasaDoCodigo.Ordering.Models.ItemPedido", b =>
                {
                    b.HasOne("CasaDoCodigo.Ordering.Models.Pedido", "Pedido")
                        .WithMany("Itens")
                        .HasForeignKey("PedidoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
