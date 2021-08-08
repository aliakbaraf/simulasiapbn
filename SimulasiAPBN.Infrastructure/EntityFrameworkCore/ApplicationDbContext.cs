/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimulasiAPBN.Core.Models;

namespace SimulasiAPBN.Infrastructure.EntityFrameworkCore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) {}
        
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Allocation> Allocations { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetTarget> BudgetTargets { get; set; }
        public DbSet<EconomicMacro> EconomicMacros { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<SignInAttempt> SignInAttempts { get; set; }
        public DbSet<SignInSession> SignInSessions { get; set; }
        public DbSet<SimulationConfig> SimulationConfigs { get; set; }
        public DbSet<SimulationSession> SimulationSessions { get; set; }
        public DbSet<SimulationShare> SimulationShares { get; set; }
        public DbSet<SimulationSpecialPolicyAllocation> SimulationSpecialPolicyAllocations { get; set; }
        public DbSet<SimulationStateExpenditure> SimulationStateExpenditures { get; set; }
        public DbSet<SimulationEconomicMacro> SimulationEconomicMacros { get; set; }
        public DbSet<SpecialPolicy> SpecialPolicies { get; set; }
        public DbSet<SpecialPolicyAllocation> SpecialPolicyAllocations { get; set; }
        public DbSet<StateBudget> StateBudgets { get; set; }
        public DbSet<StateExpenditure> StateExpenditures { get; set; }
        public DbSet<StateExpenditureAllocation> StateExpenditureAllocations { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<WebContent> WebContents { get; set; }
        
        private void MarkEntities()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["CreatedAt"] = DateTimeOffset.Now;
                        entry.CurrentValues["UpdatedAt"] = null;
                        entry.CurrentValues["DeletedAt"] = null;
                        break;
                    case EntityState.Modified:
                        entry.CurrentValues["UpdatedAt"] = DateTimeOffset.Now;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["DeletedAt"] = DateTimeOffset.Now;
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Account>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<Allocation>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<Budget>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<BudgetTarget>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<MediaFile>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SignInAttempt>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SignInSession>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SimulationConfig>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SimulationSession>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SimulationShare>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SimulationSpecialPolicyAllocation>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SimulationStateExpenditure>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SimulationEconomicMacro>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<EconomicMacro>()
                .HasQueryFilter(entity => entity.DeletedAt == null)
                .HasMany(entity => entity.SimulationEconomicMacros)
                .WithOne(entity => entity.EconomicMacro)
                .HasForeignKey(entity => entity.EconomicMacrosId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SpecialPolicy>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<SpecialPolicyAllocation>()
                .HasQueryFilter(entity => entity.DeletedAt == null)
                .HasMany(entity => entity.SimulationSpecialPolicyAllocations)
                .WithOne(entity => entity.SpecialPolicyAllocation)
                .HasForeignKey(entity => entity.SpecialPolicyAllocationId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<StateBudget>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<StateExpenditure>()
                .HasQueryFilter(entity => entity.DeletedAt == null)
                .HasMany(entity => entity.SimulationStateExpenditures)
                .WithOne(entity => entity.StateExpenditure)
                .HasForeignKey(entity => entity.StateExpenditureId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<StateExpenditureAllocation>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<Token>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
            modelBuilder.Entity<WebContent>()
                .HasQueryFilter(entity => entity.DeletedAt == null);
        }
        
        public override int SaveChanges()
        {
            MarkEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            MarkEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
