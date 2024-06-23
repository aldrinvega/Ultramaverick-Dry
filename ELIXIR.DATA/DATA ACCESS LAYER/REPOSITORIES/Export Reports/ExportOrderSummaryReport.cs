using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports.ExportCancelledOrdersReport;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;

[Route("api/ExportReports"), ApiController]
public class ExportOrderSummaryReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportOrderSummaryReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportOrderSummaryReport")]
    public async Task<IActionResult> Export([FromQuery] ExportOrderSummaryReportQuery command)
    {
        var filePath = $"Order Summary Report {command.DateFrom}-{command.DateTo}.xlsx";
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

    public class ExportOrderSummaryReportQuery : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class Handler : IRequestHandler<ExportOrderSummaryReportQuery, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportOrderSummaryReportQuery request, CancellationToken cancellationToken)
        {
            var orders = await _reportRepository.OrderSummaryReports(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Order Summary Report");

                var headers = new List<string>
                 {
                    "MIR Id",
                    "Date Needed",
                    "Date Ordered",
                    "Customer Code",
                    "Customer Name",
                    "Item Code",
                    "Item Description",
                    "Quantity Ordered",
                    "Status",
                    "Cancelled Date",
                    "Cancelled By",
                    "Reason"
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

                for (var index = 0; index < orders.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = orders[index].MirId;
                    row.Cell(2).Value = orders[index].DateNeeded;
                    row.Cell(3).Value = orders[index].DateOrdered;
                    row.Cell(4).Value = orders[index].CustomerCode;
                    row.Cell(5).Value = orders[index].CustomerName;
                    row.Cell(6).Value = orders[index].ItemCode;
                    row.Cell(7).Value = orders[index].ItemDescription;
                    row.Cell(8).Value = orders[index].QuantityOrdered;
                    row.Cell(9).Value = orders[index].Status;
                    row.Cell(10).Value = orders[index].CancelledDate;
                    row.Cell(11).Value = orders[index].CancelledBy;
                    row.Cell(12).Value = orders[index].Reason;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Order Summary Report {request.DateFrom}-{request.DateTo}.xlsx");

            }

            return Unit.Value;

        }
    }
}
