using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.LABTEST_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;

public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions<StoreContext> options) : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<UserRole> Roles { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Module> Modules { get; set; }
    public virtual DbSet<UserRole_Modules> RoleModules { get; set; }
    public virtual DbSet<MainMenu> MainMenus { get; set; }
    public virtual DbSet<UOM> UOMS { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public virtual DbSet<LotName> LotNames { get; set; }
    public virtual DbSet<LotCategory> LotCategories { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<FarmType> Farms { get; set; }
    public virtual DbSet<RawMaterial> RawMaterials { get; set; }
    public virtual DbSet<ItemCategory> ItemCategories { get; set; }
    public virtual DbSet<Reason> Reasons { get; set; }
    public virtual DbSet<TransformationFormula> Formulas { get; set; }
    public virtual DbSet<TransformationRequirement> FormulaRequirements { get; set; }
    public virtual DbSet<ImportPOSummary> POSummary { get; set; }
    public virtual DbSet<PO_Receiving> QC_Receiving { get; set; }
    public virtual DbSet<PO_Reject> QC_Reject { get; set; }
    public virtual DbSet<WarehouseReceiving> WarehouseReceived { get; set; }
    public virtual DbSet<Warehouse_Reject> Warehouse_Reject { get; set; }
    public virtual DbSet<TransformationPlanning> Transformation_Planning { get; set; }
    public virtual DbSet<TransformationRequest> Transformation_Request { get; set; }
    public virtual DbSet<TransformationReject> Transformation_Reject { get; set; }
    public virtual DbSet<TransformationPreparation> Transformation_Preparation { get; set; }
    public virtual DbSet<Ordering> Orders { get; set; }
    public virtual DbSet<MoveOrder> MoveOrders { get; set; }
    public virtual DbSet<GenerateOrderNo> GenerateOrderNos { get; set; }
    public virtual DbSet<TransactMoveOrder> TransactMoveOrder { get; set; }
    public virtual DbSet<MiscellaneousReceipt> MiscellaneousReceipts { get; set; }
    public virtual DbSet<MiscellaneousIssue> MiscellaneousIssues { get; set; }
    public virtual DbSet<MiscellaneousIssueDetails> MiscellaneousIssueDetails { get; set; }
    public virtual DbSet<Transaction> Transactions { get; set; }
    public virtual DbSet<ChecklistForCompliants> ChecklistForCompliant { get; set; }
    public virtual DbSet<CheckListInputs> CheckListInput { get; set; }
    public virtual DbSet<CheckListString> CheckListStrings { get; set; }
    public virtual DbSet<Analysis> Analyses { get; set; }
    public virtual DbSet<Parameters> Parameters { get; set; }
    public virtual DbSet<SampleType> SampleTypes { get; set; }
    public virtual DbSet<TypeOfSwab> TypeOfSwabs { get; set; }
    public virtual DbSet<ProductCondition> ProductConditions { get; set; }
    public virtual DbSet<Disposition> Dispositions { get; set; }
    public virtual DbSet<ProductType> ProductTypes { get; set; }
    public virtual DbSet<CancelledOrders> CancelledOrders { get; set; }
    public virtual DbSet<AccountTitle> AccountTitles { get; set; }
    public virtual DbSet<ChecklistTypes> ChecklistTypes { get; set; }
    public virtual DbSet<LabTestRequests> LabTestRequests { get; set; }
    public virtual DbSet<ReceiveRequest> ReceiveRequests { get; set; }
    public virtual DbSet<ReturnedItems> ReturnedItems { get; set; }
    public virtual DbSet<RejectedItems> RejectedItems { get; set; }
    public virtual DbSet<LabTestResult> LabTestResults { get; set; }
    public virtual DbSet<ChecklistQuestions> ChecklistQuestions { get; set; }
    public virtual DbSet<QCChecklist> QcChecklists { get; set; }
    public virtual DbSet<ChecklistOpenFieldAnswer> QChecklistOpenFieldAnswers { get; set; }
    public virtual DbSet<ChecklistProductDimension> QChecklistProductDimensions { get; set; }
    public virtual DbSet<ChecklistOtherObservation> ChecklistOtherObservations { get; set; }
    public virtual DbSet<ChecklistCompliance> ChecklistCompliances { get; set; }
    public virtual DbSet<ChecklistReviewVerificationLog> ChecklistReviewVerificationLogs { get; set; }
    public virtual DbSet<AdvancesToEmployees> AdvancesToEmployees { get; set; }
    public virtual DbSet<ChecklistAnswers> ChecklistAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReceiveRequest>()
            .HasOne(l => l.LabTestRequests)
            .WithMany()
            .HasForeignKey(l => l.LabTestRequestsId);

        modelBuilder.Entity<LabTestRequests>()
            .HasOne(l => l.WarehouseReceived)
            .WithMany()
            .HasForeignKey(l => l.WarehouseReceivingId);

        //AcceptedLabRequest

        modelBuilder.Entity<LabTestRequests>()
            .Property(e => e.Analysis)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        modelBuilder.Entity<LabTestRequests>()
            .Property(e => e.Parameters)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        modelBuilder.Entity<LabTestRequests>()
            .Property(e => e.ProductCondition)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v,  (JsonSerializerOptions)null),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        modelBuilder.Entity<LabTestRequests>()
            .Property(e => e.SampleType)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        modelBuilder.Entity<LabTestRequests>()
            .Property(e => e.TypeOfSwab)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        modelBuilder.Entity<CheckListString>()
            .Property(e => e.Value)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<List<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                ));

        modelBuilder.Entity<ChecklistQuestions>()
            .HasOne(x => x.AddedByUser)
            .WithMany()
            .HasForeignKey(x => x.AddedBy);

        ///New Checklist Entities

        modelBuilder.Entity<QCChecklist>()
            .HasOne(x => x.PoReceiving)
            .WithMany()
            .HasForeignKey(x => x.ReceivingId);

        modelBuilder.Entity<ChecklistTypes>()
            .HasOne(x => x.AddedByUser)
            .WithMany()
            .HasForeignKey(x => x.AddedBy);

        modelBuilder.Entity<ChecklistTypes>()
            .HasOne(x => x.ModifiedByUser)
            .WithMany()
            .HasForeignKey(x => x.ModifiedBy);

        modelBuilder.Entity<ChecklistQuestions>()
            .HasOne(x => x.ModifiedByUser)
            .WithMany()
            .HasForeignKey(x => x.ModifiedBy);
    }
}