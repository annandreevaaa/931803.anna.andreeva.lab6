using Microsoft.EntityFrameworkCore;

namespace WebApplication6.Models
{
    public class AppdbContext : DbContext
    {
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<Forum> Forums { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }

        public AppdbContext (DbContextOptions<AppdbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public AppdbContext()
        {
           Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=dbLab6;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("ForumId").IncrementsBy(1);
            modelBuilder.HasSequence<int>("TopicId").IncrementsBy(1);
            modelBuilder.HasSequence<int>("PostId").IncrementsBy(1);
            modelBuilder.HasSequence<int>("FolderId").IncrementsBy(1);
            modelBuilder.HasSequence<int>("PictureId").IncrementsBy(1);

            modelBuilder.Entity<Forum>().Property(o => o.Id)
                        .HasDefaultValueSql("NEXT VALUE FOR ForumId");

            modelBuilder.Entity<Topic>().Property(o => o.Id)
                        .HasDefaultValueSql("NEXT VALUE FOR TopicId");

            modelBuilder.Entity<Post>().Property(o => o.Id)
                        .HasDefaultValueSql("NEXT VALUE FOR PostId");
            
            modelBuilder.Entity<Folder>().Property(o => o.Id)
                        .HasDefaultValueSql("NEXT VALUE FOR FolderId");
           
            modelBuilder.Entity<Picture>().Property(o => o.Id)
                        .HasDefaultValueSql("NEXT VALUE FOR PictureId");
        }
    }
}
