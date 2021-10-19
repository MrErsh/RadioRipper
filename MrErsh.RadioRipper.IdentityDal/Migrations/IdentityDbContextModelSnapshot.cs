﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MrErsh.RadioRipper.IdentityDal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MrErsh.RadioRipper.IdentityDal.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    partial class IdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("Role");

                    b.HasData(
                        new
                        {
                            Id = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9",
                            ConcurrencyStamp = "b7eb86b2-6169-43a8-9a04-36b5ca4eb606",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = "3FD8AC80-AB40-4BAB-97C0-00721424FD31",
                            ConcurrencyStamp = "85437ffe-ce74-450c-8b88-d4717ee767ed",
                            Name = "User",
                            NormalizedName = "USER"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaim");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.View",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -2,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.Run",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -3,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.Add",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -4,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.StopForOtherUsers",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -5,
                            ClaimType = "Permission",
                            ClaimValue = "Users.Add",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -6,
                            ClaimType = "Permission",
                            ClaimValue = "Users.Remove",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -7,
                            ClaimType = "Permission",
                            ClaimValue = "Users.Edit",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        },
                        new
                        {
                            Id = -8,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.Add",
                            RoleId = "3FD8AC80-AB40-4BAB-97C0-00721424FD31"
                        },
                        new
                        {
                            Id = -9,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.Run",
                            RoleId = "3FD8AC80-AB40-4BAB-97C0-00721424FD31"
                        },
                        new
                        {
                            Id = -10,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.View",
                            RoleId = "3FD8AC80-AB40-4BAB-97C0-00721424FD31"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaim");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.View",
                            UserId = "22714632-C0F2-4DC0-96D8-5E6024616E4A"
                        },
                        new
                        {
                            Id = -2,
                            ClaimType = "Permission",
                            ClaimValue = "Stations.Run",
                            UserId = "22714632-C0F2-4DC0-96D8-5E6024616E4A"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");

                    b.HasData(
                        new
                        {
                            UserId = "EA83A100-F55C-457F-9DA7-697D880F0227",
                            RoleId = "0EBE358E-D52F-420B-B4FA-C17EA742B8B9"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("MrErsh.RadioRipper.IdentityDal.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = "EA83A100-F55C-457F-9DA7-697D880F0227",
                            ConcurrencyStamp = "b9ee6447-6c3a-4460-bb6c-03d2841fddd3",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAEAACcQAAAAEAXUGT73DtpwAnjpRqfUMsIHvhwR9AaGSHhLfqc9QinE1r8Rsrkj5GkSPwc8uQFZFw==",
                            SecurityStamp = "804d7fd7-eb8e-435b-98b5-6c381631922f",
                            UserName = "admin"
                        },
                        new
                        {
                            Id = "22714632-C0F2-4DC0-96D8-5E6024616E4A",
                            ConcurrencyStamp = "ee577e9a-db74-4007-bc26-21c6eda355a9",
                            NormalizedUserName = "DEMO",
                            PasswordHash = "AQAAAAEAACcQAAAAENyGB/QO0Cj2/C0bbqAeaUu/yARGg0jWTfc1cKw61TClrGEbDJPouBPmBCw4XmHZ1g==",
                            SecurityStamp = "104cf1f3-73a3-4e4a-9482-34b5c01436dd",
                            UserName = "demo"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MrErsh.RadioRipper.IdentityDal.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MrErsh.RadioRipper.IdentityDal.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MrErsh.RadioRipper.IdentityDal.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MrErsh.RadioRipper.IdentityDal.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
