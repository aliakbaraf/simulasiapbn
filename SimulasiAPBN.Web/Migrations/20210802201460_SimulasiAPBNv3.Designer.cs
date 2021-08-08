﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SimulasiAPBN.Infrastructure.EntityFrameworkCore;

namespace SimulasiAPBN.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210802201460_SimulasiAPBNv3")]
    partial class InitialApplicationSchemaV3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            modelBuilder.Entity("SimulasiAPBN.Core.Models.EconomicMacro", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("StateBudgetId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Description")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Naration")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("NarationDefisit")
                    .HasColumnType("nvarchar(max)");

                b.Property<decimal>("DefaultValue")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal>("MinimumValue")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal>("MaximumValue")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal>("Threshold")
                    .HasColumnType("decimal(18,2)");

                b.Property<decimal>("ThresholdValue")
                    .HasColumnType("decimal(18,2)");

                b.Property<string>("UnitDesc")
                    .IsRequired()
                    .HasColumnType("nvarchar(50)");

                b.Property<int>("OrderFlag")
                    .IsRequired()
                    .HasColumnType("int");

                b.Property<DateTimeOffset>("CreatedAt")
                    .HasColumnType("datetimeoffset");

                b.Property<DateTimeOffset?>("DeletedAt")
                    .HasColumnType("datetimeoffset");

                b.Property<DateTimeOffset?>("UpdatedAt")
                    .HasColumnType("datetimeoffset");

                b.HasKey("Id");

                b.HasIndex("StateBudgetId");

                b.ToTable("EconomicMacros");
            });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.SimulationSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("EngineKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SimulationState")
                        .HasColumnType("int");

                    b.Property<Guid>("StateBudgetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("UsedIncome")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTimeOffset?>("UpdatedAt")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("StateBudgetId");

                    b.ToTable("SimulationSessions");
                });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.SimulationEconomicMacro", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTimeOffset>("CreatedAt")
                    .HasColumnType("datetimeoffset");

                b.Property<DateTimeOffset?>("DeletedAt")
                    .HasColumnType("datetimeoffset");

                b.Property<Guid>("SimulationSessionId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("EconomicMacrosId")
                    .HasColumnType("uniqueidentifier");

                b.Property<decimal>("UsedValue")
                    .HasColumnType("decimal(18,2)");

                b.Property<DateTimeOffset?>("UpdatedAt")
                    .HasColumnType("datetimeoffset");

                b.HasKey("Id");

                b.HasIndex("SimulationSessionId");

                b.HasIndex("EconomicMacrosId");

                b.ToTable("SimulationEconomicMacros");
            });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.SimulationSession", b =>
                {
                    b.HasOne("SimulasiAPBN.Core.Models.StateBudget", "StateBudget")
                        .WithMany()
                        .HasForeignKey("StateBudgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StateBudget");
                });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.EconomicMacro", b =>
            {
                b.HasOne("SimulasiAPBN.Core.Models.StateBudget", "StateBudget")
                        .WithMany("EconomicMacros")
                        .HasForeignKey("StateBudgetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                b.Navigation("StateBudget");

                //b.HasData(
                //    new SimulasiAPBN.Core.Models.EconomicMacro
                //    {
                //        Id = Guid.NewGuid(),
                //        Name = "Pertumbuhan Ekonomi",
                //        UnitDesc = "%",
                //        OrderFlag = 1,
                //        DefaultValue = 5m,
                //        Threshold = 0.1m,
                //        ThresholdValue = 1263.4m,
                //        MinimumValue = 3m,
                //        MaximumValue = 8m
                //    });


            });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.SimulationEconomicMacro", b =>
            {
                b.HasOne("SimulasiAPBN.Core.Models.SimulationSession", "SimulationSession")
                    .WithMany("SimulationEconomicMacros")
                    .HasForeignKey("SimulationSessionId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("SimulasiAPBN.Core.Models.EconomicMacro", "EconomicMacro")
                    .WithMany("SimulationEconomicMacros")
                    .HasForeignKey("EconomicMacrosId")
                    .OnDelete(DeleteBehavior.NoAction)
                    .IsRequired();

                b.Navigation("SimulationSession");

                b.Navigation("EconomicMacro");
            });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.SimulationSession", b =>
                {
                    b.Navigation("SimulationEconomicMacros");
                });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.StateBudget", b =>
                {
                    b.Navigation("EconomicMacros");
                });

            modelBuilder.Entity("SimulasiAPBN.Core.Models.EconomicMacro", b =>
            {
                b.Navigation("SimulationEconomicMacros");
            });
#pragma warning restore 612, 618
        }
    }
}
