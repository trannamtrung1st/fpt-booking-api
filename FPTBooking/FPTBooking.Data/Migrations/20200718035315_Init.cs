using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FPTBooking.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(maxLength: 255, nullable: true),
                    PhotoUrl = table.Column<string>(maxLength: 1000, nullable: true),
                    MemberCode = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    LoggedIn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingService",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingService", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "BookingUsage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingUsage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BuildingArea",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingArea", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "BuildingBlock",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingBlock", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "RoomType",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomType", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    UserId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(maxLength: 255, nullable: true),
                    LastName = table.Column<string>(maxLength: 255, nullable: true),
                    FullName = table.Column<string>(maxLength: 255, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Member_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuildingLevel",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    BuildingBlockCode = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingLevel", x => x.Code);
                    table.ForeignKey(
                        name: "FK_BuildingLevel_BuildingBlock_BuildingBlockCode",
                        column: x => x.BuildingBlockCode,
                        principalTable: "BuildingBlock",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypeService",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingServiceCode = table.Column<string>(maxLength: 100, nullable: false),
                    RoomTypeCode = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypeService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomTypeService_BookingService_BookingServiceCode",
                        column: x => x.BookingServiceCode,
                        principalTable: "BookingService",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomTypeService_RoomType_RoomTypeCode",
                        column: x => x.RoomTypeCode,
                        principalTable: "RoomType",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppEvent",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayContent = table.Column<string>(maxLength: 2000, nullable: true),
                    UserId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    HappenedTime = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEvent_Member_UserId",
                        column: x => x.UserId,
                        principalTable: "Member",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaMember",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaCode = table.Column<string>(maxLength: 100, nullable: false),
                    MemberId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    IsManager = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaMember_BuildingArea_AreaCode",
                        column: x => x.AreaCode,
                        principalTable: "BuildingArea",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaMember_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentMember",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    DepartmentCode = table.Column<string>(maxLength: 100, nullable: false),
                    IsManager = table.Column<bool>(nullable: false, defaultValueSql: "(CONVERT([bit],(0)))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentMember_Department_DepartmentCode",
                        column: x => x.DepartmentCode,
                        principalTable: "Department",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentMember_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    RoomTypeCode = table.Column<string>(maxLength: 100, nullable: false),
                    BuildingAreaCode = table.Column<string>(maxLength: 100, nullable: false),
                    BuildingLevelCode = table.Column<string>(maxLength: 100, nullable: true),
                    BuildingBlockCode = table.Column<string>(maxLength: 100, nullable: true),
                    DepartmentCode = table.Column<string>(maxLength: 100, nullable: true),
                    AreaSize = table.Column<double>(nullable: true),
                    PeopleCapacity = table.Column<int>(nullable: false),
                    HangingStartTime = table.Column<DateTime>(nullable: true),
                    HangingEndTime = table.Column<DateTime>(nullable: true),
                    ActiveFromTime = table.Column<TimeSpan>(nullable: false),
                    ActiveToTime = table.Column<TimeSpan>(nullable: false),
                    HangingUserId = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false),
                    Note = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Room_BuildingArea_BuildingAreaCode",
                        column: x => x.BuildingAreaCode,
                        principalTable: "BuildingArea",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_BuildingLevel_BuildingLevelCode",
                        column: x => x.BuildingLevelCode,
                        principalTable: "BuildingLevel",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_Department_DepartmentCode",
                        column: x => x.DepartmentCode,
                        principalTable: "Department",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Room_RoomType_RoomTypeCode",
                        column: x => x.RoomTypeCode,
                        principalTable: "RoomType",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    SentDate = table.Column<DateTime>(nullable: false),
                    BookedDate = table.Column<DateTime>(nullable: false),
                    NumOfPeople = table.Column<int>(nullable: false),
                    Note = table.Column<string>(maxLength: 2000, nullable: true),
                    FromTime = table.Column<TimeSpan>(nullable: false),
                    ToTime = table.Column<TimeSpan>(nullable: false),
                    RoomCode = table.Column<string>(maxLength: 100, nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: false),
                    Archived = table.Column<bool>(nullable: false),
                    BookMemberId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    UsingMemberIds = table.Column<string>(nullable: false),
                    ManagerMessage = table.Column<string>(maxLength: 2000, nullable: true),
                    Feedback = table.Column<string>(maxLength: 2000, nullable: true),
                    DepartmentAccepted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Member_BookMemberId",
                        column: x => x.BookMemberId,
                        principalTable: "Member",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Booking_Room_RoomCode",
                        column: x => x.RoomCode,
                        principalTable: "Room",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomResource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false),
                    RoomCode = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomResource_Room_RoomCode",
                        column: x => x.RoomCode,
                        principalTable: "Room",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachedService",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingServiceCode = table.Column<string>(maxLength: 100, nullable: false),
                    BookingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachedService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachedService_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttachedService_BookingService_BookingServiceCode",
                        column: x => x.BookingServiceCode,
                        principalTable: "BookingService",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingHistory",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayContent = table.Column<string>(maxLength: 2000, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    Data = table.Column<string>(nullable: true),
                    FromStatus = table.Column<string>(maxLength: 50, nullable: true),
                    ToStatus = table.Column<string>(maxLength: 50, nullable: true),
                    BookingId = table.Column<int>(nullable: false),
                    MemberId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    HappenedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingHistory_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingHistory_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsageOfBooking",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(nullable: false),
                    BookingUsageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageOfBooking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsageOfBooking_Booking_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Booking",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsageOfBooking_BookingUsage_BookingUsageId",
                        column: x => x.BookingUsageId,
                        principalTable: "BookingUsage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "Administrator", "25a2acab-0da3-41ca-ab9d-c2ab27f7e59b", "Administrator", "ADMINISTRATOR" },
                    { "Manager", "178ffb87-c240-417f-9526-763eb29f7b70", "Manager", "MANAGER" },
                    { "RoomChecker", "7f78b3ae-a76c-4365-b3ba-1eaf6fa8b799", "RoomChecker", "ROOMCHECKER" },
                    { "User", "7e3cfde4-3781-48d3-b082-745b5b88b88b", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "LoggedIn", "MemberCode", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "TraPTP@fpt.edu.vn", 0, "c52a3749-058a-457a-8275-fe657fd7d0b8", "TraPTP@fpt.edu.vn", false, null, false, null, false, "TraPTP", "TRAPTP@FPT.EDU.VN", "TRAPTP@FPT.EDU.VN", null, null, false, null, "5597a2b1-5421-47fd-b60b-b45106fa7855", false, "TraPTP@fpt.edu.vn" },
                    { "utnt@fpt.edu.vn", 0, "8a4588e8-807b-4a66-811c-6c0377883b50", "utnt@fpt.edu.vn", false, null, false, null, false, "utnt", "UTNT@FPT.EDU.VN", "UTNT@FPT.EDU.VN", null, null, false, null, "e07afedc-21c1-4f0b-97e8-4c90f9df1e0a", false, "utnt@fpt.edu.vn" }
                });

            migrationBuilder.InsertData(
                table: "BookingService",
                columns: new[] { "Code", "Archived", "Description", "Name" },
                values: new object[] { "TB", false, "Tea break party in break time", "Tea break" });

            migrationBuilder.InsertData(
                table: "BuildingArea",
                columns: new[] { "Code", "Archived", "Description", "Name" },
                values: new object[,]
                {
                    { "LB", false, "Library", "Library" },
                    { "ADMIN", false, "Administrative", "Administrative" }
                });

            migrationBuilder.InsertData(
                table: "BuildingBlock",
                columns: new[] { "Code", "Archived", "Description", "Name" },
                values: new object[] { "MAIN", false, null, "Main building" });

            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "Code", "Archived", "Description", "Name" },
                values: new object[,]
                {
                    { "LB", false, "Library", "Library" },
                    { "ADMIN", false, "Administrative", "Administrative" }
                });

            migrationBuilder.InsertData(
                table: "RoomType",
                columns: new[] { "Code", "Archived", "Description", "Name" },
                values: new object[,]
                {
                    { "LB", false, "Library", "Library" },
                    { "ADMIN", false, "Administrative", "Administrative" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "TraPTP@fpt.edu.vn", "Manager" },
                    { "TraPTP@fpt.edu.vn", "RoomChecker" },
                    { "utnt@fpt.edu.vn", "Manager" },
                    { "utnt@fpt.edu.vn", "RoomChecker" }
                });

            migrationBuilder.InsertData(
                table: "BuildingLevel",
                columns: new[] { "Code", "Archived", "BuildingBlockCode", "Description", "Name" },
                values: new object[,]
                {
                    { "F1", false, "MAIN", null, "1st floor" },
                    { "F2", false, "MAIN", null, "2nd floor" },
                    { "F3", false, "MAIN", null, "3rd floor" },
                    { "F4", false, "MAIN", null, "4th floor" }
                });

            migrationBuilder.InsertData(
                table: "Member",
                columns: new[] { "UserId", "Code", "Email", "FirstName", "FullName", "LastName", "MiddleName", "Phone" },
                values: new object[,]
                {
                    { "TraPTP@fpt.edu.vn", "TraPTP", "TraPTP@fpt.edu.vn", null, null, null, null, null },
                    { "utnt@fpt.edu.vn", "utnt", "utnt@fpt.edu.vn", null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "AreaMember",
                columns: new[] { "Id", "AreaCode", "IsManager", "MemberId" },
                values: new object[,]
                {
                    { 1, "LB", true, "TraPTP@fpt.edu.vn" },
                    { 2, "ADMIN", true, "utnt@fpt.edu.vn" }
                });

            migrationBuilder.InsertData(
                table: "DepartmentMember",
                columns: new[] { "Id", "DepartmentCode", "IsManager", "MemberId" },
                values: new object[,]
                {
                    { 1, "LB", true, "TraPTP@fpt.edu.vn" },
                    { 2, "ADMIN", true, "utnt@fpt.edu.vn" }
                });

            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Code", "ActiveFromTime", "ActiveToTime", "Archived", "AreaSize", "BuildingAreaCode", "BuildingBlockCode", "BuildingLevelCode", "DepartmentCode", "Description", "HangingEndTime", "HangingStartTime", "HangingUserId", "IsAvailable", "Name", "Note", "PeopleCapacity", "RoomTypeCode" },
                values: new object[,]
                {
                    { "306", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "306", null, 35, "ADMIN" },
                    { "305", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "305", null, 35, "ADMIN" },
                    { "304", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "304", null, 35, "ADMIN" },
                    { "303", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "303", null, 35, "ADMIN" },
                    { "302", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "302", null, 35, "ADMIN" },
                    { "301", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "301", null, 35, "ADMIN" },
                    { "234", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "234", null, 35, "ADMIN" },
                    { "233", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "233", null, 35, "ADMIN" },
                    { "232", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "232", null, 35, "ADMIN" },
                    { "231", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "231", null, 35, "ADMIN" },
                    { "230", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "230", null, 35, "ADMIN" },
                    { "229", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "229", null, 35, "ADMIN" },
                    { "227", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "227", null, 35, "ADMIN" },
                    { "307", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "307", null, 35, "ADMIN" },
                    { "226", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "226", null, 35, "ADMIN" },
                    { "225", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "225", null, 35, "ADMIN" },
                    { "224", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "224", null, 35, "ADMIN" },
                    { "223", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "223", null, 35, "ADMIN" },
                    { "222", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "222", null, 35, "ADMIN" },
                    { "221", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "221", null, 35, "ADMIN" },
                    { "220", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "220", null, 35, "ADMIN" },
                    { "219", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "219", null, 35, "ADMIN" },
                    { "218", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "218", null, 35, "ADMIN" },
                    { "217", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "217", null, 35, "ADMIN" },
                    { "228", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "228", null, 35, "ADMIN" },
                    { "308", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "308", null, 35, "ADMIN" },
                    { "310", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "310", null, 35, "ADMIN" },
                    { "216", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "216", null, 35, "ADMIN" },
                    { "420", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "420", null, 35, "ADMIN" },
                    { "419", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "419", null, 35, "ADMIN" },
                    { "418", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "418", null, 35, "ADMIN" },
                    { "417", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "417", null, 35, "ADMIN" },
                    { "416", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "416", null, 35, "ADMIN" },
                    { "415", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "415", null, 35, "ADMIN" },
                    { "414", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "414", null, 35, "ADMIN" },
                    { "413", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "413", null, 35, "ADMIN" },
                    { "412", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "412", null, 35, "ADMIN" },
                    { "411", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "411", null, 35, "ADMIN" },
                    { "410", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "410", null, 35, "ADMIN" },
                    { "409", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "409", null, 35, "ADMIN" },
                    { "408", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "408", null, 35, "ADMIN" },
                    { "407", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "407", null, 35, "ADMIN" },
                    { "406", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "406", null, 35, "ADMIN" },
                    { "405", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "405", null, 35, "ADMIN" },
                    { "404", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "404", null, 35, "ADMIN" },
                    { "403", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "403", null, 35, "ADMIN" },
                    { "402", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "402", null, 35, "ADMIN" },
                    { "401", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "401", null, 35, "ADMIN" },
                    { "315", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "315", null, 35, "ADMIN" },
                    { "314", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "314", null, 35, "ADMIN" },
                    { "313", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "313", null, 35, "ADMIN" },
                    { "312", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "312", null, 35, "ADMIN" },
                    { "311", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "311", null, 35, "ADMIN" },
                    { "309", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F3", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "309", null, 35, "ADMIN" },
                    { "215", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "215", null, 35, "ADMIN" },
                    { "213", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "213", null, 35, "ADMIN" },
                    { "421", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "421", null, 35, "ADMIN" },
                    { "121", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "121", null, 35, "ADMIN" },
                    { "120", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "120", null, 35, "ADMIN" },
                    { "119", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "119", null, 35, "ADMIN" },
                    { "118", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "118", null, 35, "ADMIN" },
                    { "117", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "117", null, 35, "ADMIN" },
                    { "116", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "116", null, 35, "ADMIN" },
                    { "115", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "115", null, 35, "ADMIN" },
                    { "114", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "114", null, 35, "ADMIN" },
                    { "113", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "113", null, 35, "ADMIN" },
                    { "112", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "112", null, 35, "ADMIN" },
                    { "111", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "111", null, 35, "ADMIN" },
                    { "110", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "110", null, 35, "ADMIN" },
                    { "109", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "109", null, 35, "ADMIN" },
                    { "108", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "108", null, 35, "ADMIN" },
                    { "107", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "107", null, 35, "ADMIN" },
                    { "106", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "106", null, 35, "ADMIN" },
                    { "105", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "105", null, 35, "ADMIN" },
                    { "104", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "104", null, 35, "ADMIN" },
                    { "103", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "103", null, 35, "ADMIN" },
                    { "102", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "102", null, 35, "ADMIN" },
                    { "101", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "101", null, 35, "ADMIN" },
                    { "Seminar", new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), false, null, "LB", "MAIN", "F1", "LB", "This room is managed by Library department.", null, null, null, true, "Seminar", null, 80, "LB" },
                    { "LB. 15", new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), false, null, "LB", "MAIN", "F1", "LB", "This room is managed by Library department.", null, null, null, true, "LB. 15", null, 8, "LB" },
                    { "LB. 13", new TimeSpan(0, 13, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), false, null, "LB", "MAIN", "F1", "LB", "This room is Business room. Library department can only use the room from 13:00 to 17:00.", null, null, null, true, "LB. 13", null, 15, "LB" },
                    { "LB. 12", new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), false, null, "LB", "MAIN", "F1", "LB", "This room is managed by Library department.", null, null, null, true, "LB. 12", null, 10, "LB" },
                    { "122", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "122", null, 35, "ADMIN" },
                    { "214", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "214", null, 35, "ADMIN" },
                    { "123", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "123", null, 35, "ADMIN" },
                    { "125", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "125", null, 35, "ADMIN" },
                    { "212", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "212", null, 35, "ADMIN" },
                    { "211", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "211", null, 35, "ADMIN" },
                    { "210", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "210", null, 35, "ADMIN" },
                    { "209", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "209", null, 35, "ADMIN" },
                    { "208", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "208", null, 35, "ADMIN" },
                    { "207", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "207", null, 35, "ADMIN" },
                    { "206", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "206", null, 35, "ADMIN" },
                    { "205", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "205", null, 35, "ADMIN" },
                    { "204", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "204", null, 35, "ADMIN" },
                    { "203", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "203", null, 35, "ADMIN" },
                    { "202", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "202", null, 35, "ADMIN" },
                    { "201", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F2", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "201", null, 35, "ADMIN" },
                    { "LB. 21", new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 17, 0, 0, 0), false, null, "LB", "MAIN", "F2", "LB", "This room is managed by Library department.", null, null, null, true, "LB. 21", null, 10, "LB" },
                    { "137", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "137", null, 35, "ADMIN" },
                    { "136", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "136", null, 35, "ADMIN" },
                    { "135", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "135", null, 35, "ADMIN" },
                    { "134", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "134", null, 35, "ADMIN" },
                    { "133", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "133", null, 35, "ADMIN" },
                    { "132", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "132", null, 35, "ADMIN" },
                    { "131", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "131", null, 35, "ADMIN" },
                    { "130", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "130", null, 35, "ADMIN" },
                    { "129", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "129", null, 35, "ADMIN" },
                    { "128", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "128", null, 35, "ADMIN" },
                    { "127", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "127", null, 35, "ADMIN" },
                    { "126", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "126", null, 35, "ADMIN" },
                    { "124", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F1", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "124", null, 35, "ADMIN" },
                    { "422", new TimeSpan(0, 6, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, null, "ADMIN", "MAIN", "F4", "ADMIN", "This room is managed by Administrative department.", null, null, null, true, "422", null, 35, "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEvent_UserId",
                table: "AppEvent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaMember_MemberId",
                table: "AreaMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaMember_AreaCode_MemberId",
                table: "AreaMember",
                columns: new[] { "AreaCode", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedService_BookingId",
                table: "AttachedService",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachedService_BookingServiceCode",
                table: "AttachedService",
                column: "BookingServiceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_BookMemberId",
                table: "Booking",
                column: "BookMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_RoomCode",
                table: "Booking",
                column: "RoomCode");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistory_BookingId",
                table: "BookingHistory",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistory_MemberId",
                table: "BookingHistory",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_BuildingLevel_BuildingBlockCode",
                table: "BuildingLevel",
                column: "BuildingBlockCode");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMember_DepartmentCode",
                table: "DepartmentMember",
                column: "DepartmentCode");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentMember_MemberId",
                table: "DepartmentMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_BuildingAreaCode",
                table: "Room",
                column: "BuildingAreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_Room_BuildingLevelCode",
                table: "Room",
                column: "BuildingLevelCode");

            migrationBuilder.CreateIndex(
                name: "IX_Room_DepartmentCode",
                table: "Room",
                column: "DepartmentCode");

            migrationBuilder.CreateIndex(
                name: "IX_Room_RoomTypeCode",
                table: "Room",
                column: "RoomTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_RoomResource_RoomCode",
                table: "RoomResource",
                column: "RoomCode");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeService_BookingServiceCode",
                table: "RoomTypeService",
                column: "BookingServiceCode");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypeService_RoomTypeCode",
                table: "RoomTypeService",
                column: "RoomTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_UsageOfBooking_BookingId",
                table: "UsageOfBooking",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_UsageOfBooking_BookingUsageId",
                table: "UsageOfBooking",
                column: "BookingUsageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEvent");

            migrationBuilder.DropTable(
                name: "AreaMember");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AttachedService");

            migrationBuilder.DropTable(
                name: "BookingHistory");

            migrationBuilder.DropTable(
                name: "DepartmentMember");

            migrationBuilder.DropTable(
                name: "RoomResource");

            migrationBuilder.DropTable(
                name: "RoomTypeService");

            migrationBuilder.DropTable(
                name: "UsageOfBooking");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BookingService");

            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "BookingUsage");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BuildingArea");

            migrationBuilder.DropTable(
                name: "BuildingLevel");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "RoomType");

            migrationBuilder.DropTable(
                name: "BuildingBlock");
        }
    }
}
