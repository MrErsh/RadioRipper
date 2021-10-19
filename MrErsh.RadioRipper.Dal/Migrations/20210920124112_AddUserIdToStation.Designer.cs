﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MrErsh.RadioRipper.Dal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MrErsh.RadioRipper.Dal.Migrations
{
    [DbContext(typeof(RadioDbContext))]
    [Migration("20210920124112_AddUserIdToStation")]
    partial class AddUserIdToStation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MrErsh.RadioRipper.Model.Station", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<bool>("IsRunning")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Stations");
                });

            modelBuilder.Entity("MrErsh.RadioRipper.Model.Track", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Artist")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("MetadataHeader")
                        .HasColumnType("text");

                    b.Property<Guid?>("StationId")
                        .HasColumnType("uuid");

                    b.Property<string>("TrackName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("StationId");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("MrErsh.RadioRipper.Model.Track", b =>
                {
                    b.HasOne("MrErsh.RadioRipper.Model.Station", "Station")
                        .WithMany("Tracks")
                        .HasForeignKey("StationId");

                    b.Navigation("Station");
                });

            modelBuilder.Entity("MrErsh.RadioRipper.Model.Station", b =>
                {
                    b.Navigation("Tracks");
                });
#pragma warning restore 612, 618
        }
    }
}