 using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.ORDERING_REPOSITORY
{
    public class OrderingRepository : IOrdering
    {

       private readonly StoreContext _context;
        public OrderingRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<OrderDto>> GetAllListofOrders(string farms)
        {

            var totalout = _context.Transformation_Preparation.GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var totalOrders = _context.Orders.Where(x => x.PreparedDate != null)
           .GroupBy(x => new
           {
               x.ItemCode,
               x.IsActive
           }).Select(x => new OrderDto
           {
               ItemCode = x.Key.ItemCode,
               TotalOrders = x.Sum(x => x.QuantityOrdered)
           });


            var x = totalOrders;

            var totalRemaining = (from totalIn in _context.WarehouseReceived
                                  join totalOut in totalout

                                  on totalIn.Id equals totalOut.WarehouseId
                                  into leftJ
                                  from totalOut in leftJ.DefaultIfEmpty()


                                  join orderout in totalOrders on totalIn.ItemCode equals orderout.ItemCode
                                  into leftJ2
                                  from orderout in leftJ2.DefaultIfEmpty()

                                  group totalOut by new
                                  {

                                      totalIn.Id,
                                      totalIn.ItemCode,
                                      totalIn.ItemDescription,
                                      totalIn.ManufacturingDate,
                                      totalIn.Expiration,
                                      totalIn.ActualGood,
                                      totalIn.ExpirationDays,
                                      orderout.TotalOrders

                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.Id,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      ManufacturingDate = total.Key.ManufacturingDate,
                                      ExpirationDate = total.Key.Expiration,
                                      ExpirationDays = total.Key.ExpirationDays,
                                      In = total.Key.ActualGood,
                                      Out = total.Sum(x => x.Out),
                                      Remaining = total.Key.ActualGood - total.Sum(x => x.Out),
                                      TotalMoveOrder = total.Key.TotalOrders

                                  });

            var orders = (from ordering in _context.Orders
                          where ordering.FarmName == farms && ordering.PreparedDate == null && ordering.IsActive == true
                          join warehouse in totalRemaining
                          on ordering.ItemCode equals warehouse.ItemCode
                          into leftJ
                          from warehouse in leftJ.DefaultIfEmpty()

                          group warehouse by new
                          {

                              ordering.Id,
                              ordering.OrderDate,
                              ordering.DateNeeded,
                              ordering.FarmName,
                              ordering.FarmCode,
                              ordering.Category,
                              ordering.ItemCode,
                              ordering.ItemDescription,
                              ordering.Uom,
                              ordering.QuantityOrdered,
                              ordering.IsActive,
                              ordering.IsPrepared,

                          } into total

                          orderby total.Key.DateNeeded ascending

                          select new OrderDto
                          {

                              Id = total.Key.Id,
                              OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                              DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                              Farm = total.Key.FarmName,
                              FarmCode = total.Key.FarmCode,
                              Category = total.Key.Category,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              QuantityOrder = total.Key.QuantityOrdered,
                              IsActive = total.Key.IsActive,
                              IsPrepared = total.Key.IsPrepared,
                              StockOnHand = total.Sum(x => x.Remaining) - total.Sum(x => x.TotalMoveOrder),

                          });

            return await orders.ToListAsync();

        }

        public async Task<bool> EditQuantityOrder(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;


            existingOrder.QuantityOrdered = orders.QuantityOrdered;

            return true;
        }

        public async Task<bool> SchedulePreparedDate(Ordering orders)
        {

            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;  

            existingOrder.PreparedDate = orders.PreparedDate;
            existingOrder.OrderNoPKey = orders.OrderNoPKey;

            return true;
        }

        public async Task<IReadOnlyList<OrderDto>> GetAllListOfPreparedDate()
        {
            var orders = _context.Orders.Select(x => new OrderDto
            {

                Id = x.Id,

                FarmCode = x.FarmCode,
                Category = x.Category,
                QuantityOrder = x.QuantityOrdered,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                PreparedDate = x.PreparedDate.ToString(),
                IsApproved = x.IsApproved != null

            });

            return await orders.Where(x => x.IsApproved != true)
                               .ToListAsync();

        }

        public async Task<bool> ApprovePreparedDate(Ordering orders)
        {

            var order = await _context.Orders.Where(x => x.OrderNoPKey == orders.OrderNoPKey)
                                             .Where(x => x.IsActive == true)
                                              .ToListAsync();

            foreach(var items in order)
            {
                items.IsApproved = true;
                items.ApprovedDate = DateTime.Now;
                items.RejectBy = null;
                items.RejectedDate = null;
                items.Remarks = null;

            }

            return true;

        }

        public async Task<bool> RejectPreparedDate(Ordering orders)
        {

            var order = await _context.Orders.Where(x => x.OrderNoPKey == orders.OrderNoPKey)
                                             .Where(x => x.IsActive == true)
                                             .ToListAsync();

            foreach (var items in order)
            {

                items.IsReject = true;
                items.RejectBy = orders.RejectBy;
                items.IsActive = true;
                items.Remarks = orders.Remarks;
                items.RejectedDate = DateTime.Now;
                items.PreparedDate = null;
                items.OrderNoPKey = 0;
            }

            return true;
        }

        public async Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo)
        {
            var totalout = _context.Transformation_Preparation.GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var totalRemaining = (from totalIn in _context.WarehouseReceived
                                  join totalOut in totalout

                                  on totalIn.Id equals totalOut.WarehouseId
                                  into leftJ
                                  from totalOut in leftJ.DefaultIfEmpty()

                                  group totalOut by new
                                  {

                                      totalIn.Id,
                                      totalIn.ItemCode,
                                      totalIn.ItemDescription,
                                      totalIn.ManufacturingDate,
                                      totalIn.Expiration,
                                      totalIn.ActualGood,
                                      totalIn.ExpirationDays

                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.Id,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      ManufacturingDate = total.Key.ManufacturingDate,
                                      ExpirationDate = total.Key.Expiration,
                                      ExpirationDays = total.Key.ExpirationDays,
                                      In = total.Key.ActualGood,
                                      Out = total.Sum(x => x.Out),
                                      Remaining = total.Key.ActualGood - total.Sum(x => x.Out)

                                  });

            var totalOrders = _context.Orders.GroupBy(x => new
            {
                x.ItemCode,
                x.IsPrepared,
                x.IsActive
            }).Select(x => new OrderDto
            {
                ItemCode = x.Key.ItemCode,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                IsPrepared = x.Key.IsPrepared

            }).Where(x => x.IsPrepared == false);


            var orders = (from ordering in _context.Orders                  
                          where ordering.OrderDate >= DateTime.Parse(DateFrom) && ordering.OrderDate <= DateTime.Parse(DateTo)
                          join warehouse in totalRemaining
                          on ordering.ItemCode equals warehouse.ItemCode
                          into leftJ

                          from warehouse in leftJ.DefaultIfEmpty()

                          group warehouse by new
                          {

                              ordering.Id,
                              ordering.OrderDate,
                              ordering.DateNeeded,
                              ordering.FarmName,
                              ordering.FarmCode,
                              ordering.Category,
                              ordering.ItemCode,
                              ordering.ItemDescription,
                              ordering.Uom,
                              ordering.QuantityOrdered,
                              ordering.IsActive,
                              ordering.IsPrepared,
                              ordering.PreparedDate,
                              ordering.IsApproved
                             

                          } into total

                          select new OrderDto
                          {

                              Id = total.Key.Id,
                              OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                              DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                              Farm = total.Key.FarmName,
                              FarmCode = total.Key.FarmCode,
                              Category = total.Key.Category,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              QuantityOrder = total.Key.QuantityOrdered,
                              IsActive = total.Key.IsActive,
                              IsPrepared = total.Key.IsPrepared,
                              StockOnHand = total.Sum(x => x.Remaining),
                              Difference = total.Sum(x => x.Remaining) - total.Key.QuantityOrdered,
                              PreparedDate = total.Key.PreparedDate.ToString(),
                              IsApproved = total.Key.IsApproved != null

                          });

            return await orders.ToListAsync();



        }

        public async Task<bool> AddNewOrders(Ordering orders)
        {

            orders.IsActive = true;

            await _context.Orders.AddAsync(orders);

            return true;

        }

        public async Task<bool> ValidateFarmType(Ordering orders)
        {
            var farmtype = await _context.Farms.Where(x => x.FarmName == orders.FarmName)
                                               .Where(x => x.IsActive == true)
                                               .FirstOrDefaultAsync();

            if (farmtype == null)
                return false;


            return true;
            
        }

        public async Task<bool> ValidateFarmCode(Ordering orders)
        {
            var farmcode = await _context.Farms.Where(x => x.FarmCode == orders.FarmCode)
                                               .Where(x => x.IsActive == true)
                                               .FirstOrDefaultAsync();

            if (farmcode == null)
                return false;


            return true;
        }
        public async Task<bool> ValidateRawMaterial(Ordering orders)
        {
            var rawmaterial = await _context.RawMaterials.Where(x => x.ItemCode == orders.ItemCode)
                                                         .Where(x => x.IsActive == true)
                                                         .FirstOrDefaultAsync();

            if (rawmaterial == null)
                return false;

            return true;
        }

        public async Task<bool> ValidateUom(Ordering orders)
        {
            var uom = await _context.UOMS.Where(x => x.UOM_Code == orders.Uom)
                                         .Where(x => x.IsActive == true)
                                         .FirstOrDefaultAsync();

            if (uom == null)
               return false;

            return true;
        }

        public async Task<bool> ValidateExistingOrders(Ordering orders)
        {
            var validate = await _context.Orders.Where(x => x.TransactId == orders.TransactId)
                                                .Where(x => x.IsActive == true)
                                                .FirstOrDefaultAsync();

            if (validate == null)
                return true;

            return false;

        }

        public async Task<PagedList<OrderDto>> GetAllListofOrdersPagination(UserParams userParams)
        {
     
            var orders = _context.Orders.OrderBy(x => x.OrderDate)              
                                        .GroupBy(x => new
                                        {
                                            x.FarmName,
                                            x.IsActive,
                                            x.PreparedDate

                                        }).Where(x => x.Key.IsActive == true)
                                          .Where(x => x.Key.PreparedDate == null)

                                          .Select(x => new OrderDto
                                          {
                                              Farm = x.Key.FarmName,
                                              IsActive = x.Key.IsActive
                                          });
            
            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<bool> ValidateOrderAndDateNeeded(Ordering orders)
        {
            var dateNow = DateTime.Now;

            if (Convert.ToDateTime(orders.DateNeeded).Day < dateNow.Day)
                return false;

            return true;
        }

        public async Task<bool> CancelOrders(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                .Where(x => x.IsActive == true)
                                                .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsActive = false;
            existing.IsCancelBy = orders.IsCancelBy;
            existing.IsCancel = true;
            existing.CancelDate = DateTime.Now;
            existing.Remarks = orders.Remarks;

            return true;
                                               
        }

        public async Task<IReadOnlyList<OrderDto>> GetAllListOfCancelledOrders()
        {
            var cancelled = _context.Orders.Where(x => x.CancelDate != null)
                                           .Where(x => x.IsActive == false)
                                           .Where(x => x.IsCancelBy != null)

            .Select(x => new OrderDto
            {
                Id = x.Id,
                FarmCode = x.FarmCode,
                Category = x.Category,
                QuantityOrder = x.QuantityOrdered,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                PreparedDate = x.PreparedDate.ToString(),
                CancelDate = x.CancelDate.ToString(),
                CancelBy = x.IsCancelBy


            });

            return await cancelled.ToListAsync();


        }

        public async Task<bool> ReturnCancellOrdersInList(Ordering orders)
        {
            var existing = await _context.Orders.Where(x => x.Id == orders.Id)
                                                .Where(x => x.IsActive == false)
                                                .FirstOrDefaultAsync();
            if (existing == null)
                return false;

            existing.IsActive = true;
            existing.IsCancelBy = null;
            existing.IsCancel = null;
            existing.Remarks = null;
            existing.CancelDate = null;

            return true;

        }

        public async Task<IReadOnlyList<OrderDto>> DetailedListOfOrders(string farm)
        {
            var orders = _context.Orders.Select(x => new OrderDto
            {

                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                Farm = x.FarmName,
                FarmCode = x.FarmCode,
                Category = x.Category,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                QuantityOrder = x.QuantityOrdered

            });


            return await orders.Where(x => x.Farm == farm)
                               .ToListAsync();

        }

        public async Task<IReadOnlyList<OrderDto>> GetAllListForApprovalOfSchedule()
        {
            var orders = _context.Orders.GroupBy(x => new
            {
                x.OrderNoPKey,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApproved,
                x.IsActive

            }).Where(x => x.Key.IsApproved == null)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsActive == true)
               .Select(x => new OrderDto
               {
                   OrderNo = x.Key.OrderNoPKey,
                   Farm = x.Key.FarmName,
                   FarmCode = x.Key.FarmCode,
                   Category = x.Key.FarmType,
                   TotalOrders = x.Sum(x => x.QuantityOrdered),
                   OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
                   PreparedDate = x.Key.PreparedDate.ToString()

               });

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<OrderDto>> GetAllOrdersForScheduleApproval(int id)
        {
            var orders = _context.Orders.OrderBy(x => x.PreparedDate)

            .Select(x => new OrderDto
            {
                OrderNo = x.OrderNoPKey,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                Farm = x.FarmName,
                FarmCode = x.FarmCode,
                Category = x.Category,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                QuantityOrder = x.QuantityOrdered,
                IsApproved = x.IsApproved != null

            });

            return await orders.Where(x => x.OrderNo == id)
                               .Where(x => x.IsApproved == false)
                               .ToListAsync();
        }

        public async Task<int> CountOrderNoKey()
        {
            var count = await _context.Orders.Where(x => x.OrderNoPKey > 0)
                   .GroupBy(x => new
                   {
                       x.OrderNoPKey

                   }).Select(x => new
                   {
                       x.Key.OrderNoPKey

                   }).ToListAsync();

            var getCount = count.Count;

            return getCount;

        }

        public async Task<PagedList<OrderDto>> GetAllListForMoveOrderPagination(UserParams userParams)
        {

            var orders = _context.Orders
                                      .GroupBy(x => new
                                      {
                                          x.FarmName,
                                          x.IsActive,
                                          x.IsApproved,
                                          x.IsMove

                                      }).Where(x => x.Key.IsActive == true)
                                        .Where(x => x.Key.IsApproved == true)
                                        .Where(x => x.Key.IsMove == false)
                                        .Select(x => new OrderDto
                                        {
                                            Farm = x.Key.FarmName,
                                            IsActive = x.Key.IsActive,
                                            IsApproved = x.Key.IsApproved != null
                                        });

            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);


        }

        public async Task<IReadOnlyList<OrderDto>> TotalListOfApprovedPreparedDate(string farm)
        {

            var orders = _context.Orders.GroupBy(x => new
            {

                x.OrderNoPKey,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApproved,
                x.IsMove,
                x.IsReject,
                x.Remarks 

            }).Where(x => x.Key.FarmName == farm)
              .Where(x => x.Key.IsApproved == true)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsMove == false)


            .Select(x => new OrderDto
            {
                Id = x.Key.OrderNoPKey,
                Farm = x.Key.FarmName,
                FarmCode = x.Key.FarmCode,
                Category = x.Key.FarmType,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
                PreparedDate = x.Key.PreparedDate.ToString(),
                IsMove = x.Key.IsMove,
                IsReject = x.Key.IsReject != null,
                Remarks = x.Key.Remarks

            });

            return await orders.ToListAsync();

        }
        public async Task<bool> GenerateNumber(GenerateOrderNo generate)
        {
            await _context.GenerateOrderNos.AddAsync(generate);
            return true;
        }

        public async Task<bool> PrepareItemForMoveOrder(MoveOrder orders)
        {

            await _context.MoveOrders.AddAsync(orders);
            return true;

        }

        public async Task<IReadOnlyList<OrderDto>> GetAllOutOfStockByItemCodeAndOrderDate(string itemcode, string orderdate)
        {


            var totalout = _context.Transformation_Preparation.GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var totalRemaining = (from totalIn in _context.WarehouseReceived
                                  join totalOut in totalout

                                  on totalIn.Id equals totalOut.WarehouseId
                                  into leftJ
                                  from totalOut in leftJ.DefaultIfEmpty()

                                  group totalOut by new
                                  {

                                      totalIn.Id,
                                      totalIn.ItemCode,
                                      totalIn.ItemDescription,
                                      totalIn.ManufacturingDate,
                                      totalIn.Expiration,
                                      totalIn.ActualGood,
                                      totalIn.ExpirationDays

                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.Id,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      ManufacturingDate = total.Key.ManufacturingDate,
                                      ExpirationDate = total.Key.Expiration,
                                      ExpirationDays = total.Key.ExpirationDays,
                                      In = total.Key.ActualGood,
                                      Out = total.Sum(x => x.Out),
                                      Remaining = total.Key.ActualGood - total.Sum(x => x.Out)

                                  });

            var totalOrders = _context.Orders.GroupBy(x => new
            {
                x.ItemCode,
                x.IsPrepared,
                x.IsActive
            }).Select(x => new OrderDto
            {
                ItemCode = x.Key.ItemCode,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                IsPrepared = x.Key.IsPrepared

            }).Where(x => x.IsPrepared == false);


            var orders = (from ordering in _context.Orders
                          where ordering.ItemCode == itemcode
                          && ordering.OrderDate == DateTime.Parse(orderdate)
                          join warehouse in totalRemaining
                          on ordering.ItemCode equals warehouse.ItemCode
                          into leftJ

                          from warehouse in leftJ.DefaultIfEmpty()

                          group warehouse by new
                          {

                              ordering.Id,
                              ordering.OrderDate,
                              ordering.DateNeeded,
                              ordering.FarmName,
                              ordering.FarmCode,
                              ordering.Category,
                              ordering.ItemCode,
                              ordering.ItemDescription,
                              ordering.Uom,
                              ordering.QuantityOrdered,
                              ordering.IsActive,
                              ordering.IsPrepared,
                              ordering.PreparedDate,
                              ordering.IsApproved


                          } into total

                          select new OrderDto
                          {

                              Id = total.Key.Id,
                              OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                              DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                              Farm = total.Key.FarmName,
                              FarmCode = total.Key.FarmCode,
                              Category = total.Key.Category,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              QuantityOrder = total.Key.QuantityOrdered,
                              IsActive = total.Key.IsActive,
                              IsPrepared = total.Key.IsPrepared,
                              StockOnHand = total.Sum(x => x.Remaining),
                              Difference = total.Sum(x => x.Remaining) - total.Key.QuantityOrdered,
                              PreparedDate = total.Key.PreparedDate.ToString(),
                              IsApproved = total.Key.IsApproved != null

                          });


            return await orders.ToListAsync();

        

        }

        public async Task<IReadOnlyList<OrderDto>> ListOfOrdersForMoveOrder(int id)
        {
            var moveorders = _context.MoveOrders.Where(x => x.IsActive == true)
               .GroupBy(x => new
               {
                   x.OrderNo,
                   x.OrderNoPKey

            }).Select(x => new MoveOrderItem
            {
                OrderNo = x.Key.OrderNo,
                OrderPKey = x.Key.OrderNoPKey,
                QuantityPrepared = x.Sum(x => x.QuantityOrdered),

            });

            var orders = (from ordering in _context.Orders
                          where ordering.OrderNoPKey == id
                          join moveorder in moveorders
                          on ordering.Id equals moveorder.OrderPKey into leftJ

                          from moveorder in leftJ.DefaultIfEmpty()

                          group moveorder by new
                          {
                              ordering.Id,
                              ordering.OrderNoPKey,
                              ordering.OrderDate,
                              ordering.DateNeeded,
                              ordering.FarmName,
                              ordering.FarmCode,
                              ordering.Category,
                              ordering.ItemCode,
                              ordering.ItemDescription,
                              ordering.Uom,
                              ordering.QuantityOrdered,
                              ordering.IsApproved,

                          } into total


                          select new OrderDto
                          {
                              Id = total.Key.Id,
                              OrderNo = total.Key.OrderNoPKey,
                              OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                              DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                              Farm = total.Key.FarmName,
                              FarmCode = total.Key.FarmCode,
                              Category = total.Key.Category,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              QuantityOrder = total.Key.QuantityOrdered,
                              IsApproved = total.Key.IsApproved != null,
                              PreparedQuantity = total.Sum(x => x.QuantityPrepared),

                          });

            return await orders.ToListAsync();

        }

        public async Task<OrderDto> GetMoveOrderDetailsForMoveOrder(int orderid)
        {
                var orders = _context.Orders.Select(x => new OrderDto
                {

                    Id = x.Id,
                    OrderNo = x.OrderNoPKey,
                    Farm = x.FarmName,
                    FarmCode = x.FarmCode, 
                    FarmType = x.FarmType, 
                    ItemCode = x.ItemCode, 
                    ItemDescription = x.ItemDescription, 
                    Uom = x.Uom,
                    QuantityOrder = x.QuantityOrdered, 
                    Category = x.Category,
                    OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                    DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                    PreparedDate = x.PreparedDate.ToString()

                });

                return await orders.Where(x => x.Id == orderid)
                                   .FirstOrDefaultAsync();
             
        }

        public async Task<ItemStocks> GetActualItemQuantityInWarehouse(int id, string itemcode)
        {
            var totalout = _context.Transformation_Preparation.GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var totaloutMoveorder = await _context.MoveOrders.Where(x => x.WarehouseId == id)
                                                             .Where(x => x.ItemCode == itemcode)
                                                             .SumAsync(x => x.QuantityOrdered);
             
            var totalRemaining = (from totalIn in _context.WarehouseReceived
                                  where totalIn.Id == id && totalIn.ItemCode == itemcode && totalIn.IsActive == true
                                  join totalOut in totalout
                                  on totalIn.Id equals totalOut.WarehouseId
                                  into leftJ
                                  from totalOut in leftJ.DefaultIfEmpty()

                                  group totalOut by new
                                  {

                                      totalIn.Id,
                                      totalIn.ItemCode,
                                      totalIn.ItemDescription,
                                      totalIn.ManufacturingDate,
                                      totalIn.Expiration,
                                      totalIn.ActualGood,
                                      totalIn.ExpirationDays
                          
                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.Id,
                                      ItemCode = total.Key.ItemCode,
                                      ItemDescription = total.Key.ItemDescription,
                                      ManufacturingDate = total.Key.ManufacturingDate,
                                      ExpirationDate = total.Key.Expiration,
                                      ExpirationDays = total.Key.ExpirationDays,
                                      In = total.Key.ActualGood,
                                      Out = total.Sum(x => x.Out),
                                      Remaining = total.Key.ActualGood - total.Sum(x => x.Out) - totaloutMoveorder

                                  });

            return await totalRemaining.Where(x => x.Remaining != 0)
                                       .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<MoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id)
        {
            var orders = _context.MoveOrders.Select(x => new MoveOrderDto
            {
                Id = x.Id,
                OrderNo = x.OrderNo,
                BarcodeNo = x.WarehouseId,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Quantity = x.QuantityOrdered,
                Expiration = x.ExpirationDate.ToString(),
                IsActive = x.IsActive 

            });

            return await orders.Where(x => x.OrderNo == id)
                               .Where(x => x.IsActive == true)
                               .ToListAsync();
        }

        public async Task<bool> CancelMoveOrder(MoveOrder moveorder)
        {

            var existingInfo = await _context.MoveOrders.Where(x => x.Id == moveorder.Id)
                                                        .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;

            existingInfo.IsActive = false;
            existingInfo.CancelledDate = DateTime.Now;

            return true;


        }

        public async Task<bool> AddPlateNumberInMoveOrder(Ordering order)
        {

            var existing = await _context.Orders.Where(x => x.Id == order.Id)
                                                .FirstOrDefaultAsync();

            var existingMoveorder = await _context.MoveOrders.Where(x => x.OrderNoPKey == order.Id)
                                                             .ToListAsync();

            if (existing == null)
                return false;

            existing.PlateNumber = order.PlateNumber;
            existing.IsMove = true;

            foreach(var items in existingMoveorder)
            {
                items.PlateNumber = order.PlateNumber;
            }

            return true;
        }

        public async Task<bool> AddDeliveryStatus(Ordering order)
        {
            var existing = await _context.Orders.Where(x => x.Id == order.Id)
                                                .FirstOrDefaultAsync();

            var existingMoveorder = await _context.MoveOrders.Where(x => x.OrderNoPKey == order.Id)
                                                             .ToListAsync();

            if (existing == null)
                return false;


            existing.DeliveryStatus = order.DeliveryStatus;
            existing.IsMove = true;

            foreach (var items in existingMoveorder)
            {
                items.DeliveryStatus = order.DeliveryStatus;
            }

            return true;


        }


        public async Task<bool> ApprovalForMoveOrder(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                    .ToListAsync();

            if (existing == null)
                return false;

            foreach(var items in existing)
            {

                items.ApprovedDate = DateTime.Now;
                items.ApproveDateTempo = DateTime.Now;
                items.IsApprove = true;
            }

            return true;

        }


        public async Task<bool> RejectForMoveOrder(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                      .ToListAsync();


            var existingOrders = await _context.Orders.Where(x => x.OrderNoPKey == moveorder.OrderNo)
                                                      .ToListAsync();

            if (existing == null)
                return false;

            foreach (var items in existing)
            {
                items.RejectBy = moveorder.RejectBy;
                items.RejectedDate = DateTime.Now;
                items.RejectedDateTempo = DateTime.Now;
                items.Remarks = moveorder.Remarks;
                items.IsReject = true;
                items.IsActive = false;
            }

            foreach (var items in existingOrders)
            {
                items.IsMove = false;
                items.IsReject = true;
                items.RejectBy = moveorder.RejectBy;
                items.Remarks = moveorder.Remarks;
                
            }


            return true;

        }

        public async Task<bool> ReturnMoveOrderForApproval(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                    .ToListAsync();


            var existingOrders = await _context.Orders.Where(x => x.OrderNoPKey == moveorder.OrderNo)
                                                      .ToListAsync();

            foreach (var items in existing)
            {
                items.RejectBy = null;
                items.RejectedDate = null;
                items.Remarks = moveorder.Remarks;
                items.IsReject = null;
                items.IsActive = true;
            }

            foreach (var items in existingOrders)
            {
                items.IsMove = true;
                items.IsReject = null;
                items.RejectBy = null;
                items.Remarks = moveorder.Remarks;

            }

            return true;

        }

        public async Task<PagedList<MoveOrderDto>> ForApprovalMoveOrderPagination(UserParams userParams)

        {
            var orders = _context.MoveOrders.GroupBy(x => new
            {

                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.PlateNumber

            }).Where(x => x.Key.IsApprove != true)
             .Where(x => x.Key.PlateNumber != null)


           .Select(x => new MoveOrderDto
           {
               OrderNo = x.Key.OrderNo,
               FarmName = x.Key.FarmName,
               FarmCode = x.Key.FarmCode,
               Category = x.Key.FarmType,
               Quantity = x.Sum(x => x.QuantityOrdered),
               OrderDate = x.Key.OrderDate.ToString(),
               PreparedDate = x.Key.PreparedDate.ToString(),
               PlateNumber = x.Key.PlateNumber

           });

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }

        public async Task<PagedList<MoveOrderDto>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search)
        {
            var orders = _context.MoveOrders.GroupBy(x => new
            {

                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.PlateNumber

            }).Where(x => x.Key.IsApprove != true)
              .Where(x => x.Key.PlateNumber != null)

          .Select(x => new MoveOrderDto
          {
              OrderNo = x.Key.OrderNo,
              FarmName = x.Key.FarmName,
              FarmCode = x.Key.FarmCode,
              Category = x.Key.FarmType,
              Quantity = x.Sum(x => x.QuantityOrdered),
              OrderDate = x.Key.OrderDate.ToString(),
              PreparedDate = x.Key.PreparedDate.ToString(),
              PlateNumber = x.Key.PlateNumber

          }).Where(x => Convert.ToString(x.OrderNo).ToLower()
            .Contains(search.Trim().ToLower()));

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IReadOnlyList<MoveOrderDto>> ViewMoveOrderForApproval(int orderid)
        {
            var orders = _context.MoveOrders.Select(x => new MoveOrderDto
            {

                Id = x.Id,
                OrderNo = x.OrderNo,
                BarcodeNo = x.WarehouseId,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Quantity = x.QuantityOrdered,
                Expiration = x.ExpirationDate.ToString()

            });

            return await orders.Where(x => x.OrderNo == orderid)
                               .ToListAsync();

        }

        public async Task<PagedList<MoveOrderDto>> ApprovedMoveOrderPagination(UserParams userParams)
        {
            var orders = _context.MoveOrders.GroupBy(x => new
            {

                x.OrderNo,
                x.WarehouseId,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.PlateNumber,
                x.IsPrepared,
                x.IsReject,
                x.ApproveDateTempo,
                x.IsPrint,
                x.IsTransact

            }).Where(x => x.Key.IsApprove == true)
              .Where(x => x.Key.PlateNumber != null)
              .Where(x => x.Key.IsReject != true)


             .Select(x => new MoveOrderDto
             {
                 OrderNo = x.Key.OrderNo,
                 BarcodeNo = x.Key.WarehouseId,
                 FarmName = x.Key.FarmName,
                 FarmCode = x.Key.FarmCode,
                 Category = x.Key.FarmType,
                 Quantity = x.Sum(x => x.QuantityOrdered),
                 OrderDate = x.Key.OrderDate.ToString(),
                 PreparedDate = x.Key.PreparedDate.ToString(),
                 PlateNumber = x.Key.PlateNumber,
                 IsApprove = x.Key.IsApprove != null,
                 IsPrepared = x.Key.IsPrepared,
                 ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                 IsPrint = x.Key.IsPrint != null,
                 IsTransact = x.Key.IsTransact != null

             });

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<MoveOrderDto>> ApprovedMoveOrderPaginationOrig(UserParams userParams, string search)
        {
            var orders = _context.MoveOrders.GroupBy(x => new
            {

                x.OrderNo,
                x.WarehouseId,
                x.ExpirationDate,
                x.ItemCode,
                x.ItemDescription,
                x.Uom,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.PlateNumber,
                x.IsPrepared,
                x.IsReject,
                x.ApproveDateTempo,
                x.IsPrint,
                x.IsTransact

            })
              .Where(x => x.Key.IsApprove == true)
              .Where(x => x.Key.PlateNumber != null)
              .Where(x => x.Key.IsReject != true)

             .Select(x => new MoveOrderDto
             {
                 OrderNo = x.Key.OrderNo,
                 BarcodeNo = x.Key.WarehouseId,
                 Expiration = x.Key.ExpirationDate.ToString(),
                 ItemCode = x.Key.ItemCode,
                 ItemDescription = x.Key.ItemDescription,
                 Uom = x.Key.Uom,
                 FarmName = x.Key.FarmName,
                 FarmCode = x.Key.FarmCode,
                 Category = x.Key.FarmType,
                 Quantity = x.Sum(x => x.QuantityOrdered),
                 OrderDate = x.Key.OrderDate.ToString(),
                 PreparedDate = x.Key.PreparedDate.ToString(),
                 PlateNumber = x.Key.PlateNumber,
                 IsApprove = x.Key.IsApprove != null,
                 IsPrepared = x.Key.IsPrepared,
                 ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                 IsPrint = x.Key.IsPrint != null,
                 IsTransact = x.Key.IsTransact != null

             }).Where(x => Convert.ToString(x.OrderNo).ToLower()
               .Contains(search.Trim().ToLower()));

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<MoveOrderDto>> RejectedMoveOrderPagination(UserParams userParams)
        {
            var orders = _context.MoveOrders.GroupBy(x => new
            {

                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.PlateNumber,
                x.IsReject,
                x.RejectedDateTempo,
                x.Remarks

            })
              .Where(x => x.Key.PlateNumber != null)
              .Where(x => x.Key.IsReject == true)


        .Select(x => new MoveOrderDto
        {
            OrderNo = x.Key.OrderNo,
            FarmName = x.Key.FarmName,
            FarmCode = x.Key.FarmCode,
            Category = x.Key.FarmType,
            Quantity = x.Sum(x => x.QuantityOrdered),
            OrderDate = x.Key.OrderDate.ToString(),
            PreparedDate = x.Key.PreparedDate.ToString(),
            PlateNumber = x.Key.PlateNumber,
            IsReject = x.Key.IsReject != null,
            RejectedDate = x.Key.RejectedDateTempo.ToString(),
            Remarks = x.Key.Remarks

        });

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<MoveOrderDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search)
        {
            var orders = _context.MoveOrders.GroupBy(x => new
            {

                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.PlateNumber,
                x.IsReject,
                x.RejectedDateTempo,
                x.Remarks

            })
              .Where(x => x.Key.PlateNumber != null)
              .Where(x => x.Key.IsReject == true)

       .Select(x => new MoveOrderDto
       {
           OrderNo = x.Key.OrderNo,
           FarmName = x.Key.FarmName,
           FarmCode = x.Key.FarmCode,
           Category = x.Key.FarmType,
           Quantity = x.Sum(x => x.QuantityOrdered),
           OrderDate = x.Key.OrderDate.ToString(),
           PreparedDate = x.Key.PreparedDate.ToString(),
           PlateNumber = x.Key.PlateNumber,
           IsReject = x.Key.IsReject != null,
           RejectedDate = x.Key.RejectedDateTempo.Value.ToString("MM/dd/yyyy"),
           Remarks = x.Key.Remarks

       }).Where(x => Convert.ToString(x.OrderNo).ToLower()
         .Contains(search.Trim().ToLower()));

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> UpdatePrintStatus(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                  .ToListAsync();
            if (existing == null)
                return false;

            foreach (var items in existing)
            {
                items.IsPrint = true;
            }

            return true;

        }

       
        //-----------------TRANSACT MOVE ORDER------------------------

        public async Task<IReadOnlyList<OrderDto>> TotalListForTransactMoveOrder()
        {

            var orders = _context.MoveOrders.GroupBy(x => new
            {
                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.DateNeeded,
                x.PreparedDate,
                x.PlateNumber,
                x.IsApprove,
                x.IsTransact

            }).Where(x => x.Key.IsApprove == true)
              .Where(x => x.Key.IsTransact != true)

           .Select(x => new OrderDto
           {
               OrderNo = x.Key.OrderNo,
               Farm = x.Key.FarmName,
               FarmCode = x.Key.FarmCode,
               Category = x.Key.FarmType,
               TotalOrders = x.Sum(x => x.QuantityOrdered),
               OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
               DateNeeded = x.Key.DateNeeded.ToString("MM/dd/yyyy"),
               PreparedDate = x.Key.PreparedDate.ToString(),
               PlateNumber = x.Key.PlateNumber,
               IsApproved = x.Key.IsApprove != null

           });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<MoveOrderDto>> ListOfMoveOrdersForTransact(int orderid)
        {
            var orders = _context.MoveOrders.Select(x => new MoveOrderDto
            {

                OrderNoPKey = x.OrderNoPKey,
                OrderNo = x.OrderNo,
                BarcodeNo = x.WarehouseId,
                OrderDate = x.OrderDate.ToString(),
                PreparedDate = x.PreparedDate.ToString(),
                DateNeeded = x.DateNeeded.ToString(),
                FarmCode = x.FarmCode,
                FarmName = x.FarmName,
                FarmType = x.FarmType,
                Category = x.Category,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                Expiration = x.ExpirationDate.ToString(),
                Quantity = x.QuantityOrdered,
                PlateNumber = x.PlateNumber,
                IsPrepared = x.IsPrepared, 
                IsApprove = x.IsApprove != null

            });

            return await orders.Where(x => x.OrderNo == orderid)
                               .ToListAsync();

        }

        public async Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact)
        {

            var existing = await _context.MoveOrders.Where(x => x.OrderNo == transact.OrderNo)
                                                    .ToListAsync();


            await _context.TransactMoveOrder.AddAsync(transact);


            foreach(var items in existing)
            {
                items.IsTransact = true;
            }

            return true;

        }

       
    }
}
