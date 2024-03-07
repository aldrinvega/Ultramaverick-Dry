using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MemoryStream = System.IO.MemoryStream;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.Export_Reports
{
    [Route("api/ExportSetup")]
    [ApiController]
    public class ExportRawMaterials : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExportRawMaterials(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ExportRawMaterials")]
        public async Task<IActionResult> Add()
        {
            var query = new ExportRawMaterialsQuery();
            const string filePath = "Raw Materials.xlsx";
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

        public class ExportRawMaterialsQuery : IRequest<Unit>
        {
        }

        public class Handler : IRequestHandler<ExportRawMaterialsQuery, Unit>
        {
            private readonly IRawMaterialRepository _materialRepository;

            public Handler(IRawMaterialRepository materialRepository)
            {
                _materialRepository = materialRepository;
            }

            public async Task<Unit> Handle(ExportRawMaterialsQuery request, CancellationToken cancellationToken)
            {
                var rawMaterials = await _materialRepository.GetAllRawMaterialForExport();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Raw Materials");

                    var headers = new List<string>
                    {
                        "Id", "Item Code", "Item Description", "UOM",
                        "Buffer Level", "Date Added", "Expirable"
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

                        row.Cell(1).Value = rawMaterials[index - 1].Id;
                        row.Cell(2).Value = rawMaterials[index - 1].ItemCode;
                        row.Cell(3).Value = rawMaterials[index - 1].ItemDescription;
                        row.Cell(4).Value = rawMaterials[index - 1].Uom;
                        row.Cell(5).Value = rawMaterials[index - 1].BufferLevel;
                        row.Cell(6).Value = rawMaterials[index - 1].DateAdded;
                        row.Cell(7).Value = rawMaterials[index - 1].Expirable;
                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs("Raw Materials.xlsx");
                }

                return Unit.Value;
            }
        }
    }
}