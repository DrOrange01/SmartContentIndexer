using SmartContentIndexer.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SmartContentIndexer.Infrastructure.Data
{
    public class SmartContentDbContext : DbContext
    {
        public SmartContentDbContext(DbContextOptions<SmartContentDbContext> options) : base(options) { }

        public DbSet<FileItem> Files { get; set; }
        public DbSet<IndexingJob> IndexingJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FileItem configuration
            modelBuilder.Entity<FileItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.FilePath).IsUnique();
                entity.HasIndex(e => e.FileType);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.LastIndexed);

                entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
                entity.Property(e => e.FileName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FileExtension).HasMaxLength(10);
                entity.Property(e => e.Summary).HasMaxLength(1000);
                entity.Property(e => e.Category).HasMaxLength(100);

                // Store collections as JSON
                entity.Property(e => e.Keywords)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

                entity.Property(e => e.Tags)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

                entity.Property(e => e.Metadata)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>());

                // Store embeddings as binary data
                entity.Property(e => e.TextEmbedding)
                    .HasConversion(
                        v => v == null ? null : Convert.ToBase64String(MemoryMarshal.AsBytes(v.AsSpan()).ToArray()),
                        v => v == null ? null : MemoryMarshal.Cast<byte, float>(Convert.FromBase64String(v)).ToArray());

                entity.Property(e => e.ImageEmbedding)
                    .HasConversion(
                        v => v == null ? null : Convert.ToBase64String(MemoryMarshal.AsBytes(v.AsSpan()).ToArray()),
                        v => v == null ? null : MemoryMarshal.Cast<byte, float>(Convert.FromBase64String(v)).ToArray());
            });

            // IndexingJob configuration
            modelBuilder.Entity<IndexingJob>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Started);

                entity.Property(e => e.DirectoryPath).HasMaxLength(500).IsRequired();

                entity.Property(e => e.FilePatterns)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

                entity.Property(e => e.Errors)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
            });
        }
    }
}