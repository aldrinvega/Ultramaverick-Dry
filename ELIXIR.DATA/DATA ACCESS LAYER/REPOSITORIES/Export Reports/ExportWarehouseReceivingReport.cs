using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports.ExportItemsWithBbdReport;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;

[Route("api/ExportReports"), ApiController]
public class ExportWarehouseReceivingReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportWarehouseReceivingReport(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("ExportWarehouseReceivingReport")]
    public async Task<IActionResult> Export([FromQuery] ExportWarehouseReceivingReportCommand command)
    {
        var filePath = $"Warehouse Receiving Reports {command.DateFrom} - {command.DateTo}.xlsx";
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
        catch (System.Exception e)
        {
            return Conflict(e.Message);
        }
    }

    public class ExportWarehouseReceivingReportCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class Handler : IRequestHandler<ExportWarehouseReceivingReportCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportWarehouseReceivingReportCommand request, CancellationToken cancellationToken)
        {
            var warehouseReceivingReports = await _reportRepository
                .WarehouseReceivingReport(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Warehouse Receiving Reports");

                var headers = new List<string>
                    {
                        "Warehouse Id",
                        "PO Number",
                        "Receive Date",
                        "Item Code",
                        "Item Description",
                        "Uom",
                        "Category",
                        "Quantity",
                        "Manufacturing Date",
                        "Expiration Date",
                        "Total Reject",
                        "Supplier Name",
                        "Received By",
                        "Transaction Type"
                    };


                var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                range.Style.Fill.BackgroundColor = XLColor.Azure;
                range.Style.Font.Bold = true;
                range.Style.Font.FontColor = XLColor.Black;
                range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                for (var index = 1; index <= headers.Count; index++)
                {
                    worksheet.Cell(1, index).Value = headers[index - 1];
                }

                for (var index = 0; index < warehouseReceivingReports.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = warehouseReceivingReports[index].WarehouseId;
                    row.Cell(2).Value = warehouseReceivingReports[index].PONumber;
                    row.Cell(3).Value = warehouseReceivingReports[index].ReceiveDate;
                    row.Cell(4).Value = warehouseReceivingReports[index].ItemCode;
                    row.Cell(5).Value = warehouseReceivingReports[index].ItemDescription;
                    row.Cell(6).Value = warehouseReceivingReports[index].Uom;
                    row.Cell(7).Value = warehouseReceivingReports[index].Category;
                    row.Cell(8).Value = warehouseReceivingReports[index].Quantity;
                    row.Cell(9).Value = warehouseReceivingReports[index].ManufacturingDate;
                    row.Cell(10).Value = warehouseReceivingReports[index].ExpirationDate;
                    row.Cell(11).Value = warehouseReceivingReports[index].TotalReject;
                    row.Cell(12).Value = warehouseReceivingReports[index].SupplierName;
                    row.Cell(13).Value = warehouseReceivingReports[index].ReceivedBy;
                    row.Cell(14).Value = warehouseReceivingReports[index].TransactionType;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Warehouse Receiving Reports {request.DateFrom} - {request.DateTo}.xlsx");

            }

            return Unit.Value;
        }
    }
}
