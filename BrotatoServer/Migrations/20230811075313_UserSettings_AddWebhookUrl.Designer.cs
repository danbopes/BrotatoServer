﻿// <auto-generated />
using System;
using BrotatoServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BrotatoServer.Migrations
{
    [DbContext(typeof(BrotatoServerContext))]
    [Migration("20230811075313_UserSettings_AddWebhookUrl")]
    partial class UserSettings_AddWebhookUrl
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.9");

            modelBuilder.Entity("BrotatoServer.Models.DB.Run", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("CurrentRotation")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomData")
                        .HasColumnType("TEXT");

                    b.Property<long>("Date")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RunInformation")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TwitchClip")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Won")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Run");
                });

            modelBuilder.Entity("BrotatoServer.Models.DB.User", b =>
                {
                    b.Property<ulong>("SteamId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ApiKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomData")
                        .HasColumnType("TEXT");

                    b.Property<bool>("JoinedChat")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TwitchAccessToken")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("TwitchId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TwitchUsername")
                        .HasColumnType("TEXT");

                    b.HasKey("SteamId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BrotatoServer.Models.DB.UserSettings", b =>
                {
                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClipDelaySeconds")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ClipOnRunLost")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ClipOnRunWon")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OnRunLostMessage")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("TEXT");

                    b.Property<string>("OnRunStartedMessage")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("TEXT");

                    b.Property<string>("OnRunWonMessage")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("TEXT");

                    b.Property<string>("WebhookUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("BrotatoServer.Models.DB.Run", b =>
                {
                    b.HasOne("BrotatoServer.Models.DB.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BrotatoServer.Models.DB.UserSettings", b =>
                {
                    b.HasOne("BrotatoServer.Models.DB.User", null)
                        .WithOne("Settings")
                        .HasForeignKey("BrotatoServer.Models.DB.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BrotatoServer.Models.DB.User", b =>
                {
                    b.Navigation("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
