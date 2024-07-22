using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Inventory;

[Route("api/inventory")]
public class GetInventoryInformationByItemCode : ControllerBase
{
    private readonly IMediator _mediator;

    public GetInventoryInformationByItemCode(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{itemCode}")]
    public async Task<IActionResult> GetInventoryByItemCode(string itemCode)
    {
        var result = await _mediator.Send(new GetInventoryInformationByItemCodeRequest
        {
            ItemCode = itemCode
        });

        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    public class GetInventoryInformationByItemCodeRequest : IRequest<Result>
    {
        public string ItemCode { get; set; }
    }
    public class GetInventoryInformationByItemCodeResponse
    {
        public string ItemCode { get; set; }
        public string ItemDesccription { get; set; }
        public decimal StockOnHand { get; set; }
        public decimal AverageIssuance { get; set; }
        public decimal BufferLevel { get; set; }
    }

    public class Handler : IRequestHandler<GetInventoryInformationByItemCodeRequest, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetInventoryInformationByItemCodeRequest request, CancellationToken cancellationToken) 
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
                                      join ordering in getMoveOrderOutPerMonth
                                          on warehouse.ItemCode equals ordering.ItemCode
                                          into leftJ2
                                      from ordering in leftJ2.DefaultIfEmpty()
                                      group new
                                      {
                                          warehouse,
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
                                          ActualGood =total.Sum(
                                                            x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)/
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
                             join averageissuance in getAverageIssuance
                                 on rawmaterial.ItemCode equals averageissuance.ItemCode
                                 into leftJ10
                             from averageissuance in leftJ10.DefaultIfEmpty()
                             join reserveusage in getReserveUsage
                                 on rawmaterial.ItemCode equals reserveusage.ItemCode
                                 into leftJ11
                             from reserveusage in leftJ11.DefaultIfEmpty()
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
                                 averageissuance,
                                 reserveusage
                             }
                                 by new
                                 {
                                     rawmaterial.ItemCode,
                                     rawmaterial.ItemDescription,
                                     rawmaterial.UOM.UOM_Code,
                                     rawmaterial.ItemCategory.ItemCategoryName,
                                     rawmaterial.BufferLevel,
                                     WarehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                     ReceiptIn = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                     MoveOrderOut = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                     QcReceiving = qcreceive.QuantityOrdered != null ? qcreceive.QuantityOrdered : 0,
                                     IssueOut = issueout.Quantity != null ? issueout.Quantity : 0,
                                     SOH = SOH.SOH != null ? SOH.SOH : 0,
                                     Reserve = Reserve.Reserve != null ? Reserve.Reserve : 0,
                                     AverageIssuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                     ReserveUsage = reserveusage.Reserve != null ? reserveusage.Reserve : 0
                                 }
                into total
                             select new GetInventoryInformationByItemCodeResponse
                             {
                                 ItemCode = total.Key.ItemCode,
                                 ItemDesccription = total.Key.ItemDescription,
                                 BufferLevel = total.Key.BufferLevel,
                                 StockOnHand = total.Key.SOH,
                                 AverageIssuance = Math.Round(Convert.ToDecimal(total.Key.AverageIssuance), 2),
                             }).FirstOrDefaultAsync(x => x.ItemCode == request.ItemCode);

            return Result.Success(inventory);
        }
    }
}
