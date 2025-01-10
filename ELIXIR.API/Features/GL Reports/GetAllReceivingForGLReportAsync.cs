using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.GL_Reports;

[Route("api/export-report")]
public class GetAllReceivingForGlReportAsync : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAllReceivingForGlReportAsync(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("general-ledger-report")]
    public async Task<IActionResult> Export([FromQuery] GetAllReceivingForGlReportQuery query)
    {
        var filePath = $"General Ledger Report.xlsx";
        try
        {
            await _mediator.Send(query);

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filePath);
            System.IO.File.Delete(filePath);
            return result;
        }
        catch (Exception e)
        {
            return Conflict(e.Message);
        }
    }

    public class GetAllReceivingForGlReportQuery : IRequest<IList<ConsolidatedReportsResult>>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    //public class GetAllReceivingForGLReportResult
    //{
    //    public int PO_Number { get; set; }
    //    public string ItemCode { get; set; }
    //    public string ItemDescription { get; set; }
    //    public decimal? UnitCost { get; set; }
    //    public decimal ActualGood { get; set; }
    //    public decimal? TotalAmount { get; set; }
    //    public string AccountTitle { get; set; }
    //    public string TransactionType { get; set; }
    //}

    public class ConsolidatedReportsResult
    {
        public string SyncId { get; set; }
        public string MIRId { get; set; }
        public int? Id { get; set; }
        public string DateAdded { get; set; }
        public DateTime TransactDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Category { get; set; }
        public string UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Amount { get; set; }
        public int? Source { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
        public string Encoded { get; set; }
        public decimal? Quantity { get; set; }
        public int? WarehouseId { get; set; }
        public string TransactionType { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string AccountTitleCode { get; set; }
        public string AccountTitle { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public string EmployeeName { get; set; }
        public string DRCP { get; set; }
    }

    public class Handler : IRequestHandler<GetAllReceivingForGlReportQuery, IList<ConsolidatedReportsResult>>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<IList<ConsolidatedReportsResult>> Handle(GetAllReceivingForGlReportQuery request, CancellationToken cancellationToken)
        {
            var fromDate = DateTime.Parse(request.DateFrom).Date;
            var toDate = DateTime.Parse(request.DateTo).Date;

            var individualDifferences = from wr in _context.WarehouseReceived
                                        join mo in _context.MoveOrders
                                            on wr.Id equals mo.WarehouseId
                                            into moveOrders
                                        from mo in moveOrders.DefaultIfEmpty()
                                        where wr.IsActive && wr.IsWarehouseReceive
                                        select new
                                        {
                                            wr.ItemCode,
                                            wr.ActualGood,
                                            QuantityOrdered = mo != null ? mo.QuantityOrdered : 0,
                                            CostByWarehouse = wr.UnitCost * (wr.ActualGood - (mo != null ? mo.QuantityOrdered : 0))
                                        };

            // Calculate the sum of differences per ItemCode
            var totalDifferences = individualDifferences
                .GroupBy(id => id.ItemCode)
                .Select(g => new
                {
                    ItemCode = g.Key,
                    TotalDifference = g.Sum(id => id.CostByWarehouse)
                });

            // Calculate the average UnitCost per ItemCode
            var averageUnitCosts = individualDifferences
                .GroupBy(id => id.ItemCode)
                .Select(g => new
                {
                    ItemCode = g.Key,
                    AvgUnitCost = g.Average(id =>
                        (id.ActualGood - id.QuantityOrdered) == 0
                            ? 0
                            : id.CostByWarehouse / (id.ActualGood - id.QuantityOrdered))
                });

            // Combine the results
            var finalResult = from id in individualDifferences
                              join td in totalDifferences
                                  on id.ItemCode equals td.ItemCode
                              join auc in averageUnitCosts
                                  on id.ItemCode equals auc.ItemCode
                              select new
                              {
                                  id.ItemCode,
                                  id.ActualGood,
                                  id.QuantityOrdered,
                                  Difference = id.CostByWarehouse,
                                  td.TotalDifference,
                                  auc.AvgUnitCost
                              };


            var consolidatedReports = new List<ConsolidatedReportsResult>();
            var rawMaterials = await _context.RawMaterials
                .Include(x => x.ItemCategory)
                .Include(uom => uom.UOM)
                .Select(rawMaterial => new
                {
                    rawMaterial.ItemCode,
                    rawMaterial.ItemDescription,
                    rawMaterial.UOM,
                    rawMaterial.ItemCategory.ItemCategoryName,
                    rawMaterial.UOM.UOM_Description
                })
                .ToListAsync(cancellationToken: cancellationToken);

            var warehouseReceived = await _context.WarehouseReceived
                .Select(warehouseReceived => new
                {
                    warehouseReceived.PO_Number,
                    warehouseReceived.QcReceivingId,
                    warehouseReceived.Id,
                    warehouseReceived.ItemCode,
                    warehouseReceived.Uom,
                    warehouseReceived.MiscellaneousReceiptId,
                    warehouseReceived.UnitCost,
                    warehouseReceived.ActualGood
                })
                .ToListAsync(cancellationToken: cancellationToken);

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
                .ToListAsync(cancellationToken: cancellationToken);

            var moveOrderReports = await

                (from transactmoveorder in _context.TransactMoveOrder
                 where transactmoveorder.DeliveryDate.Value.Date >= fromDate && transactmoveorder.DeliveryDate.Value.Date <= toDate
                 join moveorder in _context.MoveOrders.Where(x => x.IsActive == true && x.IsRejectForPreparation == null)
                     on transactmoveorder.OrderNo equals moveorder.OrderNo into leftJ
                 from moveorder in leftJ.DefaultIfEmpty()
                 select new
                 {
                     moveorder.TransactionId,
                     moveorder.Id,
                     moveorder.ItemCode,
                     moveorder.ItemDescription,
                     moveorder.Category,
                     moveorder.OrderNo,
                     moveorder.FarmName,
                     moveorder.FarmCode,
                     moveorder.Uom,
                     moveorder.CompanyCode,
                     moveorder.CompanyName,
                     moveorder.DepartmentCode,
                     moveorder.DepartmentName,
                     moveorder.AccountTitleCode,
                     moveorder.AccountTitles,
                     moveorder.LocationCode,
                     moveorder.LocationName,
                     moveorder.QuantityOrdered,
                     moveorder.WarehouseId,
                     transactmoveorder.PreparedDate,
                     transactmoveorder.PreparedBy,
                     moveorder.DeliveryStatus,
                     moveorder.AdvancesToEmployees.EmployeeName,
                     moveorder.AdvancesToEmployees.EmployeeId
                 }).ToListAsync(cancellationToken: cancellationToken);


            consolidatedReports = _context.WarehouseReceived
                    .Where(wr => wr.IsActive == true && wr.MiscellaneousReceiptId == null)
                    .Where(wr => wr.ReceivingDate.Date >= fromDate && wr.ReceivingDate.Date <= toDate)
                    .Join(
                        _context.RawMaterials
                            .Include(rm => rm.UOM)
                            .Include(rm => rm.ItemCategory),
                        receiving => receiving.ItemCode,
                        rawMaterial => rawMaterial.ItemCode,
                        (receiving, rawMaterialsGroup) =>
                            new { Receiving = receiving, RawMaterialsGroup = rawMaterialsGroup })
                     .Join(
                        _context.QC_Receiving,
                        joinResult => joinResult.Receiving.QcReceivingId,
                        qc => qc.Id,
                        (joinResult, qc) => new { Receiving = joinResult.Receiving, RawMaterialsGroup = joinResult.RawMaterialsGroup, QC = qc })
                    .AsEnumerable()
                    .Select(
                        (joinResult, rawMaterial) => new ConsolidatedReportsResult
                        {
                            SyncId = GetMD5Hash($"R_{joinResult.Receiving.Id.ToString()}"),
                            Id = joinResult.Receiving.Id,
                            TransactDate = joinResult.Receiving.ReceivingDate,
                            ItemCode = rawMaterials.Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.ItemCode)
                            .FirstOrDefault(),
                            ItemDescription = rawMaterials.Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.ItemDescription)
                            .FirstOrDefault(),
                            Category = rawMaterials
                            .Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.ItemCategoryName)
                            .FirstOrDefault(),
                            UOM = rawMaterials
                            .Where(item => item.ItemCode == joinResult.RawMaterialsGroup.ItemCode)
                            .Select(item => item.UOM_Description)
                            .FirstOrDefault(),
                            Quantity = joinResult.Receiving.ActualGood,
                            UnitPrice = Math.Round(warehouseReceived
                            .Where(wr => wr.PO_Number == joinResult.Receiving.PO_Number && wr.ItemCode == joinResult.Receiving.ItemCode && wr.QcReceivingId == joinResult.QC.Id)
                            .Select(wr => wr.UnitCost)
                            .FirstOrDefault() ?? 0, 2),
                            WarehouseId = warehouseReceived
                                .Where(wr => wr.PO_Number == joinResult.Receiving.PO_Number && wr.ItemCode == joinResult.Receiving.ItemCode && wr.QcReceivingId == joinResult.QC.Id)
                                .Select(wr => wr.Id)
                                .FirstOrDefault(),
                            TransactionType = "Receiving",
                            CompanyCode = "31",
                            CompanyName = "Fresh Options",
                            DepartmentCode = "5001",
                            DepartmentName = "Meatshop Administration",
                            LocationCode = "0",
                            LocationName = "Common",
                            AccountTitle = "Accrued Expense Payable",
                            AccountTitleCode = "211201",
                            Reason = joinResult.Receiving.PO_Number.ToString(),
                            MIRId = "-",
                            Source = joinResult.Receiving.PO_Number,
                            Reference = joinResult.Receiving.Supplier,
                            Encoded = joinResult.Receiving.ReceivedBy,
                            Details = null
                        }).ToList();


            foreach (var moveOrderReport in moveOrderReports)
            {
                consolidatedReports.Add(new ConsolidatedReportsResult
                {
                    SyncId = GetMD5Hash($"MO_{moveOrderReport.Id.ToString()}"),
                    Id = moveOrderReport.Id,
                    TransactDate = moveOrderReport.PreparedDate.Value,
                    ItemCode = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.ItemCode)
                            .FirstOrDefault(),
                    ItemDescription = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.ItemDescription)
                            .FirstOrDefault(),
                    Category = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.ItemCategoryName)
                            .FirstOrDefault(),
                    UOM = rawMaterials.Where(item => item.ItemCode == moveOrderReport.ItemCode)
                            .Select(item => item.UOM_Description)
                            .FirstOrDefault(),
                    Quantity = moveOrderReport.QuantityOrdered,
                    WarehouseId = moveOrderReport.WarehouseId,
                    UnitPrice = warehouseReceived.Where(wr => wr.Id == moveOrderReport.WarehouseId)
                        .Select(x => x.UnitCost).FirstOrDefault(),
                    TransactionType = "Move Order",
                    CompanyCode = moveOrderReport.CompanyCode,
                    CompanyName = moveOrderReport.CompanyName,
                    DepartmentCode = moveOrderReport.DepartmentCode,
                    DepartmentName = moveOrderReport.DepartmentName,
                    LocationCode = moveOrderReport.LocationCode,
                    LocationName = moveOrderReport.LocationName,
                    AccountTitleCode = moveOrderReport.AccountTitleCode,
                    AccountTitle = moveOrderReport.AccountTitles,
                    MIRId = moveOrderReport.TransactionId,
                    Source = moveOrderReport.OrderNo,
                    Reference = moveOrderReport.FarmName,
                    Encoded = moveOrderReport.PreparedBy,
                    Reason = moveOrderReport.DeliveryStatus,
                    Details = null,
                    Status = moveOrderReport.PreparedDate != null ? "Transacted" : "Pending",
                    EmployeeName = moveOrderReport.EmployeeName
                });
            }

            var miscellaneousReceipts = await (from receiptInReports in _context.MiscellaneousReceipts
                                               join warehouseRec in _context.WarehouseReceived
                                                   on receiptInReports.Id equals warehouseRec.MiscellaneousReceiptId
                                               join rm in _context.RawMaterials
                                                .Include(category => category.ItemCategory)
                                                .Include(uom => uom.UOM)
                                                   on warehouseRec.ItemCode equals rm.ItemCode
                                               join itemCategory in _context.ItemCategories
                                                   on rm.ItemCategoryId equals itemCategory.Id
                                               where receiptInReports.IsActive
                                                     && receiptInReports.TransactionDate.Date >= fromDate
                                                     && receiptInReports.TransactionDate.Date <= toDate

                                               select new ConsolidatedReportsResult
                                               {
                                                   SyncId = GetMD5Hash($"MR_{receiptInReports.Id.ToString()}"),
                                                   Id = receiptInReports.Id,
                                                   TransactDate = receiptInReports.TransactionDate,
                                                   ItemCode = rm.ItemCode,
                                                   ItemDescription = rm.ItemDescription,
                                                   UOM = rm.UOM.UOM_Description,
                                                   Category = rm.ItemCategory.ItemCategoryName,
                                                   Quantity = warehouseRec.ActualGood,
                                                   UnitPrice = warehouseRec.UnitCost,
                                                   WarehouseId = warehouseRec.Id,
                                                   TransactionType = "Miscellaneous Receipt",
                                                   CompanyName = receiptInReports.CompanyName,
                                                   CompanyCode = receiptInReports.CompanyCode,
                                                   DepartmentCode = receiptInReports.DepartmentCode,
                                                   DepartmentName = receiptInReports.DepartmentName,
                                                   AccountTitleCode = receiptInReports.AccountTitleCode,
                                                   AccountTitle = receiptInReports.AccountTitles,
                                                   LocationCode = receiptInReports.LocationCode,
                                                   LocationName = receiptInReports.LocationName,
                                                   Encoded = receiptInReports.PreparedBy,
                                                   Source = receiptInReports.Id,
                                                   Reference = receiptInReports.CompanyName,
                                                   Reason = receiptInReports.Remarks,
                                                   Details = receiptInReports.Details,
                                                   Status = "-",
                                                   MIRId = "-"
                                               }).ToListAsync();

            consolidatedReports.AddRange(miscellaneousReceipts);

            var miscellaneousIssues = _context.MiscellaneousIssues
                .Where(wr => wr.IsTransact == true && wr.IsActive == true)
                .Where(x => x.TransactionDate.Date >= fromDate && x.TransactionDate.Date <= toDate)
                .Join(
                    _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true),
                    issue => issue.Id,
                    details => details.IssuePKey,
                    (issue, details) => new { Issue = issue, Details = details })
                .Join(
                    _context.RawMaterials
                    .Include(uom => uom.UOM),
                    result => result.Details.ItemCode,
                    rawMaterial => rawMaterial.ItemCode,
                    (result, rawMaterial) => new { Result = result, RawMaterial = rawMaterial })
                .Join(
                    _context.ItemCategories,
                    combined => combined.RawMaterial.ItemCategoryId,
                    itemCategory => itemCategory.Id,
                    (combined, itemCategory) => new { Combined = combined, ItemCategory = itemCategory })
                .Select(consolidated => new ConsolidatedReportsResult
                {
                    SyncId = GetMD5Hash($"MI_{consolidated.Combined.Result.Issue.Id.ToString()}"),
                    Id = consolidated.Combined.Result.Issue.Id,
                    TransactDate = consolidated.Combined.Result.Issue.TransactionDate,
                    ItemCode = consolidated.Combined.RawMaterial.ItemCode,
                    ItemDescription = consolidated.Combined.RawMaterial.ItemDescription,
                    UOM = consolidated.Combined.RawMaterial.UOM.UOM_Description,
                    Category = consolidated.ItemCategory.ItemCategoryName,
                    Quantity = consolidated.Combined.Result.Details.Quantity,
                    WarehouseId = consolidated.Combined.Result.Details.WarehouseId,
                    UnitPrice = consolidated.Combined.Result.Details.UnitCost,
                    TransactionType = "Miscellaneous Issue",
                    CompanyCode = consolidated.Combined.Result.Issue.CompanyCode,
                    CompanyName = consolidated.Combined.Result.Issue.CompanyName,
                    DepartmentCode = consolidated.Combined.Result.Issue.DepartmentCode,
                    DepartmentName = consolidated.Combined.Result.Issue.DepartmentName,
                    LocationCode = consolidated.Combined.Result.Issue.LocationCode,
                    LocationName = consolidated.Combined.Result.Issue.LocationName,
                    AccountTitleCode = consolidated.Combined.Result.Issue.AccountTitleCode,
                    AccountTitle = consolidated.Combined.Result.Issue.AccountTitles,
                    Source = consolidated.Combined.Result.Issue.Id,
                    Reason = consolidated.Combined.Result.Details.Remarks,
                    Reference = consolidated.Combined.Result.Details.Customer,
                    Encoded = consolidated.Combined.Result.Issue.PreparedBy,
                    Details = consolidated.Combined.Result.Issue.Details,
                    Status = "-",
                    MIRId = "-"

                })
                .ToList();
            consolidatedReports.AddRange(miscellaneousIssues);


            var result = consolidatedReports
           .GroupBy(r => new { r.TransactionType, r.ItemCode })
           .Select(g => new ConsolidatedReportsResult
           {
               SyncId = g.First().SyncId,
               Id = g.First().Id,
               TransactDate = g.First().TransactDate,
               ItemCode = g.Key.ItemCode,
               ItemDescription = g.First().ItemDescription,
               Category = g.First().Category,
               UOM = g.First().UOM,
               Quantity = g.Sum(r => r.Quantity),
               WarehouseId = g.First().WarehouseId,
               TransactionType = g.Key.TransactionType,
               CompanyCode = g.First().CompanyCode,
               CompanyName = g.First().CompanyName,
               DepartmentCode = g.First().DepartmentCode,
               DepartmentName = g.First().DepartmentName,
               LocationCode = g.First().LocationCode,
               LocationName = g.First().LocationName,
               AccountTitleCode = g.First().AccountTitleCode,
               AccountTitle = g.First().AccountTitle,
               Amount = g.Sum(r => r.Quantity * g.First().UnitPrice),
               UnitPrice = g.First().UnitPrice,
               Source = g.First().Source,
               Reference = g.First().Reference,
               Reason = g.First().Reason,
               Encoded = g.First().Encoded,
               Details = g.First().Details,
               Status = g.First().Status,
               MIRId = g.First().MIRId,
               DRCP = "Debit"
           })
           .ToList();

            var credit = consolidatedReports
            .GroupBy(r => new { r.TransactionType, r.ItemCode })
            .Select(g => new ConsolidatedReportsResult
            {
                SyncId = g.First().SyncId,
                Id = g.First().Id,
                TransactDate = g.First().TransactDate,
                ItemCode = g.Key.ItemCode,
                ItemDescription = g.First().ItemDescription,
                Category = g.First().Category,
                UOM = g.First().UOM,
                Quantity = g.Sum(r => r.Quantity),
                WarehouseId = g.First().WarehouseId,
                TransactionType = g.Key.TransactionType,
                CompanyCode = g.First().CompanyCode,
                CompanyName = g.First().CompanyName,
                DepartmentCode = g.First().DepartmentCode,
                DepartmentName = g.First().DepartmentName,
                LocationCode = g.First().LocationCode,
                LocationName = g.First().LocationName,
                AccountTitleCode = g.First().AccountTitleCode,
                AccountTitle = g.First().AccountTitle,
                Amount = -g.Sum(r => r.Quantity * g.First().UnitPrice),
                UnitPrice = g.First().UnitPrice,
                Source = g.First().Source,
                Reference = g.First().Reference,
                Reason = g.First().Reason,
                Encoded = g.First().Encoded,
                Details = g.First().Details,
                Status = g.First().Status,
                MIRId = g.First().MIRId,
                DRCP = "Credit"
            })
            .ToList();

            result.AddRange(credit);


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"General Ledger Report");

                var headers = new List<string>
                {
                    "Sync Id", 
                    "Mark", 
                    "Mark 2", 
                    "Asset / CIP #", 
                    "Accounting Tag", 
                    "Transaction Date",
                    "Supplier / Customer", 
                    "Account Title Code", 
                    "Account Title", 
                    "Company Code",
                    "Company", 
                    "Division Code", 
                    "Division", 
                    "Department Code", 
                    "Department",
                    "Unit Code", 
                    "Unit", 
                    "Sub Unit Code", 
                    "Sub Unit", 
                    "Location Code", 
                    "Location",
                    "PO No.", 
                    "Reference No.", 
                    "Item Code", 
                    "Description", 
                    "Quantity", 
                    "unit",
                    "Unit Price", 
                    "Line Amount", 
                    "Voucher / GJ No.", 
                    "Account Type", 
                    "DR / CR",
                    "Asset Code", 
                    "Asset", 
                    "Service Provider Code", 
                    "Service Provider", 
                    "BOA",
                    "Allocation", 
                    "Account Group", 
                    "Account SubGroup", 
                    "Financial Statement",
                    "Unit Responsible", 
                    "Batch", 
                    "Remarks", 
                    "Payroll Period", 
                    "Position",
                    "Payroll Type 1", 
                    "Payroll Type 2", 
                    "Additional Description for DEPR",
                    "Remaining BV for DEPR", 
                    "Useful Life", 
                    "Month", 
                    "Year", 
                    "Division",
                    "Particulars", 
                    "Month 2", 
                    "Farm Type", 
                    "Jean Remarks", 
                    "From", 
                    "Changed To",
                    "Reason", 
                    "Checking Remarks", 
                    "BOA 2", 
                    "System", 
                    "Books"
                };


                var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                range.Style.Fill.BackgroundColor = XLColor.Azure;
                range.Style.Font.Bold = true;
                range.Style.Font.FontColor = XLColor.Black;
                range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.SetAutoFilter(true);

                for (var index = 1; index <= headers.Count; index++)
                {
                    worksheet.Cell(1, index).Value = headers[index - 1];
                }

                for (var index = 1; index <= result.Count; index++)
                {
                    var row = worksheet.Row(index + 1);
                    row.Cell(1).Value = result[index - 1].SyncId;
                    row.Cell(2).Value = " ";
                    row.Cell(3).Value = " ";
                    row.Cell(4).Value = " ";
                    row.Cell(5).Value = " ";
                    row.Cell(6).Value = result[index - 1].TransactDate;
                    row.Cell(7).Value = "RDF";
                    row.Cell(8).Value = result[index - 1].AccountTitleCode;
                    row.Cell(9).Value = result[index - 1].AccountTitle;
                    row.Cell(10).Value = result[index - 1].CompanyCode;
                    row.Cell(11).Value = result[index - 1].CompanyName;
                    row.Cell(12).Value = " ";
                    row.Cell(13).Value = " ";
                    row.Cell(14).Value = result[index - 1].DepartmentCode;
                    row.Cell(15).Value = result[index - 1].DepartmentName;
                    row.Cell(16).Value = " ";
                    row.Cell(17).Value = " ";
                    row.Cell(18).Value = " ";
                    row.Cell(19).Value = " ";
                    row.Cell(20).Value = result[index - 1].LocationCode;
                    row.Cell(21).Value = result[index - 1].LocationName;
                    row.Cell(22).Value = " ";
                    row.Cell(23).Value = result[index - 1].Source;
                    row.Cell(24).Value = result[index - 1].ItemCode;
                    row.Cell(25).Value = result[index - 1].ItemDescription;
                    row.Cell(26).Value = result[index - 1].Quantity;
                    row.Cell(27).Value = result[index - 1].UOM;
                    row.Cell(28).Value = result[index - 1].UnitPrice;
                    row.Cell(29).Value = result[index - 1].Amount;
                    row.Cell(30).Value = " ";
                    row.Cell(31).Value = " ";
                    row.Cell(32).Value = result[index - 1].DRCP;
                    row.Cell(33).Value = " ";
                    row.Cell(34).Value = " ";
                    row.Cell(35).Value = " ";
                    row.Cell(36).Value = result[index - 1].Encoded;
                    row.Cell(37).Value = "MIR - Central Depot";
                    row.Cell(38).Value = " ";
                    row.Cell(39).Value = " ";
                    row.Cell(40).Value = " ";
                    row.Cell(41).Value = " ";
                    row.Cell(42).Value = " ";
                    row.Cell(43).Value = " ";
                    row.Cell(44).Value = " ";
                    row.Cell(45).Value = " ";
                    row.Cell(46).Value = " ";
                    row.Cell(47).Value = " ";
                    row.Cell(48).Value = " ";
                    row.Cell(49).Value = " ";
                    row.Cell(50).Value = " ";
                    row.Cell(51).Value = " ";
                    row.Cell(52).Value = result[index - 1].TransactDate.ToString("MMMM");
                    row.Cell(53).Value = result[index - 1].TransactDate.ToString("yyyy");
                    row.Cell(54).Value = " ";
                    row.Cell(55).Value = " ";
                    row.Cell(56).Value = " ";
                    row.Cell(57).Value = " ";
                    row.Cell(58).Value = " ";
                    row.Cell(59).Value = " ";
                    row.Cell(60).Value = " ";
                    row.Cell(61).Value = " ";
                    row.Cell(62).Value = " ";
                    row.Cell(63).Value = " ";
                    row.Cell(64).Value = "Ultra Maverick Dry";
                    row.Cell(65).Value = "GJ";
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"General Ledger Report.xlsx");
            }
            return result;
        }
        private static string GetMD5Hash(string input)
        {
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
            var sb = new System.Text.StringBuilder();
            for (var i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
