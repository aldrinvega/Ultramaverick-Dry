using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.REPORT_DTOs;
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
                              PONumber = posummary.PO_Number,
                              ItemCode = posummary.ItemCode, 
                              ItemDescription = posummary.ItemDescription, 
                              Uom = posummary.UOM,
                              Category = category.ItemCategory,
                              Quantity = receiving.Actual_Delivered,
                              ManufacturingDate = receiving.Manufacturing_Date.ToString(),
                              ExpirationDate = receiving.Expiry_Date.ToString(),
                              TotalReject = receiving.TotalReject, 
                              SupplierName = posummary.VendorName,
                              Price = posummary.UnitPrice, 
                              QcBy = receiving.QcBy

                          });

            return await report.ToListAsync();

        }
        public async Task<IReadOnlyList<WarehouseReport>> WarehouseRecivingReport(string DateFrom, string DateTo)
        {

            var warehouse = _context.WarehouseReceived.Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                                                      .Where(x => x.IsActive == true)
            .Select(x => new WarehouseReport
            {
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
                ReceivedBy = x.ReceivedBy

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
                            where receiptHeader.PreparedDate >= DateTime.Parse(DateFrom) && receiptHeader.PreparedDate <= DateTime.Parse(DateTo) && receiptHeader.IsActive == true
                            join receipt in _context.WarehouseReceived
                            on receiptHeader.Id equals receipt.MiscellaneousReceiptId into leftJ
                            from receipt in leftJ.DefaultIfEmpty()

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
                                TransactDate = receiptHeader.PreparedDate.ToString()

                            });

            return await receipts.ToListAsync();


        }

        public async Task<IReadOnlyList<MiscellaneousIssueReport>> MIssueReport(string DateFrom, string DateTo)
        {

            var issues =    (from issue in _context.MiscellaneousIssues
                            where issue.PreparedDate >= DateTime.Parse(DateFrom) && issue.PreparedDate <= DateTime.Parse(DateTo) && issue.IsActive == true
                            join issuedetails in _context.WarehouseReceived
                            on issue.Id equals issuedetails.MiscellaneousReceiptId into leftJ
                            from issuedetails in leftJ.DefaultIfEmpty()

                            select new MiscellaneousIssueReport
                            {

                                IssueId = issue.Id,
                                CustomerCode = issue.CustomerCode,
                                CustomerName = issue.Customer,
                                Details = issue.Remarks,
                                ItemCode = issuedetails.ItemCode,
                                ItemDescription = issuedetails.ItemDescription,
                                Uom = issuedetails.Uom,
                                Category = issuedetails.LotCategory,
                                Quantity = issuedetails.ActualGood,
                                ExpirationDate = issuedetails.Expiration.ToString(),
                                TransactBy = issue.PreparedBy,
                                TransactDate = issue.PreparedDate.ToString()

                            });

            return await issues.ToListAsync();
        }
    }
}
