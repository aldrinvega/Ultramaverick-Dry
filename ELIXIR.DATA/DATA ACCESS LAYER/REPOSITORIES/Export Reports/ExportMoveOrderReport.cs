using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports
{
    public class ExportMoveOrderReport
    {
        public class ExportMoveOrderReportQuery : IRequest<Unit>
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class Handler : IRequestHandler<ExportMoveOrderReportQuery, Unit>
        {
            private readonly IReportRepository _reportRepository;

            public Handler(IReportRepository reportRepository)
            {
                _reportRepository = reportRepository;
            }

            public async Task<Unit> Handle(ExportMoveOrderReportQuery request, CancellationToken cancellationToken)
            {
                var orders =
                    await _reportRepository.TransactedMoveOrderReport(request.DateFrom, request.DateTo);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Transacted MoveOrder Report");

                    var headers = new List<string>
                    {
                        "Order No", "Customer Name", "CustomerCode", "Item Code",
                        "Item Description", "Uom", "Quantity", "Move Order Date",
                        "Transacted By", "Transaction Type", "Transacted Date", "Delivery Date"
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

                    for (var index = 1; index <= orders.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = orders[index - 1].OrderNo;
                        row.Cell(2).Value = orders[index - 1].CustomerName;
                        row.Cell(3).Value = orders[index - 1].CustomerCode;
                        row.Cell(4).Value = orders[index - 1].ItemCode;
                        row.Cell(5).Value = orders[index - 1].ItemDescription;
                        row.Cell(6).Value = orders[index - 1].Uom;
                        row.Cell(7).Value = orders[index - 1].Quantity;
                        row.Cell(8).Value = orders[index - 1].MoveOrderDate;
                        row.Cell(9).Value = orders[index - 1].TransactedBy;
                        row.Cell(10).Value = orders[index - 1].TransactionType;
                        row.Cell(11).Value = orders[index - 1].TransactedDate;
                        row.Cell(12).Value = orders[index - 1].DeliveryDate;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"TransactedMoveOrderReports {request.DateFrom} - {request.DateTo}.xlsx");
                }

                return Unit.Value;
            }
        }
    }
}

[Microsoft.AspNetCore.Components.Route("api/ExportReports")]
[ApiController]
public class ExportReports : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportReports(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportReports")]
    public async Task<IActionResult> Add([FromQuery] ExportMoveOrderReport.ExportMoveOrderReportQuery command)
    {
        var filePath = $"TransactedMoveOrderReports {command.DateFrom} - {command.DateTo}.xlsx";
        try
        {
            await _mediator.Send(command);

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            var result = File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filePath);
            System.IO.File.Delete(filePath);
            return result;
        }
        catch (System.Exception e)
        {
            return Conflict(e.Message);
        }
    }
}