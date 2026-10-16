using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models.Entities;

namespace RaceDay.API.Data
{//start of namespace

    public class RaceDayDbContext : DbContext
    {//start of class
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Organiser> Organisers => Set<Organiser>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {//start of OnModelCreating

            // User - unique email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(20);

            // Organiser - 1:1 with User
            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organiser)
                .HasForeignKey<Organiser>(o => o.OrganiserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Participant - 1:1 with User
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event - *:1 with Organiser
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganiserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category - *:1 with Event
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrolment - *:1 with Participant
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Participant)
                .WithMany(p => p.Enrolments)
                .HasForeignKey(en => en.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrolment - *:1 with Category (NO ACTION to prevent multiple cascade paths)
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(en => en.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Unique enrolment (prevent duplicates)
            modelBuilder.Entity<Enrolment>()
                .HasIndex(en => new { en.ParticipantId, en.CategoryId })
                .IsUnique();

            // Result - 1:1 with Enrolment
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(en => en.Result)
                .HasForeignKey<Result>(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Result>()
                .HasIndex(r => r.EnrolmentId)
                .IsUnique();
        }//end of OnModelCreating

    }//end of class
}//end of namespace
