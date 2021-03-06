﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Voicipher.DataAccess;

namespace Voicipher.DataAccess.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Voicipher.Domain.Models.Administrator", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Administrator");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.AudioFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateProcessedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPermanentlyDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPhoneCall")
                        .HasColumnType("bit");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("OriginalSourceFileName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("RecognitionState")
                        .HasColumnType("int");

                    b.Property<string>("SourceFileName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Storage")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("TotalTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("TranscribedTime")
                        .HasColumnType("time");

                    b.Property<int>("UploadStatus")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("WasCleaned")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AudioFile");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.BackgroundJob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Attempt")
                        .HasColumnType("int");

                    b.Property<Guid>("AudioFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCompletedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Exception")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("JobState")
                        .HasColumnType("int");

                    b.Property<string>("Parameters")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("BackgroundJob");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.BillingPurchase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AutoRenewing")
                        .HasColumnType("bit");

                    b.Property<string>("ConsumptionState")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Platform")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("PurchaseId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PurchaseState")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("PurchaseToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TransactionDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("BillingPurchase");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.ContactForm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("ContactForm");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.CurrentUserSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<long>("Ticks")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("CurrentUserSubscription");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.DeletedAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateDeletedUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("DeletedAccount");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.FileChunk", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AudioFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FileChunk");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.InformationMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CampaignName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DatePublishedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("WasOpened")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("InformationMessage");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.InternalValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Key")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Value")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("InternalValue");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.LanguageVersion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("InformationMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Language")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<bool>("SentOnAndroid")
                        .HasColumnType("bit");

                    b.Property<bool>("SentOnOsx")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("InformationMessageId");

                    b.ToTable("LanguageVersion");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.RecognizedAudioSample", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("RecognizedAudioSample");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.SpeechResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DisplayText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RecognizedAudioSampleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("TotalTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("RecognizedAudioSampleId");

                    b.ToTable("SpeechResult");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.TranscribeItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Alternatives")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AudioFileId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<bool>("IsIncomplete")
                        .HasColumnType("bit");

                    b.Property<string>("SourceFileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("Storage")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("TotalTime")
                        .HasColumnType("time");

                    b.Property<string>("UserTranscript")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("WasCleaned")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AudioFileId");

                    b.ToTable("TranscribeItem");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateRegisteredUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FamilyName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("GivenName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.UserDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateRegisteredUtc")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("InstallationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("InstalledVersionNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Language")
                        .HasColumnType("int");

                    b.Property<int>("RuntimePlatform")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserDevice");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.UserSubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Operation")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSubscription");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.AudioFile", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.User", null)
                        .WithMany("AudioFiles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.BillingPurchase", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.User", null)
                        .WithMany("BillingPurchases")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.CurrentUserSubscription", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.User", null)
                        .WithOne("CurrentUserSubscription")
                        .HasForeignKey("Voicipher.Domain.Models.CurrentUserSubscription", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.LanguageVersion", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.InformationMessage", null)
                        .WithMany("LanguageVersions")
                        .HasForeignKey("InformationMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.SpeechResult", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.RecognizedAudioSample", null)
                        .WithMany("SpeechResults")
                        .HasForeignKey("RecognizedAudioSampleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.TranscribeItem", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.AudioFile", "AudioFile")
                        .WithMany("TranscribeItems")
                        .HasForeignKey("AudioFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AudioFile");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.UserDevice", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.User", null)
                        .WithMany("UserDevices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.UserSubscription", b =>
                {
                    b.HasOne("Voicipher.Domain.Models.User", null)
                        .WithMany("UserSubscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Voicipher.Domain.Models.AudioFile", b =>
                {
                    b.Navigation("TranscribeItems");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.InformationMessage", b =>
                {
                    b.Navigation("LanguageVersions");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.RecognizedAudioSample", b =>
                {
                    b.Navigation("SpeechResults");
                });

            modelBuilder.Entity("Voicipher.Domain.Models.User", b =>
                {
                    b.Navigation("AudioFiles");

                    b.Navigation("BillingPurchases");

                    b.Navigation("CurrentUserSubscription");

                    b.Navigation("UserDevices");

                    b.Navigation("UserSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
