using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;
[Route("api/ExportReports"), ApiController]
public class ExportInventoryMovementReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportInventoryMovementReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportInventoryMovementReport")]
    public async Task<IActionResult> Export([FromQuery]ExportInventoryMovementReportCommand command)
    {
        var filePath = $"Inventory Movement Report {command.DateFrom}-{command.DateTo}.xlsx";
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

    public class ExportInventoryMovementReportCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string PlusOne { get; set; }
    }

    public class Handler : IRequestHandler<ExportInventoryMovementReportCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportInventoryMovementReportCommand request, CancellationToken cancellationToken)
        {
            var inventoryMovementReport = await _reportRepository.InventoryMovementReport(request.DateFrom, request.DateTo, request.PlusOne);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Inventory Movement Report");

                var headers = new List<string>
                {
                    "Item Code",
                    "Item Description",
                    "Item Category",
                    "Total Out",
                    "Total In",
                    "Ending",
                    "Current Stock",
                    "Purchased Order",
                    "Others Plus"
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

                for (var index = 1; index <= inventoryMovementReport.Count; index++)
                {
                    var row = worksheet.Row(index + 1);

                    row.Cell(1).Value = inventoryMovementReport[index - 1].ItemCode;
                    row.Cell(2).Value = inventoryMovementReport[index - 1].ItemDescription;
                    row.Cell(3).Value = inventoryMovementReport[index - 1].ItemCategory;
                    row.Cell(4).Value = inventoryMovementReport[index - 1].TotalOut;
                    row.Cell(5).Value = inventoryMovementReport[index - 1].TotalIn;
                    row.Cell(6).Value = inventoryMovementReport[index - 1].Ending;
                    row.Cell(7).Value = inventoryMovementReport[index - 1].CurrentStock;
                    row.Cell(8).Value = inventoryMovementReport[index - 1].PurchasedOrder;
                    row.Cell(9).Value = inventoryMovementReport[index - 1].OthersPlus;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Inventory Movement Report {request.DateFrom}-{request.DateTo}.xlsx");
            }

            return Unit.Value;
        }
    }
}
