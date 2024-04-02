using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports
{
    public class ExportApprovedMoveOrder
    {
        public class ExportApprovedMoveOrderQuery : UserParams, IRequest<Unit>
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class Handler : IRequestHandler<ExportApprovedMoveOrderQuery, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(ExportApprovedMoveOrderQuery request, CancellationToken cancellationToken)
            {
                var orders = _context.MoveOrders
                    .Where(x => x.IsActive == true)
                    .GroupBy(x => new
                    {
                        x.OrderNo,
                        x.FarmName,
                        x.FarmCode,
                        x.FarmType,
                        x.PreparedDate,
                        x.IsApprove,
                        x.DeliveryStatus,
                        x.IsPrepared,
                        x.IsReject,
                        x.ApproveDateTempo,
                        x.IsPrint,
                        x.IsTransact,
                    }).Where(x => x.Key.IsApprove == true)
                    .Where(x => x.Key.DeliveryStatus != null)
                    .Where(x => x.Key.IsReject != true);

                if (!string.IsNullOrEmpty(request.DateFrom) && !string.IsNullOrEmpty(request.DateTo))
                {
                    var fromDate = DateTime.Parse(request.DateFrom);
                    var toDate = DateTime.Parse(request.DateTo);
                    orders = orders.Where(x =>
                        x.Key.PreparedDate >= fromDate.Date && x.Key.PreparedDate <= toDate.Date);
                }

                var result = orders.Select(x => new MoveOrderDto
                {
                    OrderNo = x.Key.OrderNo,
                    FarmName = x.Key.FarmName,
                    FarmCode = x.Key.FarmCode,
                    Category = x.Key.FarmType,
                    Quantity = x.Sum(x => x.QuantityOrdered),
                    PreparedDate = x.Key.PreparedDate.ToString(),
                    DeliveryStatus = x.Key.DeliveryStatus,
                    IsApprove = x.Key.IsApprove != null,
                    IsPrepared = x.Key.IsPrepared,
                    ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                    IsPrint = x.Key.IsPrint != null,
                    IsTransact = x.Key.IsTransact,
                }).OrderBy(x => x.PreparedDate);

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

                    var resultList = result.ToList();

                    for (var index = 1; index <= resultList.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        /*row.Cell(1).Value = resultList[index - 1].OrderNo;
                        row.Cell(2).Value = resultList[index - 1].CustomerName;
                        row.Cell(3).Value = resultList[index - 1].CustomerCode;
                        row.Cell(4).Value = resultList[index - 1].ItemCode;
                        row.Cell(5).Value = resultList[index - 1].ItemDescription;
                        row.Cell(6).Value = resultList[index - 1].Uom;
                        row.Cell(7).Value = resultList[index - 1].Quantity;
                        row.Cell(8).Value = resultList[index - 1].MoveOrderDate;
                        row.Cell(9).Value = resultList[index - 1].TransactedBy;
                        row.Cell(10).Value = resultList[index - 1].TransactionType;
                        row.Cell(11).Value = resultList[index - 1].TransactedDate;
                        row.Cell(12).Value = resultList[index - 1].DeliveryDate;*/
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"TransactedMoveOrderReports {request.DateFrom} - {request.DateTo}.xlsx");
                }

                return Unit.Value;
            }
        }
    }

    [Microsoft.AspNetCore.Components.Route("api/ExportReports")]
    [ApiController]
    public class ExportApprovedMoveOrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExportApprovedMoveOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ExportMoveOrderRepors")]
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
}