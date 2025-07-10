using Microsoft.EntityFrameworkCore;
using Share.Models;

namespace Backend.Context;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Occupancy> Occupancies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Spot> Spots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersRole> UsersRoles { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.IdBuilding).HasName("PK__Building__934A4474B30E6BE8");

            entity.ToTable("Building", "Parking");

            entity.Property(e => e.IdBuilding).HasColumnName("id_building");
            entity.Property(e => e.MaximumCapacity)
                .HasDefaultValue(0)
                .HasColumnName("maximum_capacity");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK__Logs__6CC851FEAA238DAD");

            entity.ToTable("Logs", "Auth");

            entity.Property(e => e.IdLog).HasColumnName("id_log");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("action");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("license_plate");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<Occupancy>(entity =>
        {
            entity.HasKey(e => e.IdOccupancy).HasName("PK__Occupanc__5464BB0D21A4AC7D");

            entity.ToTable("Occupancy", "Parking");

            entity.Property(e => e.IdOccupancy).HasColumnName("id_occupancy");
            entity.Property(e => e.IdSpot).HasColumnName("id_spot");
            entity.Property(e => e.IdVehicle).HasColumnName("id_vehicle");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.IdSpotNavigation).WithMany(p => p.Occupancies)
                .HasForeignKey(d => d.IdSpot)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Occupancy__id_sp__7D439ABD");

            entity.HasOne(d => d.IdVehicleNavigation).WithMany(p => p.Occupancies)
                .HasForeignKey(d => d.IdVehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Occupancy__id_ve__7E37BEF6");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__Roles__3D48441DA73CE02A");

            entity.ToTable("Roles", "Auth");

            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Spot>(entity =>
        {
            entity.HasKey(e => e.IdSpot).HasName("PK__Spots__5DB20C909604425F");

            entity.ToTable("Spots", "Parking");

            entity.HasIndex(e => e.Code, "UQ__Spots__357D4CF9E2013E7A").IsUnique();

            entity.Property(e => e.IdSpot).HasColumnName("id_spot");
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.IdBuilding).HasColumnName("id_building");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.IdBuildingNavigation).WithMany(p => p.Spots)
                .HasForeignKey(d => d.IdBuilding)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Spots__id_buildi__74AE54BC");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__D2D1463778437B5D");

            entity.ToTable("Users", "Auth");

            entity.HasIndex(e => e.Identification, "UQ__Users__AAA7C1F5FE4C900D").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616432DF7BF2").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Identification)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("identification");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Salt)
                .HasMaxLength(255)
                .HasColumnName("salt");
        });

        modelBuilder.Entity<UsersRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole).HasName("PK__UsersRol__4FD2ABB3149ABD05");

            entity.ToTable("UsersRoles", "Auth");

            entity.HasIndex(e => new { e.IdUser, e.IdRole }, "UQ_user_role").IsUnique();

            entity.Property(e => e.IdUserRole).HasColumnName("id_user_role");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UR_role");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UR_user");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.IdVehicle).HasName("PK__Vehicles__6DF73CE40016EBDC");

            entity.ToTable("Vehicles", "Parking");

            entity.HasIndex(e => e.LicensePlate, "UQ__Vehicles__F72CD56E0AE47DC6").IsUnique();

            entity.Property(e => e.IdVehicle).HasColumnName("id_vehicle");
            entity.Property(e => e.Accommodation).HasColumnName("accommodation");
            entity.Property(e => e.Brand)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("brand");
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("color");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("license_plate");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("model");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicles__id_use__797309D9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
