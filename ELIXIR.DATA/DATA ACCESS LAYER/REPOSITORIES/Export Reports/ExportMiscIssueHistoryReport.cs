using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.REPORT_REPOSITORY;
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

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports
{
    [Route("api/ExportReports"), ApiController]
    public class ExportMiscIssueHistoryReport : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExportMiscIssueHistoryReport(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ExportMiscIssueHistoryReports")]
        public async Task<IActionResult> Add([FromQuery] ExportMiscIssueHistoryReportCommand command)
        {
            var filePath = $"MiscellaneousIssueHistoryReport {command.DateFrom} - {command.DateTo}.xlsx";
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

        public class ExportMiscIssueHistoryReportCommand : IRequest<Unit>
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class Handler : IRequestHandler<ExportMiscIssueHistoryReportCommand, Unit>
        {
            private readonly IReportRepository _reportRepository;

            public Handler(IReportRepository reportRepository)
            {
                _reportRepository = reportRepository;
            }

            public async Task<Unit> Handle(ExportMiscIssueHistoryReportCommand request, CancellationToken cancellationToken)
            {
                var miscReceipt = await _reportRepository.MIssueReport(request.DateFrom, request.DateTo);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Misc Issue History Report");

                    var headers = new List<string>
                {
                    "Issue Id", 
                    "Customer Code", 
                    "Customer Name",
                    "Details",
                    "Item Code",
                    "Item Description", 
                    "Uom",
                    "Quantity",
                    "Expiration Date",
                    "Transacted By",
                    "Transacted Date"
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

                        row.Cell(1).Value = miscReceipt[index - 1].IssueId;
                        row.Cell(2).Value = miscReceipt[index - 1].CustomerCode;
                        row.Cell(3).Value = miscReceipt[index - 1].CustomerName;
                        row.Cell(4).Value = miscReceipt[index - 1].Details;
                        row.Cell(5).Value = miscReceipt[index - 1].ItemCode;
                        row.Cell(6).Value = miscReceipt[index - 1].ItemDescription;
                        row.Cell(7).Value = miscReceipt[index - 1].Uom;
                        row.Cell(8).Value = miscReceipt[index - 1].Quantity;
                        row.Cell(9).Value = miscReceipt[index - 1].ExpirationDate;
                        row.Cell(10).Value = miscReceipt[index - 1].TransactBy;
                        row.Cell(11).Value = miscReceipt[index - 1].TransactDate;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"MiscellaneousIssueHistoryReport {request.DateFrom} - {request.DateTo}.xlsx");
                }

                return Unit.Value;
            }
        }
    }
}
