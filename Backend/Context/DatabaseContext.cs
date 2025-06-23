using Microsoft.EntityFrameworkCore;
using Share.EntityModels.Auth;

namespace Backend.Context;

public partial class DatabaseContext : DbContext
{
    DatabaseContext() { }

    // Receives options for configuring the database connection.
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }
        

    // Maps properties for each entity in the database.
    public virtual DbSet<Log> Logs { get; set; }
    public virtual DbSet<Resource> Resources { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<RoleResource> RolesResources { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> UsersRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // LOG ===================================================================
        modelBuilder.Entity<Log>(entity =>
        {
            // Sets primary key for the Log entity
            entity.HasKey(e => e.IdLog).HasName("PK__Logs__6CC851FE3159EB8F");

            // Maps the entity to the "Logs" table in the "Auth" schema
            entity.ToTable("Logs", "Auth");

            // Configures the properties of the Log entities
            entity.Property(e => e.IdLog).HasColumnName("id_log");

            entity.Property(e => e.Action)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("action");

            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            // Configures the relationship with the User entity
            entity.HasOne(d => d.IdUserNavigation)
                .WithMany(p => p.Logs)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__Logs__id_user__45F365D3");
        });


        // RESOURCE ==============================================================

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.IdResource).HasName("PK_resource");
            entity.ToTable("Resources", "Auth");

            entity.Property(e => e.IdResource).HasColumnName("id_resource");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.Property(e => e.ResourceType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("resource_type");
        });

        // ROLE ==================================================================
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK_role");
            entity.ToTable("Roles", "Auth");

            entity.Property(e => e.IdRole).HasColumnName("id_role");

            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });


        // RESOURCE-ROLE =========================================================
        modelBuilder.Entity<RoleResource>(entity =>
        {
            entity.HasKey(e => e.IdRoleResource).HasName("PK__Roles_Re__60ED0A77D17E597E");
            entity.ToTable("RolesResources", "Auth");

            entity.Property(e => e.IdRoleResource).HasColumnName("id_role_resource");
            entity.Property(e => e.IdResource).HasColumnName("id_resource");
            entity.Property(e => e.IdRole).HasColumnName("id_role");

            entity.HasOne(d => d.IdResourceNavigation)
                .WithMany(p => p.RolesResources)
                .HasForeignKey(d => d.IdResource)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Roles_Res__id_re__4222D4EF");

            entity.HasOne(d => d.IdRoleNavigation)
                .WithMany(p => p.RolesResources)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Roles_Res__id_ro__412EB0B6");
        });

        // USER ==================================================================

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK_user");
            entity.ToTable("Users", "Auth");

            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.Property(e => e.Salt)
                .HasMaxLength(255)
                .HasColumnName("salt");

            entity.HasIndex(e => e.Username).IsUnique();
        });

        // USER-ROLE =============================================================
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole).HasName("PK_user_role");
            entity.ToTable("UsersRoles", "Auth");

            entity.Property(e => e.IdUserRole).HasColumnName("id_user_role");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdRoleNavigation)
                .WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UR_role");

            entity.HasOne(d => d.IdUserNavigation)
                .WithMany(p => p.UsersRoles)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UR_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
