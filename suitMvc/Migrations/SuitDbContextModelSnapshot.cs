﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using suitMvc.Data;

#nullable disable

namespace suitMvc.Migrations
{
    [DbContext(typeof(SuitDbContext))]
    partial class SuitDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("suitMvc.Models.Flayer", b =>
                {
                    b.Property<int>("flayer_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("flayer_id"));

                    b.Property<int>("fecha")
                        .HasColumnType("int");

                    b.Property<string>("imagen")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("usuario_id")
                        .HasColumnType("int");

                    b.HasKey("flayer_id");

                    b.HasIndex("usuario_id");

                    b.ToTable("Flayers");
                });

            modelBuilder.Entity("suitMvc.Models.Invitados", b =>
                {
                    b.Property<int>("invitado_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("invitado_id"));

                    b.Property<int>("acompanantes")
                        .HasColumnType("int");

                    b.Property<string>("apellido")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("consumiciones")
                        .HasColumnType("int");

                    b.Property<int>("entrada_free")
                        .HasColumnType("int");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("paso")
                        .HasColumnType("int");

                    b.Property<int>("pulsera")
                        .HasColumnType("int");

                    b.Property<int>("usuario_id")
                        .HasColumnType("int");

                    b.HasKey("invitado_id");

                    b.HasIndex("usuario_id");

                    b.ToTable("Invitados");
                });

            modelBuilder.Entity("suitMvc.Models.Usuarios", b =>
                {
                    b.Property<int>("usuario_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("usuario_id"));

                    b.Property<int>("admin")
                        .HasColumnType("int");

                    b.Property<string>("apellido")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("cajero")
                        .HasColumnType("int");

                    b.Property<string>("contrasena")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("nombre")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("usuario")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("usuario_id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("suitMvc.Models.Flayer", b =>
                {
                    b.HasOne("suitMvc.Models.Usuarios", "Usuarios")
                        .WithMany()
                        .HasForeignKey("usuario_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("suitMvc.Models.Invitados", b =>
                {
                    b.HasOne("suitMvc.Models.Usuarios", "Usuarios")
                        .WithMany()
                        .HasForeignKey("usuario_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
