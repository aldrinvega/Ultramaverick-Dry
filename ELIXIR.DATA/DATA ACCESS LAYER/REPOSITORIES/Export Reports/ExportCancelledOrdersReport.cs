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
public class ExportCancelledOrdersReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportCancelledOrdersReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportCancelledOrdersReport")]
    public async Task<IActionResult> Export([FromQuery]ExportCancelledOrderedReportsCommand command)
    {
        var filePath = $"Cancelled Orders {command.DateFrom}-{command.DateTo}.xlsx";
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
    public class ExportCancelledOrderedReportsCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class Handler : IRequestHandler<ExportCancelledOrderedReportsCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportCancelledOrderedReportsCommand request, CancellationToken cancellationToken)
        {
            var cancelledOrders = await _reportRepository.CancelledOrderedReports(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Cancelled Orders");

                var headers = new List<string>
                 {
                    "OrderId",
                    "DateNeeded",
                    "DateOrdered",
                    "CustomerCode",
                    "CustomerName",
                    "ItemCode",
                    "ItemDescription",
                    "QuantityOrdered",
                    "CancelledDate",
                    "CancelledBy",
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

                for (var index = 0; index < cancelledOrders.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = cancelledOrders[index].OrderId;
                    row.Cell(2).Value = cancelledOrders[index].DateNeeded;
                    row.Cell(3).Value = cancelledOrders[index].DateOrdered;
                    row.Cell(4).Value = cancelledOrders[index].CustomerCode;
                    row.Cell(5).Value = cancelledOrders[index].CustomerName;
                    row.Cell(6).Value = cancelledOrders[index].ItemCode;
                    row.Cell(7).Value = cancelledOrders[index].ItemDescription;
                    row.Cell(8).Value = cancelledOrders[index].QuantityOrdered;
                    row.Cell(9).Value = cancelledOrders[index].CancelledDate;
                    row.Cell(10).Value = cancelledOrders[index].CancelledBy;
                    row.Cell(10).Value = cancelledOrders[index].Reason;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Cancelled Orders {request.DateFrom}-{request.DateTo}.xlsx");

            }

            return Unit.Value;
        }
    }
}
