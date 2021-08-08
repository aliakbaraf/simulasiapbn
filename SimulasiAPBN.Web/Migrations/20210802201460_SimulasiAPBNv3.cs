using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimulasiAPBN.Web.Migrations
{
    public partial class InitialApplicationSchemaV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UsedIncome",
                table: "SimulationSessions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0
            );
            
            migrationBuilder.Sql("UPDATE dbo.SimulationSessions SET UsedIncome = 1743.64 WHERE UsedIncome = 0");

            migrationBuilder.CreateTable(
                name: "EconomicMacros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Naration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NarationDefisit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Threshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitDesc = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    OrderFlag = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EconomicMacros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EconomicMacros_StateBudgets_StateBudgetId",
                        column: x => x.StateBudgetId,
                        principalTable: "StateBudgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            ////migrationBuilder.Sql("INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Pertumbuhan Ekonomi', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(5.00 AS Decimal(18, 2)), CAST(3.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), CAST(0.10 AS Decimal(18, 2)), CAST(1263.40 AS Decimal(18, 2)), N'%', 1, GETDATE(),GETDATE(), NULL)");
            ////migrationBuilder.Sql("INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Nilai Tukar', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(14600.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(1287.90 AS Decimal(18, 2)), N'Rp/USD1', 2, GETDATE(),GETDATE(), NULL)");
            ////migrationBuilder.Sql("INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Inflasi', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(3.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(0.10 AS Decimal(18, 2)), CAST(1287.90 AS Decimal(18, 2)), N'%', 3, GETDATE(),GETDATE(), NULL)");
            ////migrationBuilder.Sql("INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Crude-Oil Price', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(45.00 AS Decimal(18, 2)), CAST(30.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(2837.10 AS Decimal(18, 2)), N'US$/Barrel', 4, GETDATE(),GETDATE(), NULL)");
            ////migrationBuilder.Sql("INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Lifting Migas', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(705.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(705.00 AS Decimal(18, 2)), N'ribu barel/hari', 5, GETDATE(),GETDATE(), NULL)");
            ////migrationBuilder.Sql("INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Suku Bunga SBN 10 tahun', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(7.29 AS Decimal(18, 2)), CAST(6.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(0.01 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'%', 6, GETDATE(),GETDATE(), NULL)");
            //migrationBuilder.Sql("IF EXISTS(SELECT TOP 1 CountryIncome FROM StateBudgets)"+
            //                   "BEGIN " +
            //                        "IF NOT EXISTS(SELECT TOP 1 Id FROM EconomicMacros) "+
            //                        "BEGIN" +
            //                            "INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Pertumbuhan Ekonomi', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(5.00 AS Decimal(18, 2)), CAST(3.00 AS Decimal(18, 2)), CAST(8.00 AS Decimal(18, 2)), CAST(0.10 AS Decimal(18, 2)), CAST(1263.40 AS Decimal(18, 2)), N'%', 1, GETDATE(),GETDATE(), NULL) " +
            //                            "INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Nilai Tukar', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(14600.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(20000.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(1287.90 AS Decimal(18, 2)), N'Rp/USD1', 2, GETDATE(),GETDATE(), NULL) " +
            //                            "INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Inflasi', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(3.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(0.10 AS Decimal(18, 2)), CAST(1287.90 AS Decimal(18, 2)), N'%', 3, GETDATE(),GETDATE(), NULL) " +
            //                            "INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Crude-Oil Price', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(45.00 AS Decimal(18, 2)), CAST(30.00 AS Decimal(18, 2)), CAST(100.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), CAST(2837.10 AS Decimal(18, 2)), N'US$/Barrel', 4, GETDATE(),GETDATE(), NULL) " +
            //                            "INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Lifting Migas', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(705.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), CAST(1000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(705.00 AS Decimal(18, 2)), N'ribu barel/hari', 5, GETDATE(),GETDATE(), NULL) " +
            //                            "INSERT[dbo].[EconomicMacros]([Id], [StateBudgetId], [Name], [Description], [Naration], [NarationDefisit], [DefaultValue], [MinimumValue], [MaximumValue], [Threshold], [ThresholdValue], [UnitDesc], [OrderFlag], [CreatedAt], [UpdatedAt], [DeletedAt]) VALUES(NEWID(), (SELECT TOP 1 ID FROM StateBudgets), N'Suku Bunga SBN 10 tahun', N'Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Nisl tincidunt eget nullam non. Quis hendrerit dolor magna eget est lorem ipsum dolor sit. Volutpat odio facilisis mauris sit amet massa. Commodo odio aenean sed adipiscing diam donec adipiscing tristique. Mi eget mauris pharetra et. Non tellus orci ac auctor augue. Elit at imperdiet dui accumsan sit. Ornare arcu dui vivamus arcu felis. Egestas integer eget aliquet nibh praesent. In hac habitasse platea dictumst quisque sagittis purus. Pulvinar elementum integer enim neque volutpat ac.', NULL, NULL, CAST(7.29 AS Decimal(18, 2)), CAST(6.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), CAST(0.01 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'%', 6, GETDATE(),GETDATE(), NULL) " +
            //                        "END" +
            //                    "END");

            migrationBuilder.CreateTable(
                name: "SimulationEconomicMacros",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EconomicMacrosId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationEconomicMacros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationEconomicMacros_SimulationSessions_SimulationSessionId",
                        column: x => x.SimulationSessionId,
                        principalTable: "SimulationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimulationEconomicMacros_EconomicMacros_EconomicMacrosId",
                        column: x => x.EconomicMacrosId,
                        principalTable: "EconomicMacros",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EconomicMacros_StateBudgetId",
                table: "EconomicMacros",
                column: "StateBudgetId");

            migrationBuilder.CreateIndex(
               name: "IX_SimulationEconomicMacros_SimulationSessionId",
               table: "SimulationEconomicMacros",
               column: "SimulationSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationEconomicMacros_EconomicMacrosId",
                table: "SimulationEconomicMacros",
                column: "EconomicMacrosId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedIncome",
                table: "SimulationSessions");

            migrationBuilder.DropTable(
                name: "SimulationEconomicMacros");

            migrationBuilder.DropTable(
                name: "EconomicMacros");

        }
    }
}
