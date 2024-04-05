using System;
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
public class ExportMoveOrderHistoryReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportMoveOrderHistoryReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportMoveOrderHistoryReport")]
    public async Task<IActionResult> Export(ExportMoveOrderHistoryCommand command)
    {
        var filePath = $"Move Order History Report {command.DateFrom}-{command.DateTo}.xlsx";
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

    public class ExportMoveOrderHistoryCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class Handler : IRequestHandler<ExportMoveOrderHistoryCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportMoveOrderHistoryCommand request, CancellationToken cancellationToken)
        {
            var moveOrderHistoryReports = await _reportRepository.MoveOrderReport(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Move Order History Report");

                var headers = new List<string>
                { 
                        "MoveOrderId",
                        "CustomerCode",
                        "CustomerName",
                        "ItemCode",
                        "ItemDescription",
                        "Uom",
                        "Category",
                        "Quantity",
                        "ExpirationDate",
                        "TransactionType",
                        "MoveOrderBy",
                        "MoveOrderDate",
                        "TransactedBy",
                        "TransactedDate",
                        "EmployeeId",
                        "EmployeeName",
                        "Status"
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

                for (var index = 0; index < moveOrderHistoryReports.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = moveOrderHistoryReports[index].WarehouseId;
                    row.Cell(2).Value = moveOrderHistoryReports[index].CustomerCode;
                    row.Cell(3).Value = moveOrderHistoryReports[index].CustomerName;
                    row.Cell(4).Value = moveOrderHistoryReports[index].ItemCode;
                    row.Cell(5).Value = moveOrderHistoryReports[index].Uom;
                    row.Cell(6).Value = moveOrderHistoryReports[index].Category;
                    row.Cell(7).Value = moveOrderHistoryReports[index].Quantity;
                    row.Cell(8).Value = moveOrderHistoryReports[index].ExpirationDate;
                    row.Cell(9).Value = moveOrderHistoryReports[index].TransactionType;
                    row.Cell(10).Value = moveOrderHistoryReports[index].MoveOrderBy;
                    row.Cell(11).Value = moveOrderHistoryReports[index].MoveOrderDate;
                    row.Cell(12).Value = moveOrderHistoryReports[index].TransactedBy;
                    row.Cell(13).Value = moveOrderHistoryReports[index].TransactedDate;
                    row.Cell(14).Value = moveOrderHistoryReports[index].EmployeeId;
                    row.Cell(15).Value = moveOrderHistoryReports[index].EmployeeName;
                    row.Cell(16).Value = moveOrderHistoryReports[index].Status;

                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Move Order History Report {request.DateFrom}-{request.DateTo}.xlsx");

            }

            return Unit.Value;
        }
    }
}
