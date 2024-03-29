﻿using Microsoft.EntityFrameworkCore;
using Voicipher.DataAccess.EntitiesConfiguration;
using Voicipher.Domain.Models;

namespace Voicipher.DataAccess
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public DatabaseContext(string connectionString, DbContextOptions options)
            : base(options)
        {
            _connectionString = connectionString;
        }

        public DbSet<Administrator> Administrators { get; set; }

        public DbSet<AudioFile> AudioFiles { get; set; }

        public DbSet<TranscribeItem> TranscribeItems { get; set; }

        public DbSet<FileChunk> FileChunks { get; set; }

        public DbSet<RecognizedAudioSample> RecognizedAudioSamples { get; set; }

        public DbSet<SpeechResult> SpeechResults { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        public DbSet<CurrentUserSubscription> CurrentUserSubscriptions { get; set; }

        public DbSet<UserDevice> UserDevices { get; set; }

        public DbSet<DeletedAccount> DeletedAccounts { get; set; }

        public DbSet<BillingPurchase> BillingPurchases { get; set; }

        public DbSet<PurchaseStateTransaction> PurchaseStateTransactions { get; set; }

        public DbSet<InformationMessage> InformationMessages { get; set; }

        public DbSet<LanguageVersion> LanguageVersions { get; set; }

        public DbSet<ContactForm> ContactForms { get; set; }

        public DbSet<BackgroundJob> BackgroundJobs { get; set; }

        public DbSet<InternalValue> InternalValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString, providerOptions => providerOptions.CommandTimeout(300));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdministratorConfiguration());
            modelBuilder.ApplyConfiguration(new AudioFileConfiguration());
            modelBuilder.ApplyConfiguration(new TranscribeItemConfiguration());
            modelBuilder.ApplyConfiguration(new FileChunkConfiguration());
            modelBuilder.ApplyConfiguration(new RecognizedAudioSampleConfiguration());
            modelBuilder.ApplyConfiguration(new SpeechResultConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new CurrentUserSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserDeviceConfiguration());
            modelBuilder.ApplyConfiguration(new DeletedAccountConfiguration());
            modelBuilder.ApplyConfiguration(new BillingPurchaseConfiguration());
            modelBuilder.ApplyConfiguration(new PurchaseStateTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new InformationMessageConfiguration());
            modelBuilder.ApplyConfiguration(new LanguageVersionConfiguration());
            modelBuilder.ApplyConfiguration(new ContactFormConfiguration());
            modelBuilder.ApplyConfiguration(new BackgroundJobConfiguration());
            modelBuilder.ApplyConfiguration(new InternalValueConfiguration());
        }
    }
}
