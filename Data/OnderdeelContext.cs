using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using MySql.EntityFrameworkCore;
using System.Data;
using MySql.EntityFrameworkCore.Extensions;

public class OnderdeelContext : DbContext
{
    public DbSet<Onderdeel> Onderdelen { get; set; }
    public DbSet<OnderwegOnderdeel> OnderwegOnderdelen { get; set; }
    public DbSet<InstallatieOnderdeel> InstallatieOnderdelen { get; set; }
    public string DbPath { get; }
    public OnderdeelContext()
    {
        var path = AppContext.BaseDirectory;
        DbPath = Path.Join(path, "OnderdelenDb.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Onderdeel>().UseTptMappingStrategy();
        modelBuilder.Entity<Onderdeel>().HasKey(c => c.OnderdeelID);
        //set the identity column: identity increment and identity seed
        modelBuilder.Entity<Onderdeel>().Property(c => c.OnderdeelID);
    }
}

