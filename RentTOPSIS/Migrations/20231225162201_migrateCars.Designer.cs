﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentTOPSIS.Context;

#nullable disable

namespace RentTOPSIS.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20231225162201_migrateCars")]
    partial class migrateCars
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RentTOPSIS.Models.Arabalar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AracDonanimi")
                        .HasColumnType("int");

                    b.Property<int>("BagajHacmi")
                        .HasColumnType("int");

                    b.Property<int>("GunlukUcret")
                        .HasColumnType("int");

                    b.Property<int>("KmSinirlamasi")
                        .HasColumnType("int");

                    b.Property<string>("Marka")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("YakitEkonomisi")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Arabalar");
                });
#pragma warning restore 612, 618
        }
    }
}