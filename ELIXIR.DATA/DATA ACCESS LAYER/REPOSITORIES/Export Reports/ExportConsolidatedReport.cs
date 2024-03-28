using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;

[Route("api/ExportReports")]
[ApiController]
public class Export : ControllerBase
{
    private readonly IMediator _mediator;

    public Export(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportConsolidatedReports")]
    public async Task<IActionResult> Add([FromQuery] ExportConsolidatedReportQuery command)
    {
        var filePath = $"ConsolidatedReports {command.DateFrom} - {command.DateTo}.xlsx";
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
}

public class ExportConsolidatedReportQuery : IRequest<Unit>
{
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
}

public class Handler : IRequestHandler<ExportConsolidatedReportQuery, Unit>
{
    private readonly IReportRepository _reportRepository;

    public Handler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Unit> Handle(ExportConsolidatedReportQuery request, CancellationToken cancellationToken)
    {
        var orders =
            await _reportRepository.ConsolidatedReport(request.DateFrom, request.DateTo);

      

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add($"Consolidated Report");

            var headers = new List<string>
            {
                "Transaction Date", "Transaction Id", "Warehouse Id", "Item Code",
                "Item Description", "Uom", "Quantity", "Unit Price", "Amount", "Transaction Type", "Status",
                "Source/Move Order", "Reason", "Reference", "Encoded", "Company Code",
                "Company", "Department Code", "Department", "Location Code", "Location", "Account Title Code",
                "Account Title", "Category", "Details"
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

                row.Cell(1).Value = orders[index - 1].TransactDate;
                row.Cell(2).Value = orders[index - 1].Id;
                row.Cell(3).Value = orders[index - 1].WarehouseId;
                row.Cell(4).Value = orders[index - 1].ItemCode;
                row.Cell(5).Value = orders[index - 1].ItemDescription;
                row.Cell(6).Value = orders[index - 1].UOM;
                if (orders[index - 1].TransactionType == "Move Order" || orders[index - 1].TransactionType == "Miscellaneous Issue")
                {
                    row.Cell(7).Value = "-" + orders[index - 1].Quantity;
                }
                else
                {
                    row.Cell(7).Value = orders[index - 1].Quantity;
                }
                row.Cell(8).Value = orders[index - 1].UnitPrice;
                row.Cell(9).Value = orders[index - 1].Amount;
                row.Cell(10).Value = orders[index - 1].TransactionType;
                row.Cell(11).Value = orders[index - 1].Status;
                row.Cell(12).Value = orders[index - 1].Source;
                row.Cell(13).Value = orders[index - 1].Reason;
                row.Cell(14).Value = orders[index - 1].Reference;
                row.Cell(15).Value = orders[index - 1].Encoded;
                row.Cell(16).Value = orders[index - 1].CompanyCode;
                row.Cell(17).Value = orders[index - 1].CompanyName;
                row.Cell(18).Value = orders[index - 1].DepartmentCode;
                row.Cell(19).Value = orders[index - 1].DepartmentName;
                row.Cell(20).Value = orders[index - 1].LocationCode;
                row.Cell(21).Value = orders[index - 1].LocationName;
                row.Cell(22).Value = orders[index - 1].AccountTitleCode;
                row.Cell(23).Value = orders[index - 1].AccountTitle;
                row.Cell(24).Value = orders[index - 1].Category;
                row.Cell(25).Value = orders[index - 1].Details;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs($"ConsolidatedReports {request.DateFrom} - {request.DateTo}.xlsx");
        }

        return Unit.Value;
    }
}