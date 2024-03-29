﻿// <auto-generated />
using CurriculoOnLineMVC.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CurriculoOnLineMVC.Migrations
{
    [DbContext(typeof(CurriculoOnLineDbContext))]
    partial class CurriculoOnLineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("CurriculoOnLineMVC.Models.Candidato", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Nome")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Candidatos");
                });

            modelBuilder.Entity("CurriculoOnLineMVC.Models.CandidatoPerfil", b =>
                {
                    b.Property<int>("CandidatoId");

                    b.Property<int>("PerfilId");

                    b.HasKey("CandidatoId", "PerfilId");

                    b.HasIndex("PerfilId");

                    b.ToTable("CandidatoPerfils");
                });

            modelBuilder.Entity("CurriculoOnLineMVC.Models.Perfil", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Descricao")
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Perfis");
                });

            modelBuilder.Entity("CurriculoOnLineMVC.Models.CandidatoPerfil", b =>
                {
                    b.HasOne("CurriculoOnLineMVC.Models.Candidato", "Candidato")
                        .WithMany("Perfis")
                        .HasForeignKey("CandidatoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CurriculoOnLineMVC.Models.Perfil", "Perfil")
                        .WithMany("Candidatos")
                        .HasForeignKey("PerfilId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
