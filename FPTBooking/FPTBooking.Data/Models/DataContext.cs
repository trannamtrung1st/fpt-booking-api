using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FPTBooking.Data.Models
{
    public partial class DataContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>,
        AppUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppEvent> AppEvent { get; set; }
        public virtual DbSet<AreaMember> AreaMember { get; set; }
        public virtual DbSet<AttachedService> AttachedService { get; set; }
        public virtual DbSet<Booking> Booking { get; set; }
        public virtual DbSet<BookingHistory> BookingHistory { get; set; }
        public virtual DbSet<BookingService> BookingService { get; set; }
        public virtual DbSet<BookingUsage> BookingUsage { get; set; }
        public virtual DbSet<BuildingArea> BuildingArea { get; set; }
        public virtual DbSet<BuildingBlock> BuildingBlock { get; set; }
        public virtual DbSet<BuildingLevel> BuildingLevel { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DepartmentMember> DepartmentMember { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<RoomResource> RoomResource { get; set; }
        public virtual DbSet<RoomType> RoomType { get; set; }
        public virtual DbSet<RoomTypeService> RoomTypeService { get; set; }
        public virtual DbSet<UsageOfBooking> UsageOfBooking { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(DataConsts.CONN_STR);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.MemberCode).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.PhotoUrl).HasMaxLength(1000);
                entity.HasOne(d => d.Member)
                    .WithOne(p => p.User)
                    .HasForeignKey<Member>(d => d.UserId);
                entity.HasMany(d => d.UserRoles)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId);

                entity.HasData(new[]
                {
                    new AppUser
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Email = UserValues.LIB_EMAIL,
                        Id = UserValues.LIB_EMAIL.Replace('@', '_'),
                        UserName = UserValues.LIB_EMAIL,
                        LoggedIn = false,
                        MemberCode = UserValues.LIB_EMAIL.Split('@')[0],
                        NormalizedEmail = UserValues.LIB_EMAIL.ToUpperInvariant(),
                        NormalizedUserName = UserValues.LIB_EMAIL.ToUpperInvariant(),
                        SecurityStamp = Guid.NewGuid().ToString(),
                    },
                    new AppUser
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Email = UserValues.ADMIN_EMAIL,
                        Id = UserValues.ADMIN_EMAIL.Replace('@', '_'),
                        UserName = UserValues.ADMIN_EMAIL,
                        LoggedIn = false,
                        MemberCode = UserValues.ADMIN_EMAIL.Split('@')[0],
                        NormalizedEmail = UserValues.ADMIN_EMAIL.ToUpperInvariant(),
                        NormalizedUserName = UserValues.ADMIN_EMAIL.ToUpperInvariant(),
                        SecurityStamp = Guid.NewGuid().ToString(),
                    },
                });
            });

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(e => e.RoleId);

                entity.HasData(new AppRole[]
                {
                    new AppRole
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Id = RoleName.ADMIN,
                        Name = RoleName.ADMIN,
                        NormalizedName = RoleName.ADMIN.ToUpper()
                    },
                    new AppRole
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Id = RoleName.MANAGER,
                        Name = RoleName.MANAGER,
                        NormalizedName = RoleName.MANAGER.ToUpper()
                    },
                    new AppRole
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Id = RoleName.ROOM_CHECKER,
                        Name = RoleName.ROOM_CHECKER,
                        NormalizedName = RoleName.ROOM_CHECKER.ToUpper()
                    },
                    new AppRole
                    {
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        Id = RoleName.USER,
                        Name = RoleName.USER,
                        NormalizedName = RoleName.USER.ToUpper()
                    },
                });

            });

            modelBuilder.Entity<AppUserRole>(entity =>
            {
                entity.HasData(new[]
                {
                    new AppUserRole
                    {
                        RoleId = RoleName.MANAGER,
                        UserId = UserValues.LIB_EMAIL.Replace('@', '_'),
                    },
                    new AppUserRole
                    {
                        RoleId = RoleName.MANAGER,
                        UserId = UserValues.ADMIN_EMAIL.Replace('@', '_'),
                    },
                    new AppUserRole
                    {
                        RoleId = RoleName.ROOM_CHECKER,
                        UserId = UserValues.LIB_EMAIL.Replace('@', '_'),
                    },
                    new AppUserRole
                    {
                        RoleId = RoleName.ROOM_CHECKER,
                        UserId = UserValues.ADMIN_EMAIL.Replace('@', '_'),
                    },
                });
            });

            modelBuilder.Entity<AppEvent>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.DisplayContent).HasMaxLength(2000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppEvent)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AreaMember>(entity =>
            {
                entity.HasIndex(e => e.MemberId);

                entity.HasIndex(e => new { e.AreaCode, e.MemberId })
                    .IsUnique();

                entity.Property(e => e.AreaCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MemberId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.AreaMember)
                    .HasForeignKey(d => d.AreaCode);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.AreaMember)
                    .HasForeignKey(d => d.MemberId);

                entity.HasData(new[]
                {
                    new AreaMember
                    {
                        AreaCode = BuildingAreaValues.LIBRARY.Code,
                        Id = 1,
                        IsManager = true,
                        MemberId = UserValues.LIB_EMAIL.Replace('@', '_')
                    },
                    new AreaMember
                    {
                        AreaCode = BuildingAreaValues.ADMIN.Code,
                        Id = 2,
                        IsManager = true,
                        MemberId = UserValues.ADMIN_EMAIL.Replace('@', '_')
                    },
                });
            });

            modelBuilder.Entity<AttachedService>(entity =>
            {
                entity.HasIndex(e => e.BookingId);

                entity.HasIndex(e => e.BookingServiceCode);

                entity.Property(e => e.BookingServiceCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.AttachedService)
                    .HasForeignKey(d => d.BookingId);

                entity.HasOne(d => d.BookingService)
                    .WithMany(p => p.AttachedService)
                    .HasForeignKey(d => d.BookingServiceCode);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasIndex(e => e.BookMemberId);

                entity.HasIndex(e => e.RoomCode);

                entity.Property(e => e.BookMemberId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.ManagerMessage).HasMaxLength(2000);
                entity.Property(e => e.Feedback).HasMaxLength(2000);

                entity.Property(e => e.RoomCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UsingMemberIds).IsRequired();

                entity.HasOne(d => d.BookMember)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.BookMemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Booking)
                    .HasForeignKey(d => d.RoomCode);
            });

            modelBuilder.Entity<BookingHistory>(entity =>
            {
                entity.HasIndex(e => e.BookingId);

                entity.HasIndex(e => e.MemberId);

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.DisplayContent)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.FromStatus).HasMaxLength(50);

                entity.Property(e => e.MemberId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ToStatus).HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingHistory)
                    .HasForeignKey(d => d.BookingId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.BookingHistory)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BookingService>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasData(new[]
                {
                    new BookingService
                    {
                        Archived = false,
                        Code = "TB",
                        Name = "Tea break",
                        Description = "Tea break party in break time"
                    }
                });
            });

            modelBuilder.Entity<BookingUsage>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(2000);
            });

            modelBuilder.Entity<BuildingArea>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasData(new[]
                {
                    BuildingAreaValues.LIBRARY,
                    BuildingAreaValues.ADMIN,
                });
            });

            modelBuilder.Entity<BuildingBlock>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasData(new[]
                {
                    BuildingBlockValues.MAIN
                });
            });

            modelBuilder.Entity<BuildingLevel>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.HasIndex(e => e.BuildingBlockCode);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.BuildingBlockCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.BuildingBlock)
                    .WithMany(p => p.BuildingLevel)
                    .HasForeignKey(d => d.BuildingBlockCode);

                entity.HasData(new[]
                {
                    BuildingLevelValues.L1,
                    BuildingLevelValues.L2,
                    BuildingLevelValues.L3,
                    BuildingLevelValues.L4,
                });
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasData(new[]
                {
                    DeparmentValues.LIBRARY,
                    DeparmentValues.ADMIN,
                });
            });

            modelBuilder.Entity<DepartmentMember>(entity =>
            {
                entity.HasIndex(e => e.DepartmentCode);

                entity.HasIndex(e => e.MemberId);

                entity.Property(e => e.DepartmentCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsManager)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.MemberId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DepartmentMember)
                    .HasForeignKey(d => d.DepartmentCode);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.DepartmentMember)
                    .HasForeignKey(d => d.MemberId);

                entity.HasData(new[]
                {
                    new DepartmentMember
                    {
                        DepartmentCode = DeparmentValues.LIBRARY.Code,
                        Id = 1,
                        IsManager = true,
                        MemberId = UserValues.LIB_EMAIL.Replace('@', '_'),
                    },
                    new DepartmentMember
                    {
                        DepartmentCode = DeparmentValues.ADMIN.Code,
                        Id = 2,
                        IsManager = true,
                        MemberId = UserValues.ADMIN_EMAIL.Replace('@', '_'),
                    },
                });
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Code)
                    .HasMaxLength(100).IsUnicode(false);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .HasMaxLength(255);

                entity.Property(e => e.FullName)
                    .HasMaxLength(255);

                entity.Property(e => e.MiddleName).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Member)
                    .HasForeignKey<Member>(d => d.UserId);

                entity.HasData(new[]
                {
                    new Member
                    {
                        Code = UserValues.LIB_EMAIL.Split('@')[0],
                        Email = UserValues.LIB_EMAIL,
                        UserId = UserValues.LIB_EMAIL.Replace('@', '_'),
                    },
                    new Member
                    {
                        Code = UserValues.ADMIN_EMAIL.Split('@')[0],
                        Email = UserValues.ADMIN_EMAIL,
                        UserId = UserValues.ADMIN_EMAIL.Replace('@', '_'),
                    },
                });
            });

            var adminRoomRes = new List<RoomResource>();
            var resId = 1;
            Func<string, string, string, RoomResource> newRoomResForRoom = (code, name, roomCode) =>
            {
                return new RoomResource
                {
                    Code = code,
                    Name = name,
                    RoomCode = roomCode,
                    Id = resId++,
                    IsAvailable = true
                };
            };

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.HasIndex(e => e.BuildingAreaCode);

                entity.HasIndex(e => e.BuildingLevelCode);

                entity.HasIndex(e => e.DepartmentCode);

                entity.HasIndex(e => e.RoomTypeCode);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.HangingUserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BuildingAreaCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BuildingBlockCode)
                    .HasMaxLength(100);

                entity.Property(e => e.BuildingLevelCode)
                    .HasMaxLength(100);

                entity.Property(e => e.DepartmentCode)
                    .HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.RoomTypeCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.BuildingArea)
                    .WithMany(p => p.Room)
                    .HasForeignKey(d => d.BuildingAreaCode);

                entity.HasOne(d => d.BuildingLevel)
                    .WithMany(p => p.Room)
                    .HasForeignKey(d => d.BuildingLevelCode);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Room)
                    .HasForeignKey(d => d.DepartmentCode);

                entity.HasOne(d => d.RoomType)
                    .WithMany(p => p.Room)
                    .HasForeignKey(d => d.RoomTypeCode);

                var libRooms = new List<Room>()
                {
                    new Room
                    {
                        Code = "LB. 12",
                        ActiveFromTime = new TimeSpan(8,0,0),
                        ActiveToTime = new TimeSpan(17,0,0),
                        Description = "This room is managed by Library department.",
                        Name = "LB. 12",
                        PeopleCapacity = 10,
                    },
                    new Room
                    {
                        Code = "LB. 13",
                        ActiveFromTime = new TimeSpan(13,0,0),
                        ActiveToTime = new TimeSpan(17,0,0),
                        Description = "This room is Business room. Library department can only use the room from 13:00 to 17:00.",
                        Name = "LB. 13",
                        PeopleCapacity = 15,
                    },
                    new Room
                    {
                        Code = "LB. 15",
                        ActiveFromTime = new TimeSpan(8,0,0),
                        ActiveToTime = new TimeSpan(17,0,0),
                        Description = "This room is managed by Library department.",
                        Name = "LB. 15",
                        PeopleCapacity = 8,
                    },
                    new Room
                    {
                        Code = "LB. 21",
                        ActiveFromTime = new TimeSpan(8,0,0),
                        ActiveToTime = new TimeSpan(17,0,0),
                        Description = "This room is managed by Library department.",
                        Name = "LB. 21",
                        PeopleCapacity = 10,
                        BuildingLevelCode = BuildingLevelValues.L2.Code,
                    },
                    new Room
                    {
                        Code = "Seminar",
                        ActiveFromTime = new TimeSpan(8,0,0),
                        ActiveToTime = new TimeSpan(17,0,0),
                        Description = "This room is managed by Library department.",
                        Name = "Seminar",
                        PeopleCapacity = 80,
                    },
                };
                foreach (var r in libRooms)
                {
                    r.Archived = false;
                    r.BuildingAreaCode = BuildingAreaValues.LIBRARY.Code;
                    r.BuildingBlockCode = BuildingBlockValues.MAIN.Code;
                    r.BuildingLevelCode = r.BuildingLevelCode ?? BuildingLevelValues.L1.Code;
                    r.DepartmentCode = DeparmentValues.LIBRARY.Code;
                    r.RoomTypeCode = RoomTypeValues.LIBRARY.Code;
                    r.IsAvailable = true;
                }
                var adminRoomsGeneral = new List<ValueTuple<string, int, int>>()
                {
                    (BuildingLevelValues.L1.Code, 101,137),
                    (BuildingLevelValues.L2.Code, 201,234),
                    (BuildingLevelValues.L3.Code, 301,315),
                    (BuildingLevelValues.L4.Code, 401,422),
                };
                var adminRooms = adminRoomsGeneral.SelectMany(o =>
                {
                    var rooms = new List<Room>();
                    for (var i = o.Item2; i <= o.Item3; i++)
                    {
                        rooms.Add(new Room
                        {
                            Code = i.ToString(),
                            ActiveFromTime = new TimeSpan(6, 0, 0),
                            ActiveToTime = new TimeSpan(22, 0, 0),
                            Description = "This room is managed by Administrative department.",
                            Name = i.ToString(),
                            PeopleCapacity = 35,
                            Archived = false,
                            BuildingAreaCode = BuildingAreaValues.ADMIN.Code,
                            BuildingBlockCode = BuildingBlockValues.MAIN.Code,
                            BuildingLevelCode = o.Item1,
                            DepartmentCode = DeparmentValues.ADMIN.Code,
                            RoomTypeCode = RoomTypeValues.ADMIN.Code,
                            IsAvailable = true,
                        });
                        adminRoomRes.Add(
                            newRoomResForRoom("AC", "Air-conditioner", i.ToString()));
                        adminRoomRes.Add(
                            newRoomResForRoom("FURNITURE", "Furniture", i.ToString()));
                    }
                    return rooms;
                });

                entity.HasData(libRooms.Concat(adminRooms).ToList());
            });

            modelBuilder.Entity<RoomResource>(entity =>
            {
                entity.HasIndex(e => e.RoomCode);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RoomCode).HasMaxLength(100);

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.RoomResource)
                    .HasForeignKey(d => d.RoomCode);

                var libRooms = new List<RoomResource>()
                {
                    newRoomResForRoom("TV-DS","TV screen", "LB. 12"),
                    newRoomResForRoom("AC","Air-conditioner", "LB. 12"),
                    newRoomResForRoom("FURNITURE","Furniture", "LB. 12"),
                    newRoomResForRoom("TV-DS","TV screen", "LB. 13"),
                    newRoomResForRoom("AC","Air-conditioner", "LB. 13"),
                    newRoomResForRoom("FURNITURE","Furniture", "LB. 13"),
                    newRoomResForRoom("AC","Air-conditioner", "LB. 15"),
                    newRoomResForRoom("FURNITURE","Furniture", "LB. 15"),
                    newRoomResForRoom("TV-DS","TV screen", "LB. 21"),
                    newRoomResForRoom("AC","Air-conditioner", "LB. 21"),
                    newRoomResForRoom("FURNITURE","Furniture", "LB. 21"),
                    newRoomResForRoom("TV-DS","TV screen", "Seminar"),
                    newRoomResForRoom("AC","Air-conditioner", "Seminar"),
                    newRoomResForRoom("FURNITURE","Furniture", "Seminar"),
                };

                entity.HasData(adminRoomRes.Concat(libRooms).ToList());
            });

            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasData(new[]
                {
                    RoomTypeValues.LIBRARY,
                    RoomTypeValues.ADMIN,
                });
            });

            modelBuilder.Entity<RoomTypeService>(entity =>
            {
                entity.HasIndex(e => e.BookingServiceCode);

                entity.HasIndex(e => e.RoomTypeCode);

                entity.Property(e => e.BookingServiceCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RoomTypeCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.BookingService)
                    .WithMany(p => p.RoomTypeService)
                    .HasForeignKey(d => d.BookingServiceCode);

                entity.HasOne(d => d.RoomType)
                    .WithMany(p => p.RoomTypeService)
                    .HasForeignKey(d => d.RoomTypeCode);

                //entity.HasData(new[]
                //{
                //    new RoomTypeService
                //    {
                //        BookingServiceCode = "TB",
                //        Id = 1,
                //        RoomTypeCode = RoomTypeValues.LIBRARY.Code
                //    },
                //    new RoomTypeService
                //    {
                //        BookingServiceCode = "TB",
                //        Id = 2,
                //        RoomTypeCode = RoomTypeValues.CR_ADMIN.Code
                //    },
                //});
            });

            modelBuilder.Entity<UsageOfBooking>(entity =>
            {
                entity.HasIndex(e => e.BookingId);

                entity.HasIndex(e => e.BookingUsageId);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.UsageOfBooking)
                    .HasForeignKey(d => d.BookingId);

                entity.HasOne(d => d.BookingUsage)
                    .WithMany(p => p.UsageOfBooking)
                    .HasForeignKey(d => d.BookingUsageId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {

        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(DataConsts.CONN_STR);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
