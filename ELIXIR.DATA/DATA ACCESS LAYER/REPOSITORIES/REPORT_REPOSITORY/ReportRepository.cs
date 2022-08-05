using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.REPORT_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORT_REPOSITORY
{
    public class ReportRepository : IReportRepository
    {
        private readonly StoreContext _context;

        public ReportRepository(StoreContext context)
        {
            _context = context;


        }

        public async Task<IReadOnlyList<QCReport>> QcRecevingReport(string DateFrom, string DateTo)
        {
            var items = (from rawmaterials in _context.RawMaterials
                         where rawmaterials.IsActive == true
                         join category in _context.ItemCategories
                         on rawmaterials.ItemCategoryId equals category.Id into leftJ
                         from category in leftJ.DefaultIfEmpty()

                         select new
                         {
                             ItemCode = rawmaterials.ItemCode,
                             ItemCategory = category.ItemCategoryName
                         });

            var report = (from receiving in _context.QC_Receiving
                          where receiving.QC_ReceiveDate >= DateTime.Parse(DateFrom) && receiving.QC_ReceiveDate <= DateTime.Parse(DateTo) && receiving.IsActive == true
                          join posummary in _context.POSummary
                          on receiving.PO_Summary_Id equals posummary.Id into leftJ
                          from posummary in leftJ.DefaultIfEmpty()

                          join category in items
                          on posummary.ItemCode equals category.ItemCode
                          into leftJ2
                          from category in leftJ2.DefaultIfEmpty()


                          select new QCReport
                          {

                              Id = receiving.Id,
                              QcDate = receiving.QC_ReceiveDate.ToString(),
                              PONumber = posummary != null ? posummary.PO_Number : 0,
                              ItemCode = receiving.ItemCode,
                              ItemDescription = posummary != null ? posummary.ItemDescription : null,
                              Uom = posummary != null ? posummary.UOM : null,
                              Category = category != null ? category.ItemCategory : null,
                              Quantity = receiving.Actual_Delivered,
                              ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                              ExpirationDate = receiving.Expiry_Date.ToString(),
                              TotalReject = receiving.TotalReject,
                              SupplierName = posummary != null ? posummary.VendorName : null,
                              Price = posummary != null ? posummary.UnitPrice : 0,
                              QcBy = receiving.QcBy,
                              TruckApproval1 = receiving.Truck_Approval1,
                              TruckApprovalRemarks1 = receiving.Truck_Approval1_Remarks,
                              TruckApproval2 = receiving.Truck_Approval2,
                              TruckApprovalRemarks2 = receiving.Truck_Approval2_Remarks,
                              TruckApproval3 = receiving.Truck_Approval3,
                              TruckApprovalRemarks3 = receiving.Truck_Approval3_Remarks,
                              TruckApproval4 = receiving.Truck_Approval4,
                              TruckApprovalRemarks4 = receiving.Truck_Approval4_Remarks,
                              UnloadingApproval1 = receiving.Unloading_Approval1,
                              UnloadingApprovalRemarks1 = receiving.Unloading_Approval1_Remarks,
                              UnloadingApproval2 = receiving.Unloading_Approval2,
                              UnloadingApprovalRemarks2 = receiving.Unloading_Approval2_Remarks,
                              UnloadingApproval3 = receiving.Unloading_Approval3,
                              UnloadingApprovalRemarks3 = receiving.Unloading_Approval3_Remarks,
                              UnloadingApproval4 = receiving.Unloading_Approval4,
                              UnloadingApprovalRemarks4 = receiving.Unloading_Approval4_Remarks,
                              CheckingApproval1 = receiving.Checking_Approval1,
                              CheckingApprovalRemarks1 = receiving.Checking_Approval1_Remarks,
                              CheckingApproval2 = receiving.Checking_Approval2, 
                              CheckingApprovalRemarks2 = receiving.Checking_Approval2_Remarks,
                              QAApproval = receiving.QA_Approval, 
                              QAApprovalRemarks = receiving.QA_Approval_Remarks

                          });

            return await report.ToListAsync();

        }
        public async Task<IReadOnlyList<WarehouseReport>> WarehouseRecivingReport(string DateFrom, string DateTo)
        {

            var warehouse = _context.WarehouseReceived.Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                                                      .Where(x => x.IsActive == true)
            .Select(x => new WarehouseReport
            { 
                WarehouseId = x.Id,
                PONumber = x.PO_Number,
                ReceiveDate = x.ReceivingDate.ToString(),
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription, 
                Uom = x.Uom, 
                Category = x.LotCategory, 
                Quantity = x.ActualGood,
                ManufacturingDate = x.ManufacturingDate.ToString(),
                ExpirationDate = x.Expiration.ToString(),
                TotalReject = x.TotalReject, 
                SupplierName = x.Supplier,
                ReceivedBy = x.ReceivedBy,
                TransactionType = x.TransactionType

            });

            return await warehouse.ToListAsync();

        }

        public async Task<IReadOnlyList<TransformationReport>> TransformationReport(string DateFrom, string DateTo)
        {

            var transform = (from planning in _context.Transformation_Planning
                             where planning.ProdPlan >= DateTime.Parse(DateFrom) && planning.ProdPlan <= DateTime.Parse(DateTo) && planning.Status == true
                             join preparation in _context.Transformation_Preparation
                             on planning.Id equals preparation.TransformId into leftJ
                             from preparation in leftJ.DefaultIfEmpty()

                             join warehouse in _context.WarehouseReceived
                             on planning.Id equals warehouse.TransformId into left2
                             from warehouse in left2.DefaultIfEmpty()

                             select new TransformationReport
                             {
                                 TransformationId = planning.Id,
                                 PlanningDate = planning.ProdPlan.ToString(),
                                 ItemCode_Formula = planning.ItemCode,
                                 ItemDescription_Formula = planning.ItemDescription,
                                 Version = planning.Version,
                                 Batch = planning.Batch,
                                 Formula_Quantity = planning.Quantity,
                                 ItemCode_Recipe = preparation.ItemCode,
                                 ItemDescription_Recipe = preparation.ItemDescription,
                                 Recipe_Quantity = preparation.WeighingScale,
                                 DateTransformed = warehouse.ManufacturingDate.ToString()

                             });

            return await transform.ToListAsync();

        }

        public async Task<IReadOnlyList<MoveOrderReport>> MoveOrderReport(string DateFrom, string DateTo)
        {
            var orders = (from ordering in _context.Orders              
                         join moveorders in _context.MoveOrders
                         on ordering.Id equals moveorders.OrderNoPKey into leftJ
                         from moveorders in leftJ.DefaultIfEmpty()

                         where moveorders.PreparedDate >= DateTime.Parse(DateFrom) && moveorders.PreparedDate <= DateTime.Parse(DateTo) && moveorders.IsActive == true

                          select new MoveOrderReport
                         {

                             MoveOrderId = moveorders.OrderNo,
                             CustomerCode = ordering.FarmCode,
                             CustomerName = ordering.CustomerName, 
                             ItemCode = ordering.ItemCode, 
                             ItemDescription = ordering.ItemDescription, 
                             Uom = ordering.Uom, 
                             Category = ordering.Category, 
                             Quantity = moveorders.QuantityOrdered, 
                             ExpirationDate = moveorders.ExpirationDate.ToString(),
                             TransactionType = moveorders.DeliveryStatus, 
                             MoveOrderBy = moveorders.PreparedBy,
                             MoveOrderDate = moveorders.PreparedDate.ToString()

                         });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<MiscellaneousReceiptReport>> MReceiptReport(string DateFrom, string DateTo)
        {

            var receipts = (from receiptHeader in _context.MiscellaneousReceipts                    
                            join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId into leftJ
                            from receipt in leftJ.DefaultIfEmpty()

                            where receipt.ReceivingDate >= DateTime.Parse(DateFrom) && receipt.ReceivingDate <= DateTime.Parse(DateTo) && receipt.IsActive == true && receipt.TransactionType == "MiscellaneousReceipt"

                            select new MiscellaneousReceiptReport
                            {

                                ReceiptId = receiptHeader.Id,
                                SupplierCode = receiptHeader.SupplierCode,
                                SupplierName = receiptHeader.Supplier,
                                Details = receiptHeader.Remarks,
                                ItemCode = receipt.ItemCode,
                                ItemDescription = receipt.ItemDescription,
                                Uom = receipt.Uom,
                                Category = receipt.LotCategory,
                                Quantity = receipt.ActualGood,
                                ExpirationDate = receipt.Expiration.ToString(),
                                TransactBy = receiptHeader.PreparedBy,
                                TransactDate = receipt.ReceivingDate.ToString()

                            });

            return await receipts.ToListAsync();


        }

        public async Task<IReadOnlyList<MiscellaneousIssueReport>> MIssueReport(string DateFrom, string DateTo)
        {

            var issues = (from issue in _context.MiscellaneousIssues
                          join issuedetails in _context.MiscellaneousIssueDetails
                          on issue.Id equals issuedetails.IssuePKey into leftJ
                          from issuedetails in leftJ.DefaultIfEmpty()

                          where issuedetails.PreparedDate >= DateTime.Parse(DateFrom) && issuedetails.PreparedDate <= DateTime.Parse(DateTo) && issuedetails.IsActive == true && issuedetails.IsTransact == true

                          select new MiscellaneousIssueReport
                          {

                              IssueId = issue.Id,
                              CustomerCode = issue.CustomerCode,
                              CustomerName = issue.Customer,
                              Details = issue.Remarks,
                              ItemCode = issuedetails != null ? issuedetails.ItemCode : null,
                              ItemDescription = issuedetails != null ? issuedetails.ItemDescription : null,
                              Uom = issuedetails != null ? issuedetails.Uom : null,
                              Quantity = issuedetails != null ? issuedetails.Quantity : 0,
                              ExpirationDate = issuedetails != null ? issuedetails.ExpirationDate.ToString() : null,
                              TransactBy = issue.PreparedBy,
                              TransactDate = issuedetails != null ? issuedetails.PreparedDate.ToString() : null

                          });

            return await issues.ToListAsync();
        }

        public async Task<IReadOnlyList<WarehouseReport>> NearlyExpireItemsReport(int expirydays)
        {
            var preparationOut = _context.Transformation_Preparation.Where(x => x.IsActive == true)                                                             
                .GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var moveorderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                  .Where(x => x.IsPrepared == true)
                .GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.QuantityOrdered),
                WarehouseId = x.Key.WarehouseId
            });

            var issueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsTransact == true)
                .GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.Quantity),
                WarehouseId = x.Key.WarehouseId
            });

            var warehouseInventory = (from warehouse in _context.WarehouseReceived
                                  where warehouse.ExpirationDays <= expirydays
                                  join preparation in preparationOut
                                  on warehouse.Id equals preparation.WarehouseId
                                  into leftJ
                                  from preparation in leftJ.DefaultIfEmpty()

                                  join moveorder in moveorderOut
                                  on warehouse.Id equals moveorder.WarehouseId
                                  into leftJ2
                                  from moveorder in leftJ2.DefaultIfEmpty()

                                  join issue in issueOut
                                  on warehouse.Id equals issue.WarehouseId
                                  into leftJ3
                                  from issue in leftJ3.DefaultIfEmpty()

                                      group new
                                  {
                                      warehouse,
                                      preparation,
                                      moveorder,
                                      issue
                                  }
                                  by new 
                                  {
                                      warehouse.Id,
                                      warehouse.PO_Number,
                                      warehouse.ItemCode,
                                      warehouse.ItemDescription,
                                      warehouse.ManufacturingDate,
                                      warehouse.ReceivingDate,
                                      warehouse.LotCategory,
                                      warehouse.Uom,
                                      warehouse.ActualGood,
                                      warehouse.Expiration,
                                      warehouse.ExpirationDays,
                                      warehouse.Supplier,
                                      warehouse.ReceivedBy,
                                      PreparationOut = preparation.Out != null ? preparation.Out : 0,
                                      MoveOrderOut = moveorder.Out != null ? moveorder.Out : 0,
                                      IssueOut = issue.Out != null ? issue.Out : 0


                                  } into total

                                  orderby total.Key.ItemCode, total.Key.ExpirationDays ascending

                                  select new WarehouseReport
                                  {
                                      WarehouseId = total.Key.Id,
                                      PONumber = total.Key.PO_Number,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      Uom = total.Key.Uom,
                                      Category = total.Key.LotCategory,
                                      ReceiveDate = total.Key.ReceivingDate.ToString(),
                                      ManufacturingDate = total.Key.ManufacturingDate.ToString(),
                                      Quantity = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut - total.Key.IssueOut,
                                      ExpirationDate = total.Key.Expiration.ToString(),
                                      ExpirationDays = total.Key.ExpirationDays,
                                      SupplierName = total.Key.Supplier,
                                      ReceivedBy = total.Key.ReceivedBy

                                  });

            return await warehouseInventory.ToListAsync();
                                 
        }
    }
}
