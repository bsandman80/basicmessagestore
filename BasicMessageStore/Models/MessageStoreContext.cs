using System;
using System.Threading;
using System.Threading.Tasks;
using BasicMessageStore.Models.Messages;
using BasicMessageStore.Models.Users;
using BasicMessageStore.Security;
using Microsoft.EntityFrameworkCore;

namespace BasicMessageStore.Models
{
    public class MessageStoreContext : DbContext
    {
        private readonly IClientProvider _clientProvider;

        public MessageStoreContext(DbContextOptions<MessageStoreContext> options, IClientProvider clientProvider)
            : base(options)
        {
            _clientProvider = clientProvider;

        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        public override int SaveChanges()
        {
            Audit();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            Audit();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Message>()
                .HasKey(c => c.Id);
        }

        /// <summary>
        /// Fill in all auditable fields for Models with interface Auditable
        /// </summary>
        private void Audit()
        {
            foreach (var auditableModel in ChangeTracker.Entries<IAuditable>())
            {
                if (auditableModel.State == EntityState.Added)
                {
                    auditableModel.Reference(nameof(IAuditable.CreatedBy)).CurrentValue = _clientProvider.CurrentUser;
                    auditableModel.Property(nameof(IAuditable.Created)).CurrentValue = DateTime.Now;    
                }
                else if (auditableModel.State == EntityState.Modified)
                {
                    auditableModel.Property(nameof(IAuditable.Updated)).CurrentValue = DateTime.Now;
                }
            }
        }
        
    }
}