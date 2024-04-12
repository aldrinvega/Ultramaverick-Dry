using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;
[Route("apip/ExportReports"), ApiController]
public class ExportTransactedMoveOrdersReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportTransactedMoveOrdersReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportTransactedMoveOrdersReport")]
    public async Task<IActionResult> Export([FromQuery]ExportTransactedMoveOrderReportCommand command)
    {
        var filePath = $"Transacted Move Orders Report {command.DateFrom}-{command.DateTo}.xlsx";
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
    public class ExportTransactedMoveOrderReportCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class Handler : IRequestHandler<ExportTransactedMoveOrderReportCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportTransactedMoveOrderReportCommand request, CancellationToken cancellationToken)
        {
            var transactedMoveOrderReports = await _reportRepository.TransactedMoveOrderReport(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Transacted Move Orders");

                var headers = new List<string>
                {
                        "Order No",
                        "Customer Name",
                        "Customer Code",
                        "Item Code",
                        "Item Description",
                        "Uom",
                        "Quantity",
                        "MoveOrder Date",
                        "Transacted By",
                        "Transaction Type",
                        "Transacted Date",
                        "Delivery Date"
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

                for (var index = 0; index < transactedMoveOrderReports.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = transactedMoveOrderReports[index].OrderNo;
                    row.Cell(2).Value = transactedMoveOrderReports[index].CustomerName;
                    row.Cell(3).Value = transactedMoveOrderReports[index].CustomerCode;
                    row.Cell(4).Value = transactedMoveOrderReports[index].ItemCode;
                    row.Cell(5).Value = transactedMoveOrderReports[index].ItemDescription;
                    row.Cell(6).Value = transactedMoveOrderReports[index].Uom;
                    row.Cell(7).Value = transactedMoveOrderReports[index].Quantity;
                    row.Cell(8).Value = transactedMoveOrderReports[index].MoveOrderDate;
                    row.Cell(9).Value = transactedMoveOrderReports[index].TransactedBy;
                    row.Cell(10).Value = transactedMoveOrderReports[index].TransactedBy;
                    row.Cell(11).Value = transactedMoveOrderReports[index].TransactedDate;
                    row.Cell(12).Value = transactedMoveOrderReports[index].DeliveryDate;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Transacted Move Orders Report {request.DateFrom}-{request.DateTo}.xlsx");

            }

            return Unit.Value;
        }
    }
}
