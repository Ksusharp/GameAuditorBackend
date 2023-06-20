using Core.CommonModels.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Db.Ef
{
    public class ApplicationContext : IdentityDbContext<User>
    {

        public DbSet<Post> Posts { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<TagNavigation> TagNavigation { get; set; }

        public DbSet<User> User { get; set; }


        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO Переписать на appsettings.json
            //Нужно для создания миграций вот это получал из файла настроек в апсетинге
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=testdb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Post>().Property(x => x.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Post>().Property(x => x.UpdatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Tag>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
            modelBuilder.Entity<Tag>().HasIndex(x => x.TagName).IsUnique();
            modelBuilder.Entity<TagNavigation>().Property(x => x.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
