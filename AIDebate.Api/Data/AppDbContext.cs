using AIDebate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AIDebate.Api.Data;

public class AppDbContext: DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) :base(options){}

  public DbSet<User> Users => Set<User>();
  public DbSet<Debate> Debates => Set<Debate>();
  public  DbSet<Turn> Turns => Set<Turn>();
  public DbSet<Verdict> Verdicts => Set<Verdict>();

  protected override void OnModelCreating(ModelBuilder modelBuilder){
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<User>()
    .HasIndex(u=>u.Email)
    .IsUnique();


    modelBuilder.Entity<Debate>()
    .HasOne(s=>s.User)
    .WithMany(u=>u.Debates)
    .HasForeignKey(s=>s.UserId)
    .OnDelete(DeleteBehavior.Cascade);


    modelBuilder.Entity<Turn>()
    .HasOne(s=>s.Debate)
    .WithMany(d=>d.Turns)
    .HasForeignKey(s=>s.DebateId)
    .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Verdict>()
    .HasOne(s=>s.Debate)
    .WithOne(d=>d.Verdict)
    .HasForeignKey<Verdict>(s=>s.DebateId)
    .OnDelete(DeleteBehavior.Cascade);



  }







  }




