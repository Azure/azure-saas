﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Saas.Admin.Service.Data;

#nullable disable

namespace Saas.Admin.Service.Migrations
{
    [DbContext(typeof(TenantsContext))]
    [Migration("20230613115209_UpdatedTenant")]
    partial class UpdatedTenant
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Saas.Admin.Service.Data.Models.OnBoarding.Tenant", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Company")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(4126));

                    b.Property<string>("CreatedUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DatabaseName")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("");

                    b.Property<int>("Employees")
                        .HasColumnType("int");

                    b.Property<bool>("ExternalDB")
                        .HasColumnType("bit");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Industry")
                        .HasColumnType("int");

                    b.Property<bool>("InitReady")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int>("ProductTierId")
                        .HasColumnType("int");

                    b.Property<string>("Route")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedUser")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Guid");

                    b.HasIndex("Route")
                        .IsUnique();

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("Saas.Admin.Service.Data.Models.OnBoarding.UserInfo", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AcceptTerms")
                        .HasColumnType("bit");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BioUserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("0");

                    b.Property<string>("ConfirmPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 6, 13, 11, 52, 9, 139, DateTimeKind.Utc).AddTicks(607));

                    b.Property<DateTime>("DOB")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullNames")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IDType")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("National ID");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("LockAfter")
                        .HasColumnType("int");

                    b.Property<bool>("Notifications")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Guid");

                    b.ToTable("UserInfo");
                });

            modelBuilder.Entity("Saas.Admin.Service.Data.Models.OnBoarding.UserTenant", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CCCode")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("OO1");

                    b.Property<DateTime?>("CreatedDate")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 6, 13, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(7719));

                    b.Property<string>("CreatedUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmpNo")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("001");

                    b.Property<int>("ExpiresAfter")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(90);

                    b.Property<DateTime>("ExpiryDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2023, 9, 11, 11, 52, 9, 138, DateTimeKind.Utc).AddTicks(6357));

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("PrincipalUser")
                        .HasColumnType("bit");

                    b.Property<string>("Profession")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegSource")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("SuperUser")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "TenantId");

                    b.HasIndex("TenantId");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("Saas.Admin.Service.Data.Models.OnBoarding.UserTenant", b =>
                {
                    b.HasOne("Saas.Admin.Service.Data.Models.OnBoarding.Tenant", null)
                        .WithMany("UserTenants")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Saas.Admin.Service.Data.Models.OnBoarding.UserInfo", null)
                        .WithMany("UserTenants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Saas.Admin.Service.Data.Models.OnBoarding.Tenant", b =>
                {
                    b.Navigation("UserTenants");
                });

            modelBuilder.Entity("Saas.Admin.Service.Data.Models.OnBoarding.UserInfo", b =>
                {
                    b.Navigation("UserTenants");
                });
#pragma warning restore 612, 618
        }
    }
}
