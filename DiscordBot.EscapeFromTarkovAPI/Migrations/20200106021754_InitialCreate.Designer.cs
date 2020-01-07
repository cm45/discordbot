﻿// <auto-generated />
using System;
using DiscordBot.EscapeFromTarkovAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBot.EscapeFromTarkovAPI.Migrations
{
    [DbContext(typeof(TarkovContext))]
    [Migration("20200106021754_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DiscordBot.EscapeFromTarkovAPI.Models.Ammo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaliberId")
                        .HasColumnType("int");

                    b.Property<float>("Damage")
                        .HasColumnType("real");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("PenetrationValue")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("CaliberId");

                    b.ToTable("Ammos");
                });

            modelBuilder.Entity("DiscordBot.EscapeFromTarkovAPI.Models.Caliber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Calibers");
                });

            modelBuilder.Entity("DiscordBot.EscapeFromTarkovAPI.Models.Weapon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CaliberId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CaliberId");

                    b.ToTable("Weapons");
                });

            modelBuilder.Entity("DiscordBot.EscapeFromTarkovAPI.Models.Ammo", b =>
                {
                    b.HasOne("DiscordBot.EscapeFromTarkovAPI.Models.Caliber", null)
                        .WithMany("Ammos")
                        .HasForeignKey("CaliberId");
                });

            modelBuilder.Entity("DiscordBot.EscapeFromTarkovAPI.Models.Weapon", b =>
                {
                    b.HasOne("DiscordBot.EscapeFromTarkovAPI.Models.Caliber", "Caliber")
                        .WithMany()
                        .HasForeignKey("CaliberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
