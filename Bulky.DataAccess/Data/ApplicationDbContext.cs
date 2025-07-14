using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData( //add data to table
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Title",
                    Description = "Description",
                    ISBN = "Isbn",
                    Author = "Author",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 2,
                    Title = "Title2",
                    Description = "Description2",
                    ISBN = "Isbn2",
                    Author = "Author2",
                    ListPrice = 98,
                    Price = 89,
                    Price50 = 84,
                    Price100 = 79,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 5,
                    Title = "Title3",
                    Description = "Description3",
                    ISBN = "Isbn3",
                    Author = "Author3",
                    ListPrice = 97,
                    Price = 88,
                    Price50 = 83,
                    Price100 = 78,
                    CategoryId = 5,
                }
                );
        }
    }
}
