using Lenovo.NAT.Infrastructure.Entities;
using Lenovo.NAT.Infrastructure.Entities.Admin;
using Lenovo.NAT.Infrastructure.Entities.Logistic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lenovo.NAT.Infrastructure.Context
{
    public class ThinkToolContext : IdentityDbContext<User>
    {
        protected readonly IConfiguration _configuration;

        public ThinkToolContext(DbContextOptions<ThinkToolContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           // builder.HasDefaultSchema("Identity");
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "User");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");

            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

            

            builder.Entity<UserAccessHistory>().HasKey(o => new { o.Id });    

            builder.Entity<Country>().HasKey(o => new { o.Id });
            builder.Entity<UserModuleAccess>().HasKey(o => new { o.UserId, o.ModuleId });
            builder.Entity<Module>().HasKey(o => new { o.Id });
            

            builder.Entity<Picking>().HasKey(o => new { o.Id });
            builder.Entity<PickingInvoice>().HasKey(o => new { o.Id });
            builder.Entity<PickingItem>().HasKey(o => new { o.Id });
            builder.Entity<PickingProcessType>().HasKey(o => new { o.Id });
            builder.Entity<PickingArea>().HasKey(o => new { o.Id });
            builder.Entity<PickingBrand>().HasKey(o => new { o.Id });
            builder.Entity<PickingStatus>().HasKey(o => new { o.Id });
            builder.Entity<PickingReason>().HasKey(o => new { o.Id });
            builder.Entity<PickingCarrier>().HasKey(o => new { o.Id });


            builder.Entity<LogisticInvoice>().HasKey(o => new { o.Id });



        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                            .Build();

            var connectionString = configuration.GetSection("ConnectionString")["ThinkToolConn"];
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<IdentityRole> IdentityRole { get; set; }
        public DbSet<UserModuleAccess> UserModuleAccess { get; set; }
        public DbSet<UserAccessHistory> UserAccessHistory { get; set; }
        public DbSet<Picking> Picking { get; set; }
        public DbSet<PickingInvoice> PickingInvoice { get; set; }
        public DbSet<PickingItem> PickingItem { get; set; }
        public DbSet<PickingProcessType> PickingProcessType { get; set; }
        public DbSet<PickingArea> PickingArea { get; set; }
        public DbSet<PickingBrand> PickingBrand { get; set; }
        public DbSet<PickingStatus> PickingStatus { get; set; }
        public DbSet<PickingReason> PickingReason { get; set; }
        public DbSet<PickingCarrier> PickingCarrier { get; set; }
      
        public DbSet<LogisticInvoice> LogisticInvoice { get; set; }

        public DbSet<OrderType> OrderType { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<NFType> NFType { get; set; }
        public DbSet<CustomerSegment> CustomerSegment { get; set; }

        // Order Entities - MIGRAÇÃO COMPLETA
        public DbSet<OrderNotLoaded> OrderNotLoaded { get; set; }
        public DbSet<OrderAttachment> OrderAttachment { get; set; }
        public DbSet<OrderSoldTO> OrderSoldTO { get; set; }
        public DbSet<OrderShipTo> OrderShipTo { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }
        public DbSet<OrderHistory> OrderHistory { get; set; }


        }
}
