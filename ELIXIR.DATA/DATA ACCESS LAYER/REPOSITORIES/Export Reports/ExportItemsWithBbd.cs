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
public class ExportItemsWithBbd : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportItemsWithBbd(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ExportItemsWithBbd")]
    public async Task<IActionResult> Add()
    {
        var filePath = $"Items.xlsx";
        try
        {
            var command = new ExportItemsWithBbsCommand();
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

    public class ExportItemsWithBbsCommand : IRequest<Unit>{}

    public class Handler : IRequestHandler<ExportItemsWithBbsCommand, Unit>
    {
        private readonly IReportRepository _reportRepository;

        public Handler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Unit> Handle(ExportItemsWithBbsCommand request, CancellationToken cancellationToken)
        {
            var rawMaterials = await _reportRepository.ItemswithBBDReport();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Items");

                var headers = new List<string>
                    {
                        "Warehouse Id",
                        "Item Code",
                        "Item Description",
                        "UOM",
                        "BBD"
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

                for (var index = 1; index <= rawMaterials.Count; index++)
                {
                    var row = worksheet.Row(index + 1);

                    row.Cell(1).Value = rawMaterials[index - 1].WarehouseId;
                    row.Cell(2).Value = rawMaterials[index - 1].ItemCode;
                    row.Cell(3).Value = rawMaterials[index - 1].ItemDescription;
                    row.Cell(4).Value = rawMaterials[index - 1].UOM;
                    row.Cell(5).Value = rawMaterials[index - 1].BBD ?? "-";
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs("Items.xlsx");
            }

            return Unit.Value;
        }
    }
}
