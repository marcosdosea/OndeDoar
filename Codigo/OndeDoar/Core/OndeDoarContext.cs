using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class OndeDoarContext : DbContext
{
    public OndeDoarContext()
    {
    }

    public OndeDoarContext(DbContextOptions<OndeDoarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Avaliacao> Avaliacaos { get; set; }

    public virtual DbSet<Doador> Doadors { get; set; }

    public virtual DbSet<Instituicaodoador> Instituicaodoadors { get; set; }

    public virtual DbSet<Necessidade> Necessidades { get; set; }

    public virtual DbSet<Notificacao> Notificacaos { get; set; }

    public virtual DbSet<Postagem> Postagems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=123456;database=ondedoar");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avaliacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("avaliacao");

            entity.HasIndex(e => e.IdDoador, "fk_Avaliacao_Doador1_idx");

            entity.HasIndex(e => e.IdInstituicao, "fk_Avaliacao_Instituicao1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comentario)
                .HasMaxLength(100)
                .HasColumnName("comentario");
            entity.Property(e => e.Data)
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.IdDoador).HasColumnName("idDoador");
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.Nota).HasColumnName("nota");

            entity.HasOne(d => d.IdDoadorNavigation).WithMany(p => p.Avaliacaos)
                .HasForeignKey(d => d.IdDoador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Avaliacao_Doador1");
        });

        modelBuilder.Entity<Doador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doador");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(40)
                .HasColumnName("nome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(11)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Instituicaodoador>(entity =>
        {
            entity.HasKey(e => new { e.IdInstituicao, e.IdDoador }).HasName("PRIMARY");

            entity.ToTable("instituicaodoador");

            entity.HasIndex(e => e.IdDoador, "fk_Instituicao_has_Doador_Doador1_idx");

            entity.HasIndex(e => e.IdInstituicao, "fk_Instituicao_has_Doador_Instituicao1_idx");

            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.IdDoador).HasColumnName("idDoador");

            entity.HasOne(d => d.IdDoadorNavigation).WithMany(p => p.Instituicaodoadors)
                .HasForeignKey(d => d.IdDoador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Instituicao_has_Doador_Doador1");
        });

        modelBuilder.Entity<Necessidade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("necessidade");

            entity.HasIndex(e => e.IdInstituicao, "fk_Necessidade_Instituicao1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(40)
                .HasColumnName("categoria");
            entity.Property(e => e.Descricao)
                .HasMaxLength(100)
                .HasColumnName("descricao");
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.Nome)
                .HasMaxLength(40)
                .HasColumnName("nome");
            entity.Property(e => e.Prioridade)
                .HasColumnType("enum('Alta','Média','Baixa')")
                .HasColumnName("prioridade");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("status");
        });

        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notificacao");

            entity.HasIndex(e => e.IdInstituicao, "fk_Notificacao_Instituicao1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descricao)
                .HasMaxLength(250)
                .HasColumnName("descricao");
            entity.Property(e => e.IdInstituicao).HasColumnName("idInstituicao");
            entity.Property(e => e.Titulo)
                .HasMaxLength(25)
                .HasColumnName("titulo");
        });

        modelBuilder.Entity<Postagem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("postagem");

            entity.HasIndex(e => e.InstituicaoId, "fk_Postagem_Instituicao_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("datetime")
                .HasColumnName("data");
            entity.Property(e => e.Descricao)
                .HasMaxLength(250)
                .HasColumnName("descricao");
            entity.Property(e => e.InstituicaoId).HasColumnName("Instituicao_id");
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .HasColumnName("titulo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
