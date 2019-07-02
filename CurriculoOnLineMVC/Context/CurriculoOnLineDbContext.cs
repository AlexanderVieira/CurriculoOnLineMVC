using CurriculoOnLineMVC.Context.Conventions;
using CurriculoOnLineMVC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurriculoOnLineMVC.Context
{
    public class CurriculoOnLineDbContext: DbContext
    {
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<Perfil> Perfis { get; set; }

        public DbSet<CandidatoPerfil> CandidatoPerfils { get; set; }
        public CurriculoOnLineDbContext(DbContextOptions<CurriculoOnLineDbContext> options):base(options)
        {

        }        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var mb = ModelBuilderExtensions.RemovePluralizingTableNameConvention(modelBuilder);
            //mb.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<CandidatoPerfil>(candidatoPerfil =>
            {
                candidatoPerfil.HasKey(cp => new { cp.CandidatoId, cp.PerfilId });

                candidatoPerfil.HasOne(cp => cp.Candidato)
                    .WithMany(c => c.Perfis)
                    .HasForeignKey(cp => cp.CandidatoId)
                    .IsRequired();

                candidatoPerfil.HasOne(cp => cp.Perfil)
                    .WithMany(p => p.Candidatos)
                    .HasForeignKey(cp => cp.PerfilId)
                    .IsRequired();
            }
            );
        }
       
    }
}
