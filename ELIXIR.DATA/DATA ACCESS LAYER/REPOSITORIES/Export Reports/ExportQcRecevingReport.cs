using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;

[Route("api/ExportReports"), ApiController]

public class ExportQcRecevingReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportQcRecevingReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportQcRecevingReport")]
    public async Task<IActionResult> Export([FromQuery] ExportQcRecevingReportCommand command)
    {
        var filePath = $"QC Receiving {command.DateFrom.Trim()}-{command.DateTo.Trim()}.xlsx";
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

    public class ExportQcRecevingReportCommand : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
    public class Handler : IRequestHandler<ExportQcRecevingReportCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportQcRecevingReportCommand request, CancellationToken cancellationToken)
        {
            var qcReports = await _reportRepository.QcRecevingReport(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"QC Receiving");

                var headers = new List<string>
                    {
                        "Id",
                        "QC Date",
                        "PO Number",
                        "Item Code",
                        "Item Description",
                        "Uom",
                        "Category",
                        "Quantity",
                        "Manufacturing Date",
                        "Expiration Date",
                        "Total Reject",
                        "Supplier Name",
                        "Price",
                        "QcBy"
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

                for (var index = 0; index < qcReports.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = qcReports[index].Id;
                    row.Cell(2).Value = qcReports[index].QcDate;
                    row.Cell(3).Value = qcReports[index].PONumber;
                    row.Cell(4).Value = qcReports[index].ItemCode;
                    row.Cell(5).Value = qcReports[index].ItemDescription;
                    row.Cell(6).Value = qcReports[index].Uom;
                    row.Cell(7).Value = qcReports[index].Category;
                    row.Cell(8).Value = qcReports[index].Quantity;
                    row.Cell(9).Value = qcReports[index].ManufacturingDate;
                    row.Cell(10).Value = qcReports[index].ExpirationDate;
                    row.Cell(11).Value = qcReports[index].TotalReject;
                    row.Cell(12).Value = qcReports[index].SupplierName;
                    row.Cell(13).Value = qcReports[index].Price;
                    row.Cell(14).Value = qcReports[index].QcBy;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"QC Receiving {request.DateFrom.Trim()}-{request.DateTo.Trim()}.xlsx");

            }

            return Unit.Value;
        }
    }
}
