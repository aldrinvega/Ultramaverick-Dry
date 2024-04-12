using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using ELIXIR.DATA.CORE.INTERFACES.REPORT_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports;
[Route("api/ExportReports"), ApiController]

public class ExportApprovedMoveorderReport : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportApprovedMoveorderReport(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportApprovedMoveOrderReport")]
    public async Task<IActionResult> Export([FromQuery] ExportApprovedMoveOrderQuery query)
    {
        var filePath = $"Approved Move Orders {query.DateFrom}-{query.DateTo}.xlsx";
        try
        {
            await _mediator.Send(query);

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

    public class ExportApprovedMoveOrderQuery : IRequest<Unit>
    {
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }

    public class Handler : IRequestHandler<ExportApprovedMoveOrderQuery, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportApprovedMoveOrderQuery request, CancellationToken cancellationToken)
        {
            var approvedMoveOrders = await _reportRepository
                .ApprovedMoveOrderReport(request.DateFrom, request.DateTo);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Approved Move Order Report");

                var headers = new List<string>
                {
                    "MoveOrder Id",
                    "Order No",
                    "Customer Name",
                    "Customer Code",
                    "Item Code",
                    "Item Description",
                    "Transaction Type",
                    "Category",
                    "Quantity",
                    "Prepared Date",
                    "Delivery Status",
                    "Transacted By"
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

                for (var index = 1; index <= approvedMoveOrders.Count; index++)
                {
                    var row = worksheet.Row(index + 1);

                    row.Cell(1).Value = approvedMoveOrders[index - 1].MoveOrderId;
                    row.Cell(2).Value = approvedMoveOrders[index - 1].OrderNo;
                    row.Cell(3).Value = approvedMoveOrders[index - 1].CustomerName;
                    row.Cell(4).Value = approvedMoveOrders[index - 1].CustomerCode;
                    row.Cell(5).Value = approvedMoveOrders[index - 1].ItemCode;
                    row.Cell(6).Value = approvedMoveOrders[index - 1].ItemDescription;
                    row.Cell(7).Value = approvedMoveOrders[index - 1].TransactionType;
                    row.Cell(8).Value = approvedMoveOrders[index - 1].Category;
                    row.Cell(9).Value = approvedMoveOrders[index - 1].Quantity;
                    row.Cell(10).Value = approvedMoveOrders[index - 1].PreparedDate;
                    row.Cell(11).Value = approvedMoveOrders[index - 1].DeliveryStatus;
                    row.Cell(12).Value = approvedMoveOrders[index - 1].TransactedBy;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Approved Move Orders {request.DateFrom}-{request.DateTo}.xlsx");
            }

            return Unit.Value;
        }
    }
}