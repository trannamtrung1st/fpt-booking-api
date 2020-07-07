using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FPTBooking.Data.Models
{
    public partial class DataContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppEvent> AppEvent { get; set; }
        public virtual DbSet<AreaManager> AreaManager { get; set; }
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
        public virtual DbSet<MemberType> MemberType { get; set; }
        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<ResourceCategory> ResourceCategory { get; set; }
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
                entity.Property(e => e.PhotoUrl).HasMaxLength(1000);
                entity.HasOne(d => d.Member)
                    .WithOne(p => p.User)
                    .HasForeignKey<Member>(d => d.UserId);
            });
            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);

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

            modelBuilder.Entity<AreaManager>(entity =>
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
                    .WithMany(p => p.AreaManager)
                    .HasForeignKey(d => d.AreaCode);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.AreaManager)
                    .HasForeignKey(d => d.MemberId);
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

                entity.Property(e => e.CanceledReason).HasMaxLength(2000);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.RejectedReason).HasMaxLength(2000);

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
            });

            modelBuilder.Entity<BuildingBlock>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
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
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
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
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.MemberTypeCode);

                entity.Property(e => e.UserId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.MemberTypeCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.MiddleName).HasMaxLength(255);

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.HasOne(d => d.MemberType)
                    .WithMany(p => p.Member)
                    .HasForeignKey(d => d.MemberTypeCode);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Member)
                    .HasForeignKey<Member>(d => d.UserId);
            });

            modelBuilder.Entity<MemberType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasData(new MemberType[]
                {
                    new MemberType
                    {
                        Archived = false,
                        Code = "GENERAL",
                        Name = "General",
                        Description = "General member type of user"
                    }
                });
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasIndex(e => e.ResourceCategoryId);

                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.HasOne(d => d.ResourceCategory)
                    .WithMany(p => p.Resource)
                    .HasForeignKey(d => d.ResourceCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceCategory_Resource");
            });

            modelBuilder.Entity<ResourceCategory>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.HasIndex(e => e.BuildingAreaCode);

                entity.HasIndex(e => e.BuildingLevelCode);

                entity.HasIndex(e => e.DepartmentCode);

                entity.HasIndex(e => e.RoomTypeCode);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.BuildingAreaCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BuildingBlockCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.BuildingLevelCode)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DepartmentCode)
                    .IsRequired()
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
            });

            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
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
