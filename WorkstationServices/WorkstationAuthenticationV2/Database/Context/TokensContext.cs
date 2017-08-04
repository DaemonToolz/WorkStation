using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WorkstationAuthenticationV2.Models;

namespace WorkstationAuthenticationV2.Database.Context
{
    public partial class TokensContext : DbContext
    {
        public virtual DbSet<Token> Token { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=PC-AXEL;Database=Tokens;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Token>(entity =>
            {
                entity.HasKey(e => e.Jni)
                    .HasName("PK__Token__DC90D50E31E5AA52");

                entity.Property(e => e.Jni)
                    .HasColumnName("jni")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Beg)
                    .HasColumnName("beg")
                    .HasColumnType("datetime");

                entity.Property(e => e.Boundmac)
                    .IsRequired()
                    .HasColumnName("boundmac")
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.Exp)
                    .HasColumnName("exp")
                    .HasColumnType("datetime");

                entity.Property(e => e.TKey)
                    .IsRequired()
                    .HasColumnName("t_key")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Token1)
                    .HasColumnName("token")
                    .HasColumnType("varchar(1024)");
            });
        }
    }
}