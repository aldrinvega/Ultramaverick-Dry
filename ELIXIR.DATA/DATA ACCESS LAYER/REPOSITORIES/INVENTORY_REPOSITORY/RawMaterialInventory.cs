using ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class RawMaterialInventory : IRawMaterialInventory
    {
        private readonly StoreContext _context;
        public RawMaterialInventory(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<RawmaterialInventory>> GetAllAvailbleInRawmaterialInventory()
        {

            return await _context.WarehouseReceived
                                                        .GroupBy(x => new
                                                        {
                                                            x.ItemCode,
                                                            x.ItemDescription,
                                                            x.LotCategory,
                                                            x.Uom,
                                                            x.IsWarehouseReceive,

                                                        })
                                                         .Select(inventory => new RawmaterialInventory
                                                         {
                                                             ItemCode = inventory.Key.ItemCode,
                                                             ItemDescription = inventory.Key.ItemDescription,
                                                             LotCategory = inventory.Key.LotCategory,
                                                             Uom = inventory.Key.Uom,
                                                             SOH = inventory.Sum(x => x.ActualGood),
                                                             ReceiveIn = inventory.Sum(x => x.ActualGood),
                                                             IsWarehouseReceived = inventory.Key.IsWarehouseReceive


                                                         })
                                                          .OrderBy(x => x.ItemCode)
                                                          .Where(x => x.IsWarehouseReceived == true)
                                                          .ToListAsync();

        }

        public Task<PoSummaryInventory> GetPOSummary()
        {
            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
               .GroupBy(x => new
               {
                   x.ItemCode,

               }).Select(x => new PoSummaryInventory
               {
                   ItemCode = x.Key.ItemCode,
                   UnitPrice = x.Sum(x => x.UnitPrice)

               });

            return (Task<PoSummaryInventory>)getPoSummary;
             
        }


        public async Task<IReadOnlyList<MRPDto>> GetAllItemForInventory()
        {

            var getPoSummary = _context.POSummary.Where(x => x.IsActive == true)
              .GroupBy(x => new
              {
                  x.ItemCode,

              }).Select(x => new PoSummaryInventory
              {
                  ItemCode = x.Key.ItemCode,
                  UnitPrice = x.Sum(x => x.UnitPrice)

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
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new MoveOrderInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                   });

            var getQCReceivingIn = (from posummary in _context.POSummary
                                    where posummary.IsActive == true
                                    join receive in _context.QC_Receiving on posummary.Id equals receive.PO_Summary_Id into leftJ
                                    from receive in leftJ.DefaultIfEmpty()

                                    group receive by new
                                    {
                                        posummary.Id,
                                        posummary.ItemCode,
                                        receive.IsActive
                                      
                                    } into total

                                    where total.Key.IsActive == true

                                    select new QcReceivingInventory

                                    {
                                        Id = total.Key.Id,
                                        ItemCode = total.Key.ItemCode,
                                        QcReceive = total.Sum(x => x.Actual_Delivered)
                                    });

            var getReceiptIn = _context.MiscellaneousReceipts.Where(x => x.IsActive == true)
                   .GroupBy(x => new
                   {
                       x.ItemCode,

                   }).Select(x => new ReceiptInventory
                   {
                       ItemCode = x.Key.ItemCode,
                       Quantity = x.Sum(x => x.Quantity)
                   });

            var getIssueOut = _context.MiscellaneousIssues.Where(x => x.IsActive == true)
                .GroupBy(x => new
                {
                    x.ItemCode,

                }).Select(x => new IssueInventory
                {
                    ItemCode = x.Key.ItemCode,
                    Quantity = x.Sum(x => x.Quantity)
                });

            var getTransformation = _context.Transformation_Preparation.Where(x => x.IsActive == true)
                                                                       .Where(x => x.IsMixed == true)
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

             }).Select(x => new WarehouseInventory
             {
                 ItemCode = x.Key.ItemCode,
                 ActualGood = x.Sum(x => x.ActualGood)
             });

            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.PreparedDate != null)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.QuantityOrdered)
           });

            var getTransformationReserve = _context.Transformation_Request.Where(x => x.IsActive == true)
                                                                          .Where(x => x.IsPrepared == true)
           .GroupBy(x => new
           {
               x.ItemCode,

           }).Select(x => new OrderingInventory
           {
               ItemCode = x.Key.ItemCode,
               QuantityOrdered = x.Sum(x => x.Quantity)
           });

            var getSOH = (from warehouse in getWarehouseStock
                          join preparation in getTransformation
                          on warehouse.ItemCode equals preparation.ItemCode
                          into leftJ1
                          from preparation in leftJ1.DefaultIfEmpty()

                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ2
                          from issue in leftJ2.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ3
                          from moveorder in leftJ3.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ4
                          from receipt in leftJ4.DefaultIfEmpty()

                       
                          group new
                          {
                              warehouse,
                              preparation,
                              moveorder,
                              receipt,
                              issue
                          }

                          by new
                          {

                              warehouse.ItemCode

                          } into total

                          select new SOHInventory
                          {
                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) - 
                                    total.Sum(x => x.preparation.WeighingScale == null ? 0 : x.preparation.WeighingScale) -
                                    total.Sum(x => x.moveorder.QuantityOrdered == null ? 0 : x.moveorder.QuantityOrdered) -
                                    total.Sum(x => x.issue.Quantity == null ? 0 : x.issue.Quantity)
                          });


            var getReserve = (from warehouse in getWarehouseStock
                              join request in getTransformationReserve
                              on warehouse.ItemCode equals request.ItemCode
                              into leftJ1
                              from request in leftJ1.DefaultIfEmpty()

                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ2
                              from ordering in leftJ2.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering
                              }
                              by new
                              {
                                  warehouse.ItemCode,

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode, 
                                  Reserve = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                           (total.Sum(x => x.request.QuantityOrdered == null ? 0 : x.request.QuantityOrdered) +
                                            total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered))
                              });

              var inventory = (from rawmaterial in _context.RawMaterials
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

                               group new { 

                                         posummary, 
                                         warehouse,
                                         moveorders,
                                         qcreceive,
                                         receiptin,
                                         issueout,
                                         SOH,
                                         Reserve
                                       }
                             by new
                             {
                                 rawmaterial.ItemCode,
                                 rawmaterial.ItemDescription,
                                 rawmaterial.UOM.UOM_Code,
                                 rawmaterial.ItemCategory.ItemCategoryName,
                                 rawmaterial.BufferLevel
                    
                             } into total

                             select new MRPDto
                             {
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UOM_Code,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Sum(x => x.posummary.UnitPrice == null ? 0 : x.posummary.UnitPrice),
                                 ReceiveIn = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) +
                                             total.Sum(x => x.receiptin.Quantity == null ? 0 : x.receiptin.Quantity),
                                 MoveOrderOut = total.Sum(x => x.moveorders.QuantityOrdered == null ? 0 : x.moveorders.QuantityOrdered),
                                 QCReceiving = total.Sum(x => x.qcreceive.QcReceive == null ? 0 : x.qcreceive.QcReceive),
                                 ReceiptIn = total.Sum(x => x.receiptin.Quantity == null ? 0 : x.receiptin.Quantity),
                                 IssueOut = total.Sum(x => x.issueout.Quantity == null ? 0 : x.issueout.Quantity),
                                 TotalPrice = total.Average(x => x.posummary.UnitPrice == null ? 0 : x.posummary.UnitPrice),
                                 SOH = total.Sum(x => x.SOH.SOH == null ? 0 : x.SOH.SOH) -                               
                                       total.Sum(x => x.issueout.Quantity == null ? 0 : x.issueout.Quantity),
                                 Reserve = total.Sum(x => x.Reserve.Reserve == null ? 0 : x.Reserve.Reserve) -
                                           total.Sum(x => x.issueout.Quantity == null ? 0 : x.issueout.Quantity)
                             });


            return await inventory.ToListAsync();

        }

       
    }
}
