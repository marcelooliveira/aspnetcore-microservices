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
    [Migration("20181101035754_SemProductId")]
    partial class SemProductId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CasaDoCodigo.Ordering.Models.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("OrderId");

                    b.Property<string>("ProductCodigo")
                        .IsRequired();

                    b.Property<string>("ProductNome")
                        .IsRequired();

                    b.Property<decimal>("ProductPreco");

                    b.Property<decimal>("ProductPrecoUnitario");

                    b.Property<int>("ProductQuantidade");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("CasaDoCodigo.Ordering.Models.Order", b =>
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

                    b.Property<string>("CustomerId")
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

                    b.ToTable("Order");
                });

            modelBuilder.Entity("CasaDoCodigo.Ordering.Models.OrderItem", b =>
                {
                    b.HasOne("CasaDoCodigo.Ordering.Models.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
