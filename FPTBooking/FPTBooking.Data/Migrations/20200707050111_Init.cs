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
                    PhotoUrl = table.Column<string>(maxLength: 1000, nullable: true)
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
                    Archived = table.Column<bool>(nullable: false),
                    BuildingBlockCode = table.Column<string>(maxLength: 100, nullable: false)
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
                name: "MemberType",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberType", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCategory", x => x.Id);
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
                name: "Member",
                columns: table => new
                {
                    UserId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(maxLength: 255, nullable: false),
                    LastName = table.Column<string>(maxLength: 255, nullable: false),
                    FullName = table.Column<string>(maxLength: 255, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 255, nullable: true),
                    MemberTypeCode = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Member_MemberType_MemberTypeCode",
                        column: x => x.MemberTypeCode,
                        principalTable: "MemberType",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Code = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    ResourceCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceCategory_Resource",
                        column: x => x.ResourceCategoryId,
                        principalTable: "ResourceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "AreaLevel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaCode = table.Column<string>(maxLength: 100, nullable: true),
                    LevelCode = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaLevel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaLevel_BuildingArea_AreaCode",
                        column: x => x.AreaCode,
                        principalTable: "BuildingArea",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AreaLevel_BuildingLevel_LevelCode",
                        column: x => x.LevelCode,
                        principalTable: "BuildingLevel",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
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
                    BuildingLevelCode = table.Column<string>(maxLength: 100, nullable: false),
                    BuildingBlockCode = table.Column<string>(maxLength: 100, nullable: false),
                    DepartmentCode = table.Column<string>(maxLength: 100, nullable: false),
                    AreaSize = table.Column<double>(nullable: true),
                    PeopleCapacity = table.Column<int>(nullable: false),
                    HangingStartTime = table.Column<DateTime>(nullable: true),
                    HangingEndTime = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    IsAvailable = table.Column<bool>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_Department_DepartmentCode",
                        column: x => x.DepartmentCode,
                        principalTable: "Department",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Room_RoomType_RoomTypeCode",
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
                name: "AreaManager",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaCode = table.Column<string>(maxLength: 100, nullable: false),
                    MemberId = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaManager", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AreaManager_BuildingArea_AreaCode",
                        column: x => x.AreaCode,
                        principalTable: "BuildingArea",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaManager_Member_MemberId",
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
                    RejectedReason = table.Column<string>(maxLength: 2000, nullable: true),
                    CanceledReason = table.Column<string>(maxLength: 2000, nullable: true),
                    Archived = table.Column<bool>(nullable: false),
                    BookMemberId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    UsingMemberId = table.Column<string>(nullable: false)
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
                    Status = table.Column<string>(nullable: true),
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
                name: "RoomService",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCode = table.Column<string>(maxLength: 100, nullable: false),
                    RoomCode = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomService_Room_RoomCode",
                        column: x => x.RoomCode,
                        principalTable: "Room",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomService_BookingService_ServiceCode",
                        column: x => x.ServiceCode,
                        principalTable: "BookingService",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachedService",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingServiceCode = table.Column<string>(maxLength: 100, nullable: false),
                    BookingId = table.Column<int>(nullable: false),
                    NumOfPeople = table.Column<int>(nullable: false),
                    Note = table.Column<string>(maxLength: 2000, nullable: true),
                    Status = table.Column<string>(maxLength: 50, nullable: true),
                    AdminMessage = table.Column<string>(maxLength: 2000, nullable: true)
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
                    { "Administrator", "a7fae252-9e78-4431-9c9c-e18ae4d38ead", "Administrator", "ADMINISTRATOR" },
                    { "Manager", "5727951f-36df-4a98-9561-0b15e77ec6d9", "Manager", "MANAGER" },
                    { "RoomChecker", "0b2c320d-41a4-4f42-b334-1e77011a2157", "RoomChecker", "ROOMCHECKER" },
                    { "User", "7f0e67f1-5783-4edc-9e1a-6a93cfcf652b", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "MemberType",
                columns: new[] { "Code", "Archived", "Description", "Name" },
                values: new object[] { "GENERAL", false, "General member type of user", "General" });

            migrationBuilder.CreateIndex(
                name: "IX_AppEvent_UserId",
                table: "AppEvent",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaLevel_AreaCode",
                table: "AreaLevel",
                column: "AreaCode");

            migrationBuilder.CreateIndex(
                name: "IX_AreaLevel_LevelCode",
                table: "AreaLevel",
                column: "LevelCode");

            migrationBuilder.CreateIndex(
                name: "IX_AreaManager_MemberId",
                table: "AreaManager",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_AreaManager_AreaCode_MemberId",
                table: "AreaManager",
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
                name: "IX_Member_MemberTypeCode",
                table: "Member",
                column: "MemberTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ResourceCategoryId",
                table: "Resource",
                column: "ResourceCategoryId");

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
                name: "IX_RoomService_RoomCode",
                table: "RoomService",
                column: "RoomCode");

            migrationBuilder.CreateIndex(
                name: "IX_RoomService_ServiceCode",
                table: "RoomService",
                column: "ServiceCode");

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
                name: "AreaLevel");

            migrationBuilder.DropTable(
                name: "AreaManager");

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
                name: "Resource");

            migrationBuilder.DropTable(
                name: "RoomResource");

            migrationBuilder.DropTable(
                name: "RoomService");

            migrationBuilder.DropTable(
                name: "RoomTypeService");

            migrationBuilder.DropTable(
                name: "UsageOfBooking");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ResourceCategory");

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
                name: "MemberType");

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
