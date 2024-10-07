using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using ProductsProtoLibrary.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsProtoLibrary.Helpers
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        static DecimalValue ConvertToDecimalValueType(decimal input)
        {
            var units = (long)input;
            var nanos = (Int32)((input - units) * 1_000_000_000M);
            return new DecimalValue { Units = units, Nanos = nanos };
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var decimalConverter = new ValueConverter<DecimalValue, decimal>(
                v => (v.Units + (v.Nanos / 1_000_000_000m)),
                v => ConvertToDecimalValueType(v)
                );

            var intToShortConverter = new ValueConverter<int, short>(
                v => (short)v,
                v => (int)v);

            modelBuilder.Entity<Product>()
                .Property(c => c.UnitPrice)
                .HasConversion(decimalConverter);
            modelBuilder.Entity<Product>()
                .Property(c => c.UnitsInStock)
                .HasConversion(intToShortConverter);
            modelBuilder.Entity<Product>()
                .HasKey(c => c.ProductId);
        }

    }
}
