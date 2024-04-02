using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;
[Route("api/ExportsReports")]
public class ExportServedVsOrderReports : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportServedVsOrderReports(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportServedVsOrdered")]
    public async Task<IActionResult> Add([FromQuery] ExportServeVsReportsCommand command)
    {
        var filePath = $"Order vs Served Report {command.DateFrom} - {command.DateTo}.xlsx";
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
    public class ExportServeVsReportsCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
    
    public class Handler : IRequestHandler<ExportServeVsReportsCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportServeVsReportsCommand request, CancellationToken cancellationToken)
        {
            var miscReceipt = await _reportRepository.OrderVsServeReports(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Order vs Served Report");

                var headers = new List<string>
                {
                    "OrderNo",
                    "CustomerCode ",
                    "CustomerName",
                    "ItemCode",
                    "ItemDescription",
                    "Uom",
                    "Category ",
                    "QuantityOrdered",
                    "QuantityServed",
                    "Variance",
                    "Percentage"
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

                for (var index = 1; index <= miscReceipt.Count; index++)
                {
                    var row = worksheet.Row(index + 1);

                    row.Cell(1).Value = miscReceipt[index - 1].OrderNo;
                    row.Cell(2).Value = miscReceipt[index - 1].CustomerCode;
                    row.Cell(3).Value = miscReceipt[index - 1].CustomerName;
                    row.Cell(4).Value = miscReceipt[index - 1].ItemCode;
                    row.Cell(5).Value = miscReceipt[index - 1].ItemDescription;
                    row.Cell(6).Value = miscReceipt[index - 1].Uom;
                    row.Cell(7).Value = miscReceipt[index - 1].Category;
                    row.Cell(8).Value = miscReceipt[index - 1].QuantityOrdered;
                    row.Cell(9).Value = miscReceipt[index - 1].QuantityServed;
                    row.Cell(10).Value = miscReceipt[index - 1].Variance;
                    row.Cell(11).Value = miscReceipt[index - 1].Percentage;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Order vs Served Report {request.DateFrom} - {request.DateTo}.xlsx");
            }

            return Unit.Value;
        }
    }
}