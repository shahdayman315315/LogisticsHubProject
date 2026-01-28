using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogisticsHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogisticsHub.Infrastructure.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<WithDrawalRequest> WithDrawalRequests { get; set; }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<ApplicationUser>(entity => 
                {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();

                    entity.HasOne(e => e.Merchant).WithOne(m=>m.User).
                    HasForeignKey<Merchant>(m=>m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                    entity.HasMany(e=>e.Orders).WithOne(o=>o.User).HasForeignKey(o=>o.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e=>e.Wallet).WithOne(w=>w.User).HasForeignKey<Wallet>(w=>w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e=>e.RefreshTokens).WithOne(r=>r.User).HasForeignKey(r=>r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                }
            );

            modelbuilder.Entity<Merchant>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e=>e.CommersialRegister).HasMaxLength(50).IsRequired();

                entity.HasIndex(e => e.CommersialRegister).IsUnique();

               
                entity.HasMany(e=>e.Stores).WithOne(s=>s.Merchant).HasForeignKey(s=>s.MerchantId)
                .OnDelete(DeleteBehavior.Cascade);
            }
            
           );

            modelbuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.CommissionRate).
                HasColumnType("Decimal(5,2)").HasDefaultValue(0.00);

                entity.ToTable(t => t.HasCheckConstraint("CK_StoreCommissionRate", "[CommissionRate] >= 0 AND [CommissionRate] <= 100"));

                entity.HasMany(e=>e.Products).WithOne(p=>p.Store).HasForeignKey(p=>p.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            modelbuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Balance).HasColumnType("Decimal(18,2)").HasDefaultValue(0.00);

                entity.ToTable(t => t.HasCheckConstraint("CK_BalanceValue", "[Balance] >= 0 ") );

               
                entity.HasMany(e=>e.Transactions).WithOne(t=>t.Wallet).HasForeignKey(t=>t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            modelbuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

                entity.Property(e => e.StockQuantity).IsRequired().HasDefaultValue(0);

                    entity.ToTable(t =>
                    {
                        t.HasCheckConstraint("CK_StockQuantityValue", "[StockQuantity] >= 0");
                        t.HasCheckConstraint("CK_PriceValue", "[Price] >= 0");

                    });

                entity.HasOne(e=>e.Category).WithMany(p=>p.Products).HasForeignKey(p=>p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e=>e.orderItems).WithOne(or=>or.Product).HasForeignKey(or=>or.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelbuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name).HasMaxLength(150).IsRequired();
            });


            modelbuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TotalAmount).HasColumnType("Decimal(18,2)");

                entity.Property(e => e.ShippingAddress).HasMaxLength(500);

                entity.HasIndex(e=>e.StripeSessionId).IsUnique().HasFilter("[StripeSessionId] IS NOT NULL");

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_TotalAmountValue", "[TotalAmount] >= 0");
                    t.HasCheckConstraint("PlatformCommission", "[PlatformCommission] >= 0");

                });

                entity.HasMany(e => e.OrderItems).WithOne(oi => oi.Order).HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            });


            modelbuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UnitPrice).HasColumnType("Decimal(18,2)").IsRequired();

                entity.Property(e => e.Quantity).IsRequired();


                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_UnitPriceValue", "[UnitPrice] >= 0");
                    t.HasCheckConstraint("CK_QuantityValue", "[Quantity] >= 0");

                });
            });


            modelbuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e=>e.Description).HasMaxLength(500);

                entity.Property(e => e.Amount).HasColumnType("Decimal(18,2)").IsRequired();

                entity.Property(e => e.ExternalReferenceId).HasMaxLength(255);

                entity.ToTable(t => t.HasCheckConstraint("CK_AmountValue", "[Amount] >= 0"));

            });

            modelbuilder.Entity<WithDrawalRequest>().HasOne(wd=>wd.Wallet).WithMany()
                .HasForeignKey(wd=>wd.WalletId).OnDelete(DeleteBehavior.Cascade);
        }







    }
}
