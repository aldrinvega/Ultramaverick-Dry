using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports.ExportMoveOrderHistoryReport;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports
{
    public class ExportMoveOrderHistoryReport
    {
        public class ExportMoveroderHistoryReportCommand : IRequest<Unit>
        {
            public string DateTo { get; set; }
            public string DateFrom { get; set; }
        }

        public class Handler : IRequestHandler<ExportMoveroderHistoryReportCommand, Unit>
        {
            private readonly IReportRepository _reportRepository;

            public Handler(IReportRepository reportRepository)
            {
                _reportRepository = reportRepository;
            }

            public async Task<Unit> Handle(ExportMoveroderHistoryReportCommand request, CancellationToken cancellationToken)
            {
                var orders = await _reportRepository.MoveOrderReport(request.DateFrom, request.DateTo);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"MoveOrder History Report");

                    var headers = new List<string>
                    {
                        "Move Order ID", 
                        "Customer Code", 
                        "Customer Name", 
                        "Item Code",
                        "Item Description", 
                        "Category", 
                        "Uom", 
                        "Quantity",
                        "Transaction Type",
                        "Move Order Date",
                        "Move Order By",
                        "Employee ID",
                        "Employee Name",
                        "Status",
                        "Transacted Date",
                        "Transacted By"
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

                        row.Cell(1).Value = orders[index - 1].MoveOrderId;
                        row.Cell(2).Value = orders[index - 1].CustomerCode;
                        row.Cell(3).Value = orders[index - 1].CustomerName;
                        row.Cell(4).Value = orders[index - 1].ItemCode;
                        row.Cell(5).Value = orders[index - 1].ItemDescription;
                        row.Cell(6).Value = orders[index - 1].Category;
                        row.Cell(7).Value = orders[index - 1].Uom;
                        row.Cell(8).Value = orders[index - 1].Quantity;
                        row.Cell(9).Value = orders[index - 1].TransactionType;
                        row.Cell(10).Value = orders[index - 1].MoveOrderDate;
                        row.Cell(11).Value = orders[index - 1].MoveOrderBy;
                        row.Cell(12).Value = orders[index - 1].EmployeeId;
                        row.Cell(13).Value = orders[index - 1].EmployeeName;
                        row.Cell(14).Value = orders[index - 1].Status;
                        row.Cell(15).Value = orders[index - 1].TransactedDate;
                        row.Cell(16).Value = orders[index - 1].TransactedBy;
                        
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"MoveOrderHistoryReports {request.DateFrom} - {request.DateTo}.xlsx");
                }

                return Unit.Value;
            }
        }
    }
}

[Route("api/ExportReports")]
[ApiController]
public class ExportMoveOrderHistoryReports : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportMoveOrderHistoryReports(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportMoveOrderHistoryReports")]
    public async Task<IActionResult> Add([FromQuery] ExportMoveroderHistoryReportCommand command)
    {
        var filePath = $"MoveOrderHistoryReports {command.DateFrom} - {command.DateTo}.xlsx";
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