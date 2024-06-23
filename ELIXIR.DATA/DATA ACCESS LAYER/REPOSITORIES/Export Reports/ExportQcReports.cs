using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using ClosedXML.Excel;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;

[Route("api/ExportReports"), ApiController]
public class ExportQcReports : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportQcReports(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("qc-receiving/export")]
    public async Task<IActionResult> Add([FromQuery] ExportQcReportsQuery command)
    {
        var filePath = $"QC Receiving Rejects {command.DateFrom} - {command.DateTo}.xlsx";
        try
        {
            await _mediator.Send(command);

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

    public class ExportQcReportsQuery : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class ExportQcReportsQueryResult
    {
        public int? ReceivingId { get; set; }
        public string QcDate { get; set; }
        public string Status { get; set; }
        public int PONumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string Category { get; set; }
        public decimal? Quantity { get; set; }
        public string ManufacturingDate { get; set; }
        public string ExpirationDate { get; set; }
        public string SupplierName { get; set; }
        public decimal? Price { get; set; }
        public string QcBy { get; set; }
        public string Reason { get; set; }
        public string CancelledDate { get; set; }
    }

    public class Handler : IRequestHandler<ExportQcReportsQuery, Unit>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ExportQcReportsQuery request, CancellationToken cancellationToken)
        {
            var toDate = DateTime.Parse(request.DateTo);
            var fromDate = DateTime.Parse(request.DateFrom);

            var receivedItemsQuery = (from rawmaterials in _context.RawMaterials
                                      where rawmaterials.IsActive == true
                                      join category in _context.ItemCategories
                                          on rawmaterials.ItemCategoryId equals category.Id into leftJ
                                      from category in leftJ.DefaultIfEmpty()
                                      select new
                                      {
                                          ItemCode = rawmaterials.ItemCode,
                                          ItemCategory = category.ItemCategoryName
                                      });

            var receivedItems = await (from receiving in _context.QC_Receiving
                                       where receiving.QC_ReceiveDate >= fromDate && receiving.QC_ReceiveDate <= toDate
                                       join posummary in _context.POSummary
                                           on receiving.PO_Summary_Id equals posummary.Id into leftJ
                                       from posummary in leftJ.DefaultIfEmpty()
                                       join category in receivedItemsQuery
                                           on receiving.ItemCode equals category.ItemCode into leftJ2
                                       from category in leftJ2.DefaultIfEmpty()
                                       select new ExportQcReportsQueryResult
                                       {
                                           ReceivingId = receiving.Id,
                                           QcDate = receiving.QC_ReceiveDate.ToString("MM-dd-yyyy"),
                                           Status = receiving.IsActive == true ? "Received" : "Cancelled",
                                           PONumber = posummary != null ? posummary.PO_Number : 0,
                                           ItemCode = receiving.ItemCode,
                                           ItemDescription = posummary != null ? posummary.ItemDescription : null,
                                           Uom = posummary != null ? posummary.UOM : null,
                                           Category = category != null ? category.ItemCategory : null,
                                           Quantity = receiving.Actual_Delivered,
                                           ManufacturingDate = receiving.Manufacturing_Date.HasValue ? receiving.Manufacturing_Date.Value.ToString("MM-dd-yyyy") : null,
                                           ExpirationDate = receiving.Expiry_Date.HasValue ? receiving.Expiry_Date.Value.ToString("MM-dd-yyyy") : "Not Expirable",
                                           SupplierName = posummary != null ? posummary.VendorName : null,
                                           Price = posummary != null ? posummary.UnitPrice : 0,
                                           QcBy = receiving.QcBy,
                                           Reason = null, // No rejection reason for received items
                                           CancelledDate = null
                                       })
                                       .OrderBy(si => si.ReceivingId)
                                       .ToListAsync(cancellationToken);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Consolidated Report");

                var headers = new List<string>
                {
                    "Receiving Id",
                    "QC Date",
                    "Status",
                    "PO Number",
                    "Item Code",
                    "Item Description",
                    "Uom",
                    "Category",
                    "Quantity",
                    "Manufacturing Date",
                    "Expiration Date",
                    "Supplier Name",
                    "Price",
                    "Qc By",
                    "Reason",
                    "Cancelled Date"
                };

                var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                range.Style.Fill.BackgroundColor = XLColor.Azure;
                range.Style.Font.Bold = true;
                range.Style.Font.FontColor = XLColor.Black;
                range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                range.SetAutoFilter(true);

                var headerRow = worksheet.Row(1);
                for (var index = 0; index < headers.Count; index++)
                {
                    headerRow.Cell(index + 1).Value = headers[index];
                }

                for (var index = 0; index < receivedItems.Count; index++)
                {
                    var row = worksheet.Row(index + 2);
                    var report = receivedItems[index];

                    row.Cell(1).Value = report.ReceivingId;
                    row.Cell(2).Value = report.QcDate;
                    row.Cell(3).Value = report.Status;
                    row.Cell(4).Value = report.PONumber;
                    row.Cell(5).Value = report.ItemCode;
                    row.Cell(6).Value = report.ItemDescription;
                    row.Cell(7).Value = report.Uom;
                    row.Cell(8).Value = report.Category;
                    row.Cell(9).Value = report.Quantity;
                    row.Cell(10).Value = report.ManufacturingDate;
                    row.Cell(11).Value = report.ExpirationDate;
                    row.Cell(12).Value = report.SupplierName;
                    row.Cell(13).Value = report.Price;
                    row.Cell(14).Value = report.QcBy;
                    row.Cell(15).Value = report.Reason;
                    row.Cell(16).Value = report.CancelledDate;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"QC Receiving Rejects {request.DateFrom} - {request.DateTo}.xlsx");
                
            }

            return Unit.Value;
        }
    }
}
