﻿// <auto-generated />
using System;
using Dressify.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace dressify.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Dressify.Models.Admin", b =>
                {
                    b.Property<string>("AdminId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AdminName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("ProfilePic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AdminId");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("Dressify.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Age")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ProfilePic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool?>("isSuspended")
                        .HasColumnType("bit");

                    b.Property<int?>("nId")
                        .HasColumnType("int");

                    b.Property<string>("storeName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Dressify.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfSales")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Purchases")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<bool>("Rentable")
                        .HasColumnType("bit");

                    b.Property<float>("Sale")
                        .HasColumnType("real");

                    b.Property<string>("SubCategory")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Suspended")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProductId");

                    b.HasIndex("VendorId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Dressify.Models.ProductImage", b =>
                {
                    b.Property<int>("ImageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageID"), 1L, 1);

                    b.Property<string>("ImageExtension")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("PublicId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ImageID");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("Dressify.Models.ProductQuestion", b =>
                {
                    b.Property<int>("QuestionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuestionID"), 1L, 1);

                    b.Property<string>("Answear")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("QuestionDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("VendorId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("QuestionID");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProductId");

                    b.HasIndex("VendorId");

                    b.ToTable("ProductsQuestions");
                });

            modelBuilder.Entity("Dressify.Models.ProductRate", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsPurchased")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("RateComment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("rate")
                        .HasColumnType("int");

                    b.HasKey("CustomerId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductsRates");
                });

            modelBuilder.Entity("Dressify.Models.ProductReport", b =>
                {
                    b.Property<int>("ReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ReportId"), 1L, 1);

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AdminId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<bool>("ReportStatus")
                        .HasColumnType("bit");

                    b.Property<string>("VendorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReportId");

                    b.HasIndex("AdminId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductsReports");
                    modelBuilder.Entity("Dressify.Models.ShoppingCart", b =>
                        {
                            b.Property<string>("CustomerId")
                                .HasColumnType("nvarchar(450)");

                            b.Property<int>("ProductId")
                                .HasColumnType("int");

                            b.Property<bool>("IsRent")
                                .HasColumnType("bit");

                            b.Property<int?>("quantity")
                                .HasColumnType("int");

                            b.HasKey("CustomerId", "ProductId");

                            b.HasIndex("ProductId");

                            b.ToTable("shoppingCarts");
                        });

                    modelBuilder.Entity("Dressify.Models.SuperAdmin", b =>
                        {
                            b.Property<int>("SuperAdminId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SuperAdminId"), 1L, 1);

                            b.Property<byte[]>("PasswordHash")
                                .HasColumnType("varbinary(max)");

                            b.Property<byte[]>("PasswordSalt")
                                .HasColumnType("varbinary(max)");

                            b.Property<string>("UserName")
                                .HasColumnType("nvarchar(max)");

                            b.HasKey("SuperAdminId");

                            b.ToTable("SuperAdmins");
                        });

                    modelBuilder.Entity("Dressify.Models.WishList", b =>
                        {
                            b.Property<string>("CustomerId")
                                .HasColumnType("nvarchar(450)");

                            b.Property<int>("ProductId")
                                .HasColumnType("int");

                            b.HasKey("CustomerId", "ProductId");

                            b.HasIndex("ProductId");

                            b.ToTable("WishesLists");
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                        {
                            b.Property<string>("Id")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("ConcurrencyStamp")
                                .IsConcurrencyToken()
                                .HasColumnType("nvarchar(max)");

                            b.Property<string>("Name")
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)");

                            b.Property<string>("NormalizedName")
                                .HasMaxLength(256)
                                .HasColumnType("nvarchar(256)");

                            b.HasKey("Id");

                            b.HasIndex("NormalizedName")
                                .IsUnique()
                                .HasDatabaseName("RoleNameIndex")
                                .HasFilter("[NormalizedName] IS NOT NULL");

                            b.ToTable("AspNetRoles", (string)null);
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                        {
                            b.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                            b.Property<string>("ClaimType")
                                .HasColumnType("nvarchar(max)");

                            b.Property<string>("ClaimValue")
                                .HasColumnType("nvarchar(max)");

                            b.Property<string>("RoleId")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b.HasKey("Id");

                            b.HasIndex("RoleId");

                            b.ToTable("AspNetRoleClaims", (string)null);
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                        {
                            b.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                            b.Property<string>("ClaimType")
                                .HasColumnType("nvarchar(max)");

                            b.Property<string>("ClaimValue")
                                .HasColumnType("nvarchar(max)");

                            b.Property<string>("UserId")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b.HasKey("Id");

                            b.HasIndex("UserId");

                            b.ToTable("AspNetUserClaims", (string)null);
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                        {
                            b.Property<string>("LoginProvider")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("ProviderKey")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("ProviderDisplayName")
                                .HasColumnType("nvarchar(max)");

                            b.Property<string>("UserId")
                                .IsRequired()
                                .HasColumnType("nvarchar(450)");

                            b.HasKey("LoginProvider", "ProviderKey");

                            b.HasIndex("UserId");

                            b.ToTable("AspNetUserLogins", (string)null);
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                        {
                            b.Property<string>("UserId")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("RoleId")
                                .HasColumnType("nvarchar(450)");

                            b.HasKey("UserId", "RoleId");

                            b.HasIndex("RoleId");

                            b.ToTable("AspNetUserRoles", (string)null);
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                        {
                            b.Property<string>("UserId")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("LoginProvider")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("Name")
                                .HasColumnType("nvarchar(450)");

                            b.Property<string>("Value")
                                .HasColumnType("nvarchar(max)");

                            b.HasKey("UserId", "LoginProvider", "Name");

                            b.ToTable("AspNetUserTokens", (string)null);
                        });

                    modelBuilder.Entity("Dressify.Models.Product", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", "Vendor")
                                .WithMany("Products")
                                .HasForeignKey("VendorId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.Navigation("Vendor");
                        });

                    modelBuilder.Entity("Dressify.Models.ProductImage", b =>
                        {
                            b.HasOne("Dressify.Models.Product", "Product")
                                .WithMany("ProductImages")
                                .HasForeignKey("ProductId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.Navigation("Product");
                        });

                    modelBuilder.Entity("Dressify.Models.ProductQuestion", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", "Customer")
                                .WithMany("QuestionsAsked")
                                .HasForeignKey("CustomerId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.HasOne("Dressify.Models.Product", "Product")
                                .WithMany("Questions")
                                .HasForeignKey("ProductId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.HasOne("Dressify.Models.ApplicationUser", "Vendor")
                                .WithMany("QuestionsAnswered")
                                .HasForeignKey("VendorId");

                            b.Navigation("Customer");

                            b.Navigation("Product");

                            b.Navigation("Vendor");
                        });

                    modelBuilder.Entity("Dressify.Models.ProductRate", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", "ApplicationUser")
                                .WithMany()
                                .HasForeignKey("CustomerId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.HasOne("Dressify.Models.Product", "Product")
                                .WithMany()
                                .HasForeignKey("ProductId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.Navigation("ApplicationUser");

                            b.Navigation("Product");
                        });

                    modelBuilder.Entity("Dressify.Models.ProductReport", b =>
                        {
                            b.HasOne("Dressify.Models.Admin", "Admin")
                                .WithMany("Reports")
                                .HasForeignKey("AdminId");

                            b.HasOne("Dressify.Models.ApplicationUser", "Customer")
                                .WithMany("Reports");

                        });
                    modelBuilder.Entity("Dressify.Models.ShoppingCart", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", "ApplicationUser")
                                .WithMany()
                                .HasForeignKey("CustomerId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.HasOne("Dressify.Models.Product", "Product")
                                .WithMany("Reports")
                                .HasForeignKey("ProductId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.Navigation("Admin");

                            b.Navigation("Customer");
                            b.Navigation("ApplicationUser");

                            b.Navigation("Product");
                        });

                    modelBuilder.Entity("Dressify.Models.WishList", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", "ApplicationUser")
                                .WithMany("WishesLists")
                                .HasForeignKey("CustomerId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.HasOne("Dressify.Models.Product", "Product")
                                .WithMany()
                                .HasForeignKey("ProductId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b.Navigation("ApplicationUser");

                            b.Navigation("Product");
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
                            b.HasOne("Dressify.Models.ApplicationUser", null)
                                .WithMany()
                                .HasForeignKey("UserId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", null)
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

                            b.HasOne("Dressify.Models.ApplicationUser", null)
                                .WithMany()
                                .HasForeignKey("UserId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();
                        });

                    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                        {
                            b.HasOne("Dressify.Models.ApplicationUser", null)
                                .WithMany()
                                .HasForeignKey("UserId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();
                        });

                    modelBuilder.Entity("Dressify.Models.Admin", b =>
                        {
                            b.Navigation("Reports");
                        });

                    modelBuilder.Entity("Dressify.Models.ApplicationUser", b =>
                        {
                            b.Navigation("Products");

                            b.Navigation("QuestionsAnswered");

                            b.Navigation("QuestionsAsked");

                            b.Navigation("Reports");

                            b.Navigation("WishesLists");
                        });

                    modelBuilder.Entity("Dressify.Models.Product", b =>
                        {
                            b.Navigation("ProductImages");

                            b.Navigation("Questions");

                            b.Navigation("Reports");
                        });
#pragma warning restore 612, 618
                });
        }
    } 
}
