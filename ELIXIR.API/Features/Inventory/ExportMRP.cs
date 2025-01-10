using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Inventory;

[Route("api/export-report"), ApiController]
public class ExportMRP : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportMRP(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("mrp")]
    public async Task<IActionResult> Export()
    {
        var query = new ExportMRPRequest();
        var filePath = $"Ultra Maverick Dry MRP.xlsx";
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

    public class ExportMRPRequest : IRequest<Unit> {}

    public class Handler : IRequestHandler<ExportMRPRequest, Unit>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ExportMRPRequest request, CancellationToken cancellationToken)
        {
            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);


            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode
                }).Select(x => new PoSummaryInventory
                {
                    ItemCode = x.Key.ItemCode,
                    UnitPrice = x.Sum(x => x.UnitPrice),
                    Ordered = x.Sum(x => x.Ordered),
                    TotalPrice = x.Average(x => x.UnitPrice)
                });

            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new WarehouseInventory
                {
                    ItemCode = x.Key.ItemCode,
                    ActualGood = x.Sum(x => x.ActualGood)
                });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)
                });

            var getQCReceivingIn = _context.QC_Receiving.Where(x => x.IsActive == true)
                .Where(x => x.ExpiryIsApprove == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.Actual_Delivered)
                });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "MiscellaneousReceipt")
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });

            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });

            var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.ActualGood
                }).Select(x => new WarehouseInventory
                {
                    ItemCode = x.Key.ItemCode,
                    ActualGood = x.Sum(x => x.ActualGood)
                });

            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true & x.IsCancelledOrder == null)
                .Where(x => x.PreparedDate != null)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new OrderingInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.AllocatedQuantity ?? (int)x.QuantityOrdered)
                });

            var getTransformationReserve = _context.Transformation_Request.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new OrderingInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.Quantity)
                });

            var getSOH = (from warehouse in getWarehouseIn
                          join issue in getIssueOut
                              on warehouse.ItemCode equals issue.ItemCode
                              into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()
                          join moveorder in getMoveOrderOut
                              on warehouse.ItemCode equals moveorder.ItemCode
                              into leftJ3
                          from moveorder in leftJ3.DefaultIfEmpty()
                          group new
                          {
                              warehouse,
                              moveorder,
                              issue
                          }
                              by new
                              {
                                  warehouse.ItemCode
                              }
                into total
                          select new SOHInventory
                          {
                              ItemCode = total.Key.ItemCode,
                              SOH = (total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood)) -
                                    (total.Sum(x => x.moveorder.QuantityOrdered == null ? 0 : x.moveorder.QuantityOrdered) +
                                      total.Sum(x => x.issue.Quantity == null ? 0 : x.issue.Quantity))
                          });

            ///try mong alisin and sum sa getorderingReservesataass kasi by Item code naman sila
            var getReserve = (from warehouse in getWarehouseStock
                              join ordering in getOrderingReserve
                                  on warehouse.ItemCode equals ordering.ItemCode
                                  into leftJ1
                              from ordering in leftJ1.DefaultIfEmpty()
                              group new
                              {
                                  warehouse,
                                  ordering
                              }
                                  by new
                                  {
                                      warehouse.ItemCode,
                                      ordering.QuantityOrdered
                                  }
                into total
                              select new ReserveInventory
                              {
                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                            total.Key.QuantityOrdered
                              });

            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getQCReceivingIn
                                      on posummary.ItemCode equals receive.ItemCode
                                      into leftJ
                                  from receive in leftJ.DefaultIfEmpty()
                                  group new
                                  {
                                      posummary,
                                      receive
                                  }
                                      by new
                                      {
                                          posummary.ItemCode,
                                          posummary.Ordered,
                                          receive.QuantityOrdered
                                      }
                into total
                                  select new PoSummaryInventory
                                  {
                                      ItemCode = total.Key.ItemCode,
                                      Ordered = (total.Key.Ordered == null ? 0 : total.Key.Ordered) -
                                                (total.Key.QuantityOrdered == null ? 0 : total.Key.QuantityOrdered)
                                  });

            var getTransformOutPerMonth = _context.Transformation_Preparation
                .Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                .Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new TransformationInventory
                {
                    ItemCode = x.Key.ItemCode,
                    WeighingScale = x.Sum(x => x.WeighingScale)
                });

            var getMoveOrderOutPerMonth = _context.MoveOrders
                .Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsPrepared == true)
                .GroupBy(x => new
                {
                    x.ItemCode,
                }).Select(x => new MoveOrderInventory
                {
                    ItemCode = x.Key.ItemCode,
                    QuantityOrdered = x.Sum(x => x.QuantityOrdered)
                });

            var getAverageIssuance = (from warehouse in getWarehouseStock
                                      join transformation in getTransformOutPerMonth
                                          on warehouse.ItemCode equals transformation.ItemCode
                                          into leftJ1
                                      from transformation in leftJ1.DefaultIfEmpty()
                                      join ordering in getMoveOrderOutPerMonth
                                          on warehouse.ItemCode equals ordering.ItemCode
                                          into leftJ2
                                      from ordering in leftJ2.DefaultIfEmpty()
                                      group new
                                      {
                                          warehouse,
                                          transformation,
                                          ordering
                                      }
                                          by new
                                          {
                                              warehouse.ItemCode
                                          }
                into total
                                      select new WarehouseInventory
                                      {
                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x =>
                                                            x.transformation.WeighingScale == null ? 0 : x.transformation.WeighingScale) +
                                                        total.Sum(
                                                            x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)) /
                                                       30
                                      });


            var getReserveUsage = (from warehouse in getWarehouseStock
                                   join ordering in getOrderingReserve
                                       on warehouse.ItemCode equals ordering.ItemCode
                                       into leftJ
                                   from ordering in leftJ.DefaultIfEmpty()
                                   group new
                                   {
                                       warehouse,
                                       ordering
                                   }
                                       by new
                                       {
                                           warehouse.ItemCode,
                                           ordering.QuantityOrdered
                                       }
                into total
                                   select new ReserveInventory
                                   {
                                       ItemCode = total.Key.ItemCode,
                                       Reserve = total.Key.QuantityOrdered == null ? 0 : total.Key.QuantityOrdered
                                   });

            var getTransformTo = _context.WarehouseReceived.Where(x => x.IsActive == true)
                .Where(x => x.TransactionType == "Transformation")
                .GroupBy(x => new

                {
                    x.ItemCode,
                }).Select(x => new ReceiptInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.ActualGood)
                });

            var individualDifferences = from wr in _context.WarehouseReceived
                                        join mo in _context.MoveOrders
                                            on wr.Id equals mo.WarehouseId
                                            into moveOrders
                                        from mo in moveOrders.DefaultIfEmpty()
                                        where wr.IsActive && wr.IsWarehouseReceive
                                        select new
                                        {
                                            wr.ItemCode,
                                            wr.ActualGood,
                                            QuantityOrdered = mo != null ? mo.QuantityOrdered : 0,
                                            CostByWarehouse = wr.UnitCost * (wr.ActualGood - (mo != null ? mo.QuantityOrdered : 0))
                                        };


            // Calculate the sum of differences per ItemCode
            var totalDifferences = individualDifferences
                .GroupBy(id => id.ItemCode)
                .Select(g => new
                {
                    ItemCode = g.Key,
                    TotalDifference = g.Sum(id => id.CostByWarehouse)
                });

            // Calculate the average UnitCost per ItemCode
            var averageUnitCosts = individualDifferences
                .GroupBy(id => id.ItemCode)
                .Select(g => new
                {
                    ItemCode = g.Key,
                    AvgUnitCost = g.Average(id =>
                        (id.ActualGood - id.QuantityOrdered) == 0
                            ? 0
                            : id.CostByWarehouse / (id.ActualGood - id.QuantityOrdered))
                });

            // Combine the results
            var finalResult = from id in individualDifferences
                              join td in totalDifferences
                                  on id.ItemCode equals td.ItemCode
                              join auc in averageUnitCosts
                                  on id.ItemCode equals auc.ItemCode
                              select new
                              {
                                  id.ItemCode,
                                  id.ActualGood,
                                  id.QuantityOrdered,
                                  Difference = id.CostByWarehouse,
                                  td.TotalDifference,
                                  auc.AvgUnitCost
                              };


            var inventory = await (from rawmaterial in _context.RawMaterials
                             join posummary in getPoSummary
                                 on rawmaterial.ItemCode equals posummary.ItemCode
                                 into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()
                             join warehouse in getWarehouseIn
                                 on rawmaterial.ItemCode equals warehouse.ItemCode
                                 into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()
                             join moveorders in getMoveOrderOut
                                 on rawmaterial.ItemCode equals moveorders.ItemCode
                                 into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()
                             join qcreceive in getQCReceivingIn
                                 on rawmaterial.ItemCode equals qcreceive.ItemCode
                                 into leftJ4
                             from qcreceive in leftJ4.DefaultIfEmpty()
                             join receiptin in getReceiptIn
                                 on rawmaterial.ItemCode equals receiptin.ItemCode
                                 into leftJ5
                             from receiptin in leftJ5.DefaultIfEmpty()
                             join issueout in getIssueOut
                                 on rawmaterial.ItemCode equals issueout.ItemCode
                                 into leftJ6
                             from issueout in leftJ6.DefaultIfEmpty()
                             join SOH in getSOH
                                 on rawmaterial.ItemCode equals SOH.ItemCode
                                 into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()
                             join Reserve in getReserve
                                 on rawmaterial.ItemCode equals Reserve.ItemCode
                                 into leftJ8
                             from Reserve in leftJ8.DefaultIfEmpty()
                             join suggestedpo in getSuggestedPo
                                 on rawmaterial.ItemCode equals suggestedpo.ItemCode
                                 into leftJ9
                             from suggestedpo in leftJ9.DefaultIfEmpty()
                             join averageissuance in getAverageIssuance
                                 on rawmaterial.ItemCode equals averageissuance.ItemCode
                                 into leftJ10
                             from averageissuance in leftJ10.DefaultIfEmpty()
                             join reserveusage in getReserveUsage
                                 on rawmaterial.ItemCode equals reserveusage.ItemCode
                                 into leftJ11
                             from reserveusage in leftJ11.DefaultIfEmpty()
                             join transformto in getTransformTo
                                 on rawmaterial.ItemCode equals transformto.ItemCode
                                 into leftJ12
                             from transformto in leftJ12.DefaultIfEmpty()
                             join transformfrom in getTransformation
                                 on rawmaterial.ItemCode equals transformfrom.ItemCode
                                 into leftJ13
                             from transformfrom in leftJ13.DefaultIfEmpty()
                             join avgUnitCost in finalResult
                                 on rawmaterial.ItemCode equals avgUnitCost.ItemCode
                                 into avgCostJoin
                             from avgUnitCost in avgCostJoin.DefaultIfEmpty()
                             group new
                             {
                                 posummary,
                                 warehouse,
                                 moveorders,
                                 qcreceive,
                                 receiptin,
                                 issueout,
                                 SOH,
                                 Reserve,
                                 suggestedpo,
                                 averageissuance,
                                 reserveusage,
                                 transformto,
                                 transformfrom,
                                 avgUnitCost
                             }
                                 by new
                                 {
                                     rawmaterial.ItemCode,
                                     rawmaterial.ItemDescription,
                                     rawmaterial.UOM.UOM_Code,
                                     rawmaterial.ItemCategory.ItemCategoryName,
                                     rawmaterial.BufferLevel,
                                     SuggestedPo = suggestedpo.Ordered != null ? suggestedpo.Ordered : 0,
                                     WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                     ReceiptIn = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                     MoveOrderOut = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                     QcReceiving = qcreceive.QuantityOrdered != null ? qcreceive.QuantityOrdered : 0,
                                     IssueOut = issueout.Quantity != null ? issueout.Quantity : 0,
                                     SOH = SOH.SOH != null ? SOH.SOH : 0,
                                     Reserve = Reserve.Reserve != null ? Reserve.Reserve : 0,
                                     AverageIssuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                     ReserveUsage = reserveusage.Reserve != null ? reserveusage.Reserve : 0,
                                     TransformFrom = transformfrom.WeighingScale != null ? transformfrom.WeighingScale : 0,
                                     TransformTo = transformto.Quantity != null ? transformto.Quantity : 0,
                                     AvgUnitCost = avgUnitCost.AvgUnitCost != null ? avgUnitCost.AvgUnitCost : 0,
                                     TotalDifference = avgUnitCost.TotalDifference != null ? avgUnitCost.TotalDifference : 0
                                 }
                into total
                             select new MRPDto
                             {
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UOM_Code,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 ReceiveIn = total.Key.QcReceiving,
                                 MoveOrderOut = total.Key.MoveOrderOut,
                                 ReceiptIn = total.Key.ReceiptIn,
                                 IssueOut = total.Key.IssueOut,
                                 WeightedAverageUnitCost = Math.Round(Convert.ToDecimal(total.Key.AvgUnitCost), 2),
                                 TotalCost = Math.Round(Convert.ToDecimal(total.Key.TotalDifference), 2),
                                 SOH = total.Key.SOH,
                                 Reserve = total.Key.Reserve - total.Key.IssueOut,
                                 SuggestedPo = total.Key.SuggestedPo,
                                 AverageIssuance = Math.Round(Convert.ToDecimal(total.Key.AverageIssuance), 2),
                                 DaysLevel = Math.Round(
                                     Convert.ToDecimal(total.Key.Reserve /
                                                       (total.Key.AverageIssuance != 0 ? total.Key.AverageIssuance : 1)), 2),
                                 ReserveUsage = total.Key.ReserveUsage
                             })
                             .OrderBy(x => x.ItemCode)
                             .ToListAsync();


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"Ultra Maverick Dry MRP");

                var headers = new List<string>
                {
                    "",
                    "Item Code",
                    "Description",
                    "Category",
                    "UOM",
                    "Weighted Average Unit Cost",
                    "Total Cost",
                    "SOH",
                    "Reserve",
                    "Buffer Level",
                    "Receive In",
                    "Receipt In",
                    "Move Order Out",
                    "Issue Out",
                    "Suggested PO",
                    "Average Issuance",
                    "Days Level",
                    "Reserve Usage",
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

                for (var index = 1; index <= inventory.Count; index++)
                {
                    var row = worksheet.Row(index + 1);

                    row.Cell(2).Value = inventory[index - 1].ItemCode;
                    row.Cell(3).Value = inventory[index - 1].ItemDescription;
                    row.Cell(4).Value = inventory[index - 1].ItemCategory;
                    row.Cell(5).Value = inventory[index - 1].Uom;
                    row.Cell(6).Value = inventory[index - 1].WeightedAverageUnitCost;
                    row.Cell(7).Value = inventory[index - 1].TotalCost;
                    row.Cell(8).Value = inventory[index - 1].SOH;
                    row.Cell(9).Value = inventory[index - 1].Reserve;
                    row.Cell(10).Value = inventory[index - 1].BufferLevel;
                    row.Cell(11).Value = inventory[index - 1].ReceiveIn;
                    row.Cell(12).Value = inventory[index - 1].ReceiptIn;
                    row.Cell(13).Value = inventory[index - 1].MoveOrderOut;
                    row.Cell(14).Value = inventory[index - 1].IssueOut;
                    row.Cell(15).Value = inventory[index - 1].SuggestedPo;
                    row.Cell(16).Value = inventory[index - 1].AverageIssuance;
                    row.Cell(17).Value = inventory[index - 1].DaysLevel;
                    row.Cell(18).Value = inventory[index - 1].ReserveUsage;

                    if (inventory[index - 1].BufferLevel >= inventory[index - 1].SOH)
                    {
                        
                        var hexColor = XLColor.FromHtml("#D9D9D9");
                        var rangeToFormat = worksheet.Range(row.FirstCell(), row.Cell(18));
                        rangeToFormat.Style.Fill.BackgroundColor = hexColor;
                        row.Cell(1).Value = "⚠️";
                    }

                }
                worksheet.Columns().AdjustToContents();
                workbook.SaveAs($"Ultra Maverick Dry MRP.xlsx");
            }

            return Unit.Value;
        }
    }
}
