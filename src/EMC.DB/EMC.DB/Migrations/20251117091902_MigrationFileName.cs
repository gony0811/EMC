using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EMC.DB.Migrations
{
    public partial class MigrationFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alarms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Enable = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Action = table.Column<string>(maxLength: 512, nullable: true),
                    LastRaisedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Type = table.Column<string>(nullable: false),
                    InstanceName = table.Column<string>(maxLength: 200, nullable: true),
                    FileName = table.Column<string>(maxLength: 200, nullable: true),
                    IsUse = table.Column<bool>(nullable: false),
                    Args = table.Column<string>(maxLength: 1000, nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Axis = table.Column<string>(maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "strftime('%Y-%m-%dT%H:%M:%fZ','now')"),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Password = table.Column<string>(maxLength: 200, nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Screens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Path = table.Column<string>(maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsEnabled = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Screens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValueType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlarmHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlarmId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlarmHistories_Alarms_AlarmId",
                        column: x => x.AlarmId,
                        principalTable: "Alarms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MotionPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    MotionId = table.Column<int>(nullable: false),
                    CurrentLocation = table.Column<double>(type: "REAL", nullable: false),
                    MinimumLocation = table.Column<double>(type: "REAL", nullable: false),
                    MaximumLocation = table.Column<double>(type: "REAL", nullable: false),
                    CurrentSpeed = table.Column<int>(type: "REAL", nullable: false),
                    MinimumSpeed = table.Column<int>(type: "INTEGER", nullable: false),
                    MaximumSpeed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotionPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotionPositions_Motions_MotionId",
                        column: x => x.MotionId,
                        principalTable: "Motions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleManageRole",
                columns: table => new
                {
                    ManagerRoleId = table.Column<int>(nullable: false),
                    TargetRoleId = table.Column<int>(nullable: false),
                    CanManage = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleManageRole", x => new { x.ManagerRoleId, x.TargetRoleId });
                    table.ForeignKey(
                        name: "FK_RoleManageRole_Roles_ManagerRoleId",
                        column: x => x.ManagerRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleManageRole_Roles_TargetRoleId",
                        column: x => x.TargetRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleScreenAccess",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    ScreenId = table.Column<int>(nullable: false),
                    Granted = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleScreenAccess", x => new { x.RoleId, x.ScreenId });
                    table.ForeignKey(
                        name: "FK_RoleScreenAccess_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleScreenAccess_Screens_ScreenId",
                        column: x => x.ScreenId,
                        principalTable: "Screens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeParam",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Maximum = table.Column<string>(nullable: true),
                    Minimum = table.Column<string>(nullable: true),
                    ValueTypeId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeParam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeParam_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeParam_Unit_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Unit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RecipeParam_ValueType_ValueTypeId",
                        column: x => x.ValueTypeId,
                        principalTable: "ValueType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlarmHistories_UpdateTime",
                table: "AlarmHistories",
                column: "UpdateTime");

            migrationBuilder.CreateIndex(
                name: "IX_AlarmHistories_AlarmId_UpdateTime",
                table: "AlarmHistories",
                columns: new[] { "AlarmId", "UpdateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_Code",
                table: "Alarms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_LastRaisedAt",
                table: "Alarms",
                column: "LastRaisedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_Name",
                table: "Alarms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Device_Name",
                table: "Device",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotionPositions_MotionId_Name",
                table: "MotionPositions",
                columns: new[] { "MotionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motions_Name",
                table: "Motions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeParam_RecipeId",
                table: "RecipeParam",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeParam_UnitId",
                table: "RecipeParam",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeParam_ValueTypeId",
                table: "RecipeParam",
                column: "ValueTypeId");

            migrationBuilder.CreateIndex(
                name: "UX_Recipes_OnlyOneActive",
                table: "Recipes",
                column: "IsActive",
                unique: true,
                filter: "IsActive = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_Name",
                table: "Recipes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleManageRole_TargetRoleId",
                table: "RoleManageRole",
                column: "TargetRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleScreenAccess_RoleId",
                table: "RoleScreenAccess",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleScreenAccess_ScreenId",
                table: "RoleScreenAccess",
                column: "ScreenId");

            migrationBuilder.CreateIndex(
                name: "IX_Screens_Code",
                table: "Screens",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unit_Name",
                table: "Unit",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValueType_Name",
                table: "ValueType",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmHistories");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "MotionPositions");

            migrationBuilder.DropTable(
                name: "RecipeParam");

            migrationBuilder.DropTable(
                name: "RoleManageRole");

            migrationBuilder.DropTable(
                name: "RoleScreenAccess");

            migrationBuilder.DropTable(
                name: "Alarms");

            migrationBuilder.DropTable(
                name: "Motions");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "ValueType");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Screens");
        }
    }
}
