using ApiColecao.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiColecao.Context;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Livro>? Livros { get; set; }
    public DbSet<Dvd>? Dvds { get; set; }
    public DbSet<Cd>? Cds { get; set; }
    public DbSet<Categoria>? Categorias { get; set; }

    protected override void OnModelCreating(ModelBuilder mb) {

        mb.Entity<Livro>().HasKey(l => l.LivroId);
        mb.Entity<Livro>().Property(l => l.Nome).HasMaxLength(100).IsRequired();
        mb.Entity<Livro>().Property(l => l.Descricao).HasMaxLength(200);
        mb.Entity<Livro>().Property(l => l.Quantidade).IsRequired();


        mb.Entity<Dvd>().HasKey(d => d.DvdId);
        mb.Entity<Dvd>().Property(d => d.Nome).HasMaxLength(100).IsRequired();
        mb.Entity<Dvd>().Property(d => d.Descricao).HasMaxLength(200);
        mb.Entity<Dvd>().Property(d => d.Quantidade).IsRequired();

        mb.Entity<Cd>().HasKey(c => c.CdId);
        mb.Entity<Cd>().Property(c => c.Nome).HasMaxLength(100).IsRequired();
        mb.Entity<Cd>().Property(c => c.Descricao).HasMaxLength(200);
        mb.Entity<Cd>().Property(c => c.Quantidade).IsRequired();

        mb.Entity<Categoria>().HasKey(c => c.CategoriaId);
        mb.Entity<Categoria>().Property(c => c.Nome).HasMaxLength(100).IsRequired();

        mb.Entity<Livro>().HasOne<Categoria>(c => c.Categoria).WithMany(l => l.Livros).HasForeignKey(c => c.CategoriaId);
        mb.Entity<Dvd>().HasOne<Categoria>(c => c.Categoria).WithMany(d => d.Dvds).HasForeignKey(c => c.CategoriaId);
        mb.Entity<Cd>().HasOne<Categoria>(c => c.Categoria).WithMany(c => c.Cds).HasForeignKey(c => c.CategoriaId);


    }

}