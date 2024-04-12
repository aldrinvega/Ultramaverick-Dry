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
public class ExportNearlyExpireItemsReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportNearlyExpireItemsReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportNearlyExpireItemsReport")]
    public async Task<IActionResult> Export(ExportNearlyExpireItemsReportCommand command)
    {
        var filePath = $"Nearly Expire Items {command.ExpiryDays} Days.xlsx";
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
    public class ExportNearlyExpireItemsReportCommand : IRequest<Unit>
    {
        public int ExpiryDays { get; set; }
    }

    public class Handler : IRequestHandler<ExportNearlyExpireItemsReportCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportNearlyExpireItemsReportCommand request, CancellationToken cancellationToken)
        {
            var nearlyExpireItemsReport = await _reportRepository.NearlyExpireItemsReport(request.ExpiryDays);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Nearly Expire Items");

                var headers = new List<string>
                {
                        "Warehouse Id",
                        "PO Number",
                        "Item Code",
                        "Item Description",
                        "Uom",
                        "Category",
                        "Receive Date",
                        "Manufacturing Date",
                        "Quantity",
                        "Expiration Date",
                        "Expiration Days",
                        "Supplier Name",
                        "Received By"
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

                for (var index = 0; index < nearlyExpireItemsReport.Count; index++)
                {
                    var row = worksheet.Row(index + 2);

                    row.Cell(1).Value = nearlyExpireItemsReport[index].WarehouseId;
                    row.Cell(2).Value = nearlyExpireItemsReport[index].PONumber;
                    row.Cell(3).Value = nearlyExpireItemsReport[index].ItemCode;
                    row.Cell(4).Value = nearlyExpireItemsReport[index].ItemDescription;
                    row.Cell(5).Value = nearlyExpireItemsReport[index].Uom;
                    row.Cell(6).Value = nearlyExpireItemsReport[index].Category;
                    row.Cell(7).Value = nearlyExpireItemsReport[index].ReceiveDate;
                    row.Cell(8).Value = nearlyExpireItemsReport[index].ManufacturingDate;
                    row.Cell(9).Value = nearlyExpireItemsReport[index].Quantity;
                    row.Cell(10).Value = nearlyExpireItemsReport[index].ExpirationDate;
                    row.Cell(11).Value = nearlyExpireItemsReport[index].ExpirationDays;
                    row.Cell(12).Value = nearlyExpireItemsReport[index].SupplierName;
                    row.Cell(13).Value = nearlyExpireItemsReport[index].ReceivedBy;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Nearly Expire Items {request.ExpiryDays} Days.xlsx");

            }

            return Unit.Value;
        }
    }
}
