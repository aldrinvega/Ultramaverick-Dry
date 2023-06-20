using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using ELIXIR.DATA.DTOs.REPORT_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
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
                where receiving.QC_ReceiveDate >= DateTime.Parse(DateFrom) &&
                      receiving.QC_ReceiveDate <= DateTime.Parse(DateTo) && receiving.IsActive == true
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
                });

            return await report.ToListAsync();

        }

        public async Task<IReadOnlyList<WarehouseReport>> WarehouseReceivingReport(string DateFrom, string DateTo)
        {

            var warehouse = _context.WarehouseReceived.Where(x =>
                    x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
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
                where planning.ProdPlan >= DateTime.Parse(DateFrom) && planning.ProdPlan <= DateTime.Parse(DateTo) &&
                      planning.Status == true
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
                    ItemCode_Recipe = preparation.ItemCode != null ? preparation.ItemCode : null,
                    ItemDescription_Recipe = preparation.ItemDescription != null ? preparation.ItemDescription : null,
                    Recipe_Quantity = preparation.WeighingScale != null ? preparation.WeighingScale : 0,
                    DateTransformed = warehouse.ManufacturingDate.ToString()

                });

            return await transform.ToListAsync();

        }

        public async Task<IReadOnlyList<MoveOrderReport>> MoveOrderReport(string DateFrom, string DateTo)
        {
            var soh = _context.WarehouseReceived
                .GroupBy(x => new { x.ItemCode })
                .Select(x => new SOHDTO
                {
                    ItemCode = x.Key.ItemCode,
                    ActualGood = x.Sum(g => g.ActualGood)
                });

            var unitCost = _context.POSummary
                .GroupBy(x => new { x.ItemCode, x.UnitPrice })
                .Select(x => new TOTALCOSTDTO
                {
                    ItemCode = x.Key.ItemCode,
                    TotalCost = x.Sum(g => g.UnitPrice)
                });

            var orders = (
                from moveorder in _context.MoveOrders
                where moveorder.PreparedDate >= DateTime.Parse(DateFrom) &&
                      moveorder.PreparedDate <= DateTime.Parse(DateTo) &&
                      moveorder.IsActive == true
                join transactmoveorder in _context.TransactMoveOrder
                    on moveorder.OrderNo equals transactmoveorder.OrderNo into leftJ
                from transactmoveorder in leftJ.DefaultIfEmpty()
                join stockOnHand in soh
                    on moveorder.ItemCode equals stockOnHand.ItemCode into leftj1
                from stockOnHand in leftj1.DefaultIfEmpty()
                join UC in unitCost
                    on moveorder.ItemCode equals UC.ItemCode into leftj2
                from UC in leftj2.DefaultIfEmpty()
                group new
                {
                    moveorder,
                    stockOnHand,
                    UC,
                    transactmoveorder
                } by new
                {
                    moveorder.OrderNo,
                    moveorder.FarmCode,
                    moveorder.FarmName,
                    moveorder.ItemCode,
                    moveorder.ItemDescription,
                    moveorder.Uom,
                    moveorder.Category,
                    MoveOrderPreparedBy = moveorder.PreparedBy,
                    moveorder.QuantityOrdered,
                    moveorder.ExpirationDate,
                    moveorder.DeliveryStatus,
                    TransactMoveorderPreparedDate = moveorder.PreparedDate,
                    transactmoveorder.PreparedBy,
                    transactmoveorder.PreparedDate,
                    stockOnHand.ActualGood,
                    TotalCost = UC.TotalCost * stockOnHand.ActualGood,
                    WeightedAverageUnitCost = ((UC.TotalCost * stockOnHand.ActualGood) / stockOnHand.ActualGood == null
                        ? 1
                        : (UC.TotalCost * stockOnHand.ActualGood) / stockOnHand.ActualGood)
                }
                into result
                select new MoveOrderReport
                {
                    MoveOrderId = result.Key.OrderNo,
                    CustomerCode = result.Key.FarmCode,
                    CustomerName = result.Key.FarmName,
                    ItemCode = result.Key.ItemCode,
                    ItemDescription = result.Key.ItemDescription,
                    Uom = result.Key.Uom,
                    Category = result.Key.Category,
                    Quantity = result.Key.QuantityOrdered,
                    ExpirationDate = result.Key.ExpirationDate.ToString(),
                    TransactionType = result.Key.DeliveryStatus,
                    MoveOrderBy = result.Key.PreparedBy,
                    MoveOrderDate = result.Key.PreparedDate.ToString(),
                    TransactedBy = result.Key.PreparedBy,
                    TransactedDate = result.Key.PreparedDate.ToString(),
                    WeightedAverageUnitCost = result.Key.WeightedAverageUnitCost
                });

            return await orders.ToListAsync();
        }


        public async Task<IReadOnlyList<MiscellaneousReceiptReport>> MReceiptReport(string DateFrom, string DateTo)
        {

            var receipts = (from receiptHeader in _context.MiscellaneousReceipts
                join receipt in _context.WarehouseReceived
                    on receiptHeader.Id equals receipt.MiscellaneousReceiptId into leftJ
                from receipt in leftJ.DefaultIfEmpty()

                where receipt.ReceivingDate >= DateTime.Parse(DateFrom) &&
                      receipt.ReceivingDate <= DateTime.Parse(DateTo) && receipt.IsActive == true &&
                      receipt.TransactionType == "MiscellaneousReceipt"

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

                where issuedetails.PreparedDate >= DateTime.Parse(DateFrom) &&
                      issuedetails.PreparedDate <= DateTime.Parse(DateTo) && issuedetails.IsActive == true &&
                      issuedetails.IsTransact == true

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


                    }
                into total

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
                    Quantity = total.Key.ActualGood - total.Key.PreparationOut - total.Key.MoveOrderOut -
                               total.Key.IssueOut,
                    ExpirationDate = total.Key.Expiration.ToString(),
                    ExpirationDays = total.Key.ExpirationDays,
                    SupplierName = total.Key.Supplier,
                    ReceivedBy = total.Key.ReceivedBy

                });

            return await warehouseInventory.Where(x => x.Quantity != 0)
                .ToListAsync();

        }

        public async Task<IReadOnlyList<MoveOrderReport>> TransactedMoveOrderReport(string DateFrom, string DateTo)
        {
            var orders = (from transact in _context.TransactMoveOrder
                where transact.IsActive == true && transact.IsTransact == true &&
                      transact.PreparedDate >= DateTime.Parse(DateFrom) &&
                      transact.PreparedDate <= DateTime.Parse(DateTo)
                join moveorder in _context.MoveOrders
                    on transact.OrderNo equals moveorder.OrderNo into leftJ
                from moveorder in leftJ.DefaultIfEmpty()
                where moveorder.IsActive == true

                join customer in _context.Customers
                    on moveorder.FarmCode equals customer.CustomerCode
                    into leftJ2
                from customer in leftJ2.DefaultIfEmpty()

                group new
                    {
                        transact,
                        moveorder,
                        customer
                    }

                    by new
                    {
                        moveorder.OrderNo,
                        customer.CustomerName,
                        customer.CustomerCode,
                        moveorder.ItemCode,
                        moveorder.ItemDescription,
                        moveorder.Uom,
                        moveorder.QuantityOrdered,
                        MoveOrderDate = moveorder.ApprovedDate.ToString(),
                        transact.PreparedBy,
                        moveorder.DeliveryStatus,
                        TransactedDate = transact.PreparedDate.ToString(),
                        DeliveryDate = transact.DeliveryDate.ToString()

                    }
                into total

                select new MoveOrderReport
                {

                    OrderNo = total.Key.OrderNo,
                    CustomerName = total.Key.CustomerName,
                    CustomerCode = total.Key.CustomerCode,
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    Uom = total.Key.Uom,
                    Quantity = total.Key.QuantityOrdered,
                    MoveOrderDate = total.Key.MoveOrderDate,
                    TransactedBy = total.Key.PreparedBy,
                    TransactionType = total.Key.DeliveryStatus,
                    TransactedDate = total.Key.TransactedDate,
                    DeliveryDate = total.Key.DeliveryDate

                });
            //select new MoveOrderReport
            //{
            //    OrderNo = moveorder.OrderNo,
            //    CustomerName = moveorder.FarmName,
            //    CustomerCode = moveorder.FarmCode,
            //    FarmCode = moveorder.FarmCode,
            //    FarmName = moveorder.FarmName,
            //    FarmType = moveorder.FarmType,
            //    ItemCode = moveorder.ItemCode, 
            //    ItemDescription = moveorder.ItemDescription, 
            //    Uom = moveorder.Uom,
            //    Quantity = moveorder.QuantityOrdered, 
            //    MoveOrderDate = moveorder.ApprovedDate.ToString(),
            //    TransactedBy = transact.PreparedBy,
            //    TransactionType = moveorder.DeliveryStatus,
            //    TransactedDate = transact.PreparedDate.ToString(),          
            //    BatchNo = moveorder.BatchNo,
            //    DeliveryDate = transact.DeliveryDate.ToString()                         
            //});

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<CancelledOrderReport>> CancelledOrderedReports(string DateFrom, string DateTo)
        {

            var orders = (from ordering in _context.Orders
                where ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo) &&
                      ordering.IsCancel == true && ordering.IsActive == false

                select new CancelledOrderReport
                {

                    OrderId = ordering.Id,
                    DateNeeded = ordering.DateNeeded.ToString(),
                    DateOrdered = ordering.OrderDate.ToString(),
                    CustomerCode = ordering.FarmCode,
                    CustomerName = ordering.FarmName,
                    ItemCode = ordering.ItemCode,
                    ItemDescription = ordering.ItemDescription,
                    QuantityOrdered = ordering.QuantityOrdered,
                    CancelledDate = ordering.CancelDate.ToString(),
                    CancelledBy = ordering.IsCancelBy,
                    Reason = ordering.Remarks
                });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<InventoryMovementReport>> InventoryMovementReport(string DateFrom,
            string DateTo, string PlusOne)
        {
            var dateToday = DateTime.Now.ToString("MM/dd/yyyy");


            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new WarehouseInventory
                {
                    ItemCode = x.Key.ItemCode,
                    ActualGood = x.Sum(x => x.ActualGood)
                });

            var getMoveOrderOutByDate = _context.MoveOrders.Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo) &&
                            x.ApprovedDate != null)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)
                });

            var getMoveOrderOutByDatePlus = _context.MoveOrders.Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= DateTime.Parse(dateToday) &&
                            x.ApprovedDate != null)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)
                });
            var getTransformationByDate = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getTransformationByDatePlus = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= DateTime.Parse(dateToday))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getIssueOutByDate = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .Where(x => x.IsTransact == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(DateFrom) && x.PreparedDate <= DateTime.Parse(DateTo))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });


            var getIssueOutByDatePlus = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .Where(x => x.IsTransact == true)
                .Where(x => x.PreparedDate >= DateTime.Parse(PlusOne) && x.PreparedDate <= DateTime.Parse(dateToday))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });


            var getReceivetIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Receiving")
                .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });

            var getReceivetInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Receiving")
                .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });



            var getTransformIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Transformation")
                .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });

            var getTransformInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Transformation")
                .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });


            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "MiscellaneousReceipt")
                .Where(x => x.ReceivingDate >= DateTime.Parse(DateFrom) && x.ReceivingDate <= DateTime.Parse(DateTo))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });

            var getReceiptInPlus = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "MiscellaneousReceipt")
                .Where(x => x.ReceivingDate >= DateTime.Parse(PlusOne) && x.ReceivingDate <= DateTime.Parse(dateToday))
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                });


            var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .Where(x => x.IsTransact == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });

            var getSOH = (from warehouse in getWarehouseStock
                join preparation in getTransformation
                    on warehouse.ItemCode equals preparation.ItemCode
                    into leftJ1
                from preparation in leftJ1.DefaultIfEmpty()

                join issue in getIssueOut
                    on warehouse.ItemCode equals issue.ItemCode
                    into leftJ2
                from issue in leftJ2.DefaultIfEmpty()

                join moveorder in getMoveOrderOut
                    on warehouse.ItemCode equals moveorder.ItemCode
                    into leftJ3
                from moveorder in leftJ3.DefaultIfEmpty()

                join receipt in getReceiptIn
                    on warehouse.ItemCode equals receipt.ItemCode
                    into leftJ4
                from receipt in leftJ4.DefaultIfEmpty()


                group new
                    {
                        warehouse,
                        preparation,
                        moveorder,
                        receipt,
                        issue
                    }

                    by new
                    {
                        warehouse.ItemCode

                    }
                into total

                select new SOHInventory
                {
                    ItemCode = total.Key.ItemCode,
                    SOH = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                          total.Sum(x => x.preparation.WeighingScale == null ? 0 : x.preparation.WeighingScale) -
                          total.Sum(x => x.moveorder.QuantityOrdered == null ? 0 : x.moveorder.QuantityOrdered)

                });

            var movementInventory = (from rawmaterial in _context.RawMaterials
                join moveorder in getMoveOrderOutByDate
                    on rawmaterial.ItemCode equals moveorder.ItemCode
                    into leftJ
                from moveorder in leftJ.DefaultIfEmpty()

                join transformation in getTransformationByDate
                    on rawmaterial.ItemCode equals transformation.ItemCode
                    into leftJ2
                from transformation in leftJ2.DefaultIfEmpty()

                join issue in getIssueOutByDate
                    on rawmaterial.ItemCode equals issue.ItemCode
                    into leftJ3
                from issue in leftJ3.DefaultIfEmpty()

                join receive in getReceivetIn
                    on rawmaterial.ItemCode equals receive.ItemCode
                    into leftJ4
                from receive in leftJ4.DefaultIfEmpty()

                join transformIn in getTransformIn
                    on rawmaterial.ItemCode equals transformIn.ItemCode
                    into leftJ5
                from transformIn in leftJ5.DefaultIfEmpty()

                join receipt in getReceiptIn
                    on rawmaterial.ItemCode equals receipt.ItemCode
                    into leftJ6
                from receipt in leftJ6.DefaultIfEmpty()

                join SOH in getSOH
                    on rawmaterial.ItemCode equals SOH.ItemCode
                    into leftJ7
                from SOH in leftJ7.DefaultIfEmpty()

                join receivePlus in getReceivetInPlus
                    on rawmaterial.ItemCode equals receivePlus.ItemCode
                    into leftJ8
                from receivePlus in leftJ8.DefaultIfEmpty()

                join transformPlus in getTransformInPlus
                    on rawmaterial.ItemCode equals transformPlus.ItemCode
                    into leftJ9
                from transformPlus in leftJ9.DefaultIfEmpty()

                join receiptPlus in getReceiptInPlus
                    on rawmaterial.ItemCode equals receiptPlus.ItemCode
                    into leftJ10
                from receiptPlus in leftJ10.DefaultIfEmpty()

                join moveorderPlus in getMoveOrderOutByDatePlus
                    on rawmaterial.ItemCode equals moveorderPlus.ItemCode
                    into leftJ11
                from moveorderPlus in leftJ11.DefaultIfEmpty()

                join transformoutPlus in getTransformationByDatePlus
                    on rawmaterial.ItemCode equals transformoutPlus.ItemCode
                    into leftJ12
                from transformoutPlus in leftJ12.DefaultIfEmpty()

                join issuePlus in getIssueOutByDatePlus
                    on rawmaterial.ItemCode equals issuePlus.ItemCode
                    into leftJ13
                from issuePlus in leftJ13.DefaultIfEmpty()

                group new
                    {
                        rawmaterial,
                        moveorder,
                        transformation,
                        issue,
                        receive,
                        transformIn,
                        receipt,
                        SOH,
                        receivePlus,
                        transformPlus,
                        receiptPlus,
                        moveorderPlus,
                        transformoutPlus,
                        issuePlus
                    }
                    by new
                    {
                        rawmaterial.ItemCode,
                        rawmaterial.ItemDescription,
                        rawmaterial.ItemCategory.ItemCategoryName,
                        MoveOrder = moveorder.QuantityOrdered != null ? moveorder.QuantityOrdered : 0,
                        Transformation = transformation.WeighingScale != null ? transformation.WeighingScale : 0,
                        Issue = issue.Quantity != null ? issue.Quantity : 0,
                        ReceiptIn = receipt.Quantity != null ? receipt.Quantity : 0,
                        ReceiveIn = receive.Quantity != null ? receive.Quantity : 0,
                        TransformIn = transformIn.Quantity != null ? transformIn.Quantity : 0,
                        SOH = SOH.SOH != null ? SOH.SOH : 0,
                        ReceivePlus = receivePlus.Quantity != null ? receivePlus.Quantity : 0,
                        TransformPlus = transformPlus.Quantity != null ? transformPlus.Quantity : 0,
                        ReceiptPlus = receiptPlus.Quantity != null ? receiptPlus.Quantity : 0,
                        MoveOrderPlus = moveorderPlus.QuantityOrdered != null ? moveorderPlus.QuantityOrdered : 0,
                        TransformOutPlus = transformoutPlus.WeighingScale != null ? transformoutPlus.WeighingScale : 0,
                        IssuePlus = issuePlus.Quantity != null ? issuePlus.Quantity : 0,



                    }
                into total

                select new InventoryMovementReport
                {
                    ItemCode = total.Key.ItemCode,
                    ItemDescription = total.Key.ItemDescription,
                    ItemCategory = total.Key.ItemCategoryName,
                    TotalOut = total.Key.MoveOrder + total.Key.Transformation + total.Key.Issue,
                    TotalIn = total.Key.ReceiveIn + total.Key.ReceiptIn + total.Key.TransformIn,
                    Ending = (total.Key.ReceiveIn + total.Key.ReceiptIn + total.Key.TransformIn) -
                             (total.Key.MoveOrder + total.Key.Transformation + total.Key.Issue),
                    CurrentStock = total.Key.SOH,
                    PurchasedOrder = total.Key.ReceivePlus + total.Key.TransformPlus + total.Key.ReceiptPlus,
                    OthersPlus = total.Key.MoveOrderPlus + total.Key.TransformOutPlus + total.Key.IssuePlus
                });

            return await movementInventory.ToListAsync();

        }

        public async Task<IReadOnlyList<ConsolidatedReport>> ConsolidatedReport(string dateFrom, string dateTo)
        {
            var consolidatedReports = new List<ConsolidatedReport>();
    var rawMaterials = await _context.RawMaterials
        .Select(rawMaterial => new
        {
            rawMaterial.ItemCode,
            rawMaterial.ItemDescription,
            rawMaterial.UOM
        })
        .ToListAsync();

    var warehouseReceived = await _context.WarehouseReceived
        .Select(warehouseReceived => new
        {
            warehouseReceived.PO_Number,
            warehouseReceived.Id,
            warehouseReceived.ItemCode,
            warehouseReceived.MiscellaneousReceiptId
        })
        .ToListAsync();

    var receivingReports = await _context.QC_Receiving
        .Where(wr => wr.IsWareHouseReceive == true)
        .Select(receiving => new
        {
            receiving.Id,
            receiving.QC_ReceiveDate,
            receiving.ItemCode,
            receiving.Actual_Delivered,
            TransactionType = "Receiving"
        })
        .ToListAsync();

    var moveOrderReports = await _context.MoveOrders
        .Where(moveOrder => moveOrder.IsTransact == true)
        .Select(moveOrder => new
        {
            moveOrder.Id,
            moveOrder.ItemCode,
            moveOrder.ItemDescription,
            moveOrder.OrderNo,
            moveOrder.FarmName,
            moveOrder.FarmCode,
            moveOrder.Uom,
            moveOrder.CompanyCode,
            moveOrder.CompanyName,
            moveOrder.DepartmentName,
            moveOrder.AccountTitles,
            moveOrder.LocationName,
            moveOrder.QuantityOrdered,
            moveOrder.WarehouseId,
            moveOrder.PreparedDate
        }).ToListAsync();

    var receiptInReport = await _context.MiscellaneousReceipts
        .Where(receipt => receipt.IsActive == true)
        .Select(receiptIn => new
        {
            receiptIn.Id,
            receiptIn.CompanyName,
            receiptIn.CompanyCode,
            receiptIn.DepartmentName,
            receiptIn.AccountTitles,
            receiptIn.LocationName,
            receiptIn.TotalQuantity,
            receiptIn.PreparedDate
        })
        .ToListAsync();

    consolidatedReports = _context.QC_Receiving
        .Where(wr => wr.IsWareHouseReceive == true)
        .Join(
            _context.RawMaterials,
            receiving => receiving.ItemCode,
            rawMaterial => rawMaterial.ItemCode,
            (receiving, rawMaterialsGroup) => new { Receiving = receiving, RawMaterialsGroup = rawMaterialsGroup })
        .AsEnumerable()
        .Select(
            (joinResult, rawMaterial) => new ConsolidatedReport
            {
                Id = joinResult.Receiving.Id,
                TransactionDate = joinResult.Receiving.QC_ReceiveDate,
                ItemCode = joinResult.Receiving.ItemCode,
                ItemDescription = joinResult.RawMaterialsGroup.ItemDescription,
                Quantity = joinResult.Receiving.Actual_Delivered,
                WarehouseId = warehouseReceived
                    .Where(wr => wr.MiscellaneousReceiptId == joinResult.Receiving.Id)
                    .Select(wr => wr.Id)
                    .FirstOrDefault(),
                TransactionType = "Receiving",
                CompanyCode = moveOrderReports
                    .Where(mor => mor.ItemCode == joinResult.Receiving.ItemCode)
                    .Select(mor => mor.CompanyCode)
                    .FirstOrDefault(),
                CompanyName = moveOrderReports
                    .Where(mor => mor.ItemCode == joinResult.Receiving.ItemCode)
                    .Select(mor => mor.CompanyName)
                    .FirstOrDefault(),
                DepartmentName = moveOrderReports
                    .Where(mor => mor.ItemCode == joinResult.Receiving.ItemCode)
                    .Select(mor => mor.DepartmentName)
                    .FirstOrDefault(),
                LocationName = receiptInReport
                    .Where(ro => ro.Id == joinResult.Receiving.Id)
                    .Select(ro => ro.LocationName)
                    .FirstOrDefault(),
                AccountTitle = receiptInReport
                    .Where(ro => ro.Id == joinResult.Receiving.Id)
                    .Select(ro => ro.AccountTitles)
                    .FirstOrDefault()
            }).ToList();

    foreach (var moveOrderReport in moveOrderReports.Where(moveOrderReport => consolidatedReports.All(cr => cr.ItemCode != moveOrderReport.ItemCode)))
    {
        consolidatedReports.Add(new ConsolidatedReport
        {
            Id = moveOrderReport.Id,
            TransactionDate = moveOrderReport.PreparedDate,
            ItemCode = moveOrderReport.ItemCode,
            ItemDescription = moveOrderReport.ItemDescription,
            Quantity = moveOrderReport.QuantityOrdered,
            WarehouseId = moveOrderReport.WarehouseId,
            TransactionType = "MoveOrder",
            CompanyCode = moveOrderReport.CompanyCode,
            CompanyName = moveOrderReport.CompanyName,
            DepartmentName = moveOrderReport.DepartmentName,
            LocationName = moveOrderReport.LocationName,
            AccountTitle = moveOrderReport.AccountTitles
        });
    }

    var miscellaneousReceipts = _context.WarehouseReceived
        .Where(x => x.IsActive == true)
        .Join(
            _context.MiscellaneousReceipts,
            wr => wr.MiscellaneousReceiptId,
            miscReceipt => miscReceipt.Id,
            (wr, miscReceipt) => new { Warehouse = wr, MiscReceipt = miscReceipt })
        .Select(result => new ConsolidatedReport
        {
            Id = result.MiscReceipt.Id,
            TransactionDate = result.MiscReceipt.TransactionDate,
            ItemCode = result.Warehouse.ItemCode,
            ItemDescription = result.Warehouse.ItemDescription,
            Quantity = result.Warehouse.ActualGood,
            WarehouseId = result.Warehouse.Id,
            TransactionType = "Miscellaneous Receipt",
            CompanyCode = result.MiscReceipt.CompanyCode,
            CompanyName = result.MiscReceipt.CompanyName,
            DepartmentName = result.MiscReceipt.DepartmentName,
            LocationName = result.MiscReceipt.LocationName,
            AccountTitle = result.MiscReceipt.AccountTitles
        })
        .ToList();

    var miscellaneousIssues = _context.MiscellaneousIssues
        .Where(wr => wr.IsTransact == true)
        .Join(
            _context.MiscellaneousIssueDetails,
            issue => issue.Id,
            details => details.IssuePKey,
            (issue, details) => new { Issue = issue, Details = details })
        .Select(result => new ConsolidatedReport
        {
            Id = result.Issue.Id,
            TransactionDate = result.Issue.TransactionDate,
            ItemCode = result.Details.ItemCode,
            ItemDescription = result.Details.ItemDescription,
            Quantity = result.Issue.TotalQuantity,
            WarehouseId = result.Details.WarehouseId,
            TransactionType = "Miscellaneous Issue",
            CompanyCode = result.Issue.CompanyCode,
            CompanyName = result.Issue.CompanyName,
            DepartmentName = result.Issue.DepartmentName,
            LocationName = result.Issue.LocationName,
            AccountTitle = result.Issue.AccountTitles
        })
        .ToList();

    consolidatedReports.AddRange(miscellaneousReceipts);
    consolidatedReports.AddRange(miscellaneousIssues);

    return consolidatedReports;
        }
    }
}

