 using ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
 using System.Linq;
 using System.Threading.Tasks;
 using ELIXIR.DATA.DTOs;
 using ELIXIR.DATA.SERVICES;
 using Microsoft.AspNetCore.SignalR;

 namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.ORDERING_REPOSITORY
{
    public class OrderingRepository : IOrdering
    {
        private readonly StoreContext _context;
        private readonly IHubContext<OrderHub> _clients;
        public OrderingRepository(StoreContext context, IHubContext<OrderHub> clients)
        {
            _context = context;
            _clients = clients;
        }
        public async Task<IReadOnlyList<OrderDto>> GetAllListofOrders(string farms)
        {
            var datenow = DateTime.Now;


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
             x.ItemCode

         }).Select(x => new OrderingInventory
         {
             ItemCode = x.Key.ItemCode,
             QuantityOrdered = x.Sum(order => order.AllocatedQuantity ?? (int)order.QuantityOrdered)
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

            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsTransact == true)
            .GroupBy(x => new
            {
                x.ItemCode,

            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
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

                              join issue in getIssueOut 
                              on warehouse.ItemCode equals issue.ItemCode
                              into leftJ3
                              from issue in leftJ3.DefaultIfEmpty()

                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering,
                                  issue
                              }
                              by new
                              {
                                  warehouse.ItemCode,
                                  ordering.QuantityOrdered,
                                  warehouse.ActualGood,
                                  issue.Quantity

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Key.ActualGood - ((total.Key.QuantityOrdered != null ? total.Key.QuantityOrdered : 0) + (total.Key.Quantity != null ? total.Key.Quantity : 0))
                              });

            var customer = _context.Customers.Select(x => new
            {
                x.DepartmentName,
                x.LocationName,
                x.CustomerCode,
                x.CustomerName,
                x.CompanyName,
                x.CompanyCode
            });

    
            // var totalRemaining = (from totalIn in _context.WarehouseReceived
            //                       where totalIn.IsActive == true
            //                       join totalOut in totalout
            //
            //                       on totalIn.ItemCode equals totalOut.ItemCode
            //                       into leftJ
            //                       from totalOut in leftJ.DefaultIfEmpty()
            //
            //                       join orderout in totalOrders on totalIn.ItemCode equals orderout.ItemCode
            //                       into leftJ2
            //                       from orderout in leftJ2.DefaultIfEmpty()
            //
            //                       group new
            //                       {
            //                           totalIn,
            //                           totalOut, 
            //                           orderout
            //
            //                       }
            //                       by new 
            //                       {
            //
            //             //              totalIn.Id,
            //                           totalIn.ItemCode,
            //                           totalIn.ItemDescription,
            //                           totalIn.ManufacturingDate,
            //                           totalIn.Expiration,
            //                           totalIn.ActualGood,
            //                           totalIn.ExpirationDays,
            //                         //  orderout.TotalOrders
            //
            //                       } into total
            //
            //                       orderby total.Key.ExpirationDays ascending
            //
            //                       select new ItemStocks
            //                       {
            //              //             WarehouseId = total.Key.Id,
            //                           ItemCode = total.Key.ItemCode,
            //                           ItemDescription = total.Key.ItemDescription,
            //                           ManufacturingDate = total.Key.ManufacturingDate,
            //                           ExpirationDate = total.Key.Expiration,
            //                           ExpirationDays = total.Key.ExpirationDays,
            //                           In = total.Key.ActualGood,
            //                     //      Out = total.Sum(x => x.Out),
            //                           Remaining = total.Key.ActualGood - 
            //                         //  TotalMoveOrder = total.Key.TotalOrders
            //
            //                       });

            var orders = (from ordering in _context.Orders
                where ordering.FarmName == farms && ordering.PreparedDate == null && ordering.IsActive == true &&
                      ordering.ForAllocation == null
                join warehouse in getReserve
                    on ordering.ItemCode equals warehouse.ItemCode
                    into leftJ
                from warehouse in leftJ.DefaultIfEmpty()

                join customers in customer on ordering.FarmCode equals customers.CustomerCode
                    into leftj1
                from customers in leftj1.DefaultIfEmpty()

                          group new
                          {
                              ordering,
                              warehouse,
                              customers
                          }
                              by new
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
                              ordering.AllocatedQuantity,
                              ordering.IsActive,
                              ordering.IsPrepared,
                              customers.CustomerCode,
                              customers.CompanyName,
                              customers.CompanyCode,
                              customers.LocationName,
                              customers.DepartmentName,
                              Reserve = warehouse.Reserve == null ? 0 : warehouse.Reserve,


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
                              QuantityOrder = total.Key.AllocatedQuantity == null ? total.Key.QuantityOrdered : (decimal)total.Key.AllocatedQuantity,
                              IsActive = total.Key.IsActive,
                              IsPrepared = total.Key.IsPrepared,
                              StockOnHand = total.Key.Reserve,
                              CompanyCode = total.Key.CustomerCode,
                              CompanyName = total.Key.CompanyName,
                              DepartmentName = total.Key.DepartmentName,
                              LocationName = total.Key.LocationName
                          });

            return await orders.ToListAsync();

        }
        public async Task<bool> EditQuantityOrder(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                                     .FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;
            if (existingOrder.AllocatedQuantity != null)
            {
                if (existingOrder.AllocatedQuantity >= orders.QuantityOrdered)
                {
                    existingOrder.AllocatedQuantity = (int)orders.QuantityOrdered;
                    return true;
                }
                return false;
            }

            if (existingOrder.QuantityOrdered >= orders.QuantityOrdered)
            {
                existingOrder.QuantityOrdered = orders.QuantityOrdered;
                return true;
            }
            return false;
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
                QuantityOrder = x.AllocatedQuantity == null ? x.QuantityOrdered : (decimal)x.AllocatedQuantity,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.DateNeeded.ToString("MM/dd/yyyy"),
                PreparedDate = x.PreparedDate.ToString(),
                IsApproved = x.IsApproved != null

            });

            return await orders.Where(x => x.IsApproved != true)
                               .ToListAsync();

        }
        public async Task<bool> ApprovePreparedDate(List<Ordering> orders)
        {
            var orderNos = orders.Select(o => o.OrderNoPKey);
            var activeOrders = await _context.Orders
                .Where(o => orderNos.Contains(o.OrderNoPKey) && o.IsActive)
                .ToListAsync();

            foreach (var order in activeOrders)
            {
                order.IsApproved = true;
                order.ApprovedDate = DateTime.Now;
                order.RejectBy = null;
                order.RejectedDate = null;
                order.Remarks = null;
            }
            
            return true;
        }
        public async Task<bool> RejectPreparedDate(List<Ordering> orders)
        {
            var orderNos = orders.Select(o => o.OrderNoPKey);
            var activeOrders = await _context.Orders
                .Where(o => orderNos.Contains(o.OrderNoPKey) && o.IsActive)
                .ToListAsync();

            foreach (var item in activeOrders)
            {
                item.IsReject = true;
                item.RejectBy = item.RejectBy;
                item.Remarks = item.Remarks;
                item.RejectedDate = DateTime.Now;
                item.PreparedDate = null;
                item.OrderNoPKey = 0;
                item.IsActive = true;
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
                x.IsActive,
                x.AllocatedQuantity
            }).Select(x => new OrderDto
            {
                ItemCode = x.Key.ItemCode,
                TotalOrders = x.Key.AllocatedQuantity == null ? x.Sum(x => x.QuantityOrdered) : (decimal)x.Sum(x => x.AllocatedQuantity),
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
                              ordering.IsApproved,
                              ordering.AllocatedQuantity
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
                              QuantityOrder = total.Key.AllocatedQuantity == null ? total.Key.QuantityOrdered : (decimal)total.Key.AllocatedQuantity,
                              IsActive = total.Key.IsActive,
                              IsPrepared = total.Key.IsPrepared,
                              StockOnHand = total.Sum(x => x.Remaining),
                              Difference = (total.Sum(x => x.Remaining) - (total.Key.AllocatedQuantity == null ? total.Key.QuantityOrdered : (decimal)total.Key.AllocatedQuantity)) < 0 ? 0 : total.Sum(x => x.Remaining) - (total.Key.AllocatedQuantity == null ? total.Key.QuantityOrdered : (decimal)total.Key.AllocatedQuantity) ,
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
        public async Task<bool> ValidateCustomerName(Ordering orders)
        {
            var customername = await _context.Customers.Where(x => x.CustomerName == orders.FarmName)
                                                       .Where(x => x.IsActive == true)
                                                       .FirstOrDefaultAsync();

            if (customername == null)
                return false;


            return true;
            
        }
        public async Task<bool> ValidateCustomerCode(Ordering orders)
        {
            var customercode = await _context.Customers.Where(x => x.CustomerCode == orders.FarmCode)
                                                   .Where(x => x.IsActive == true)
                                                   .FirstOrDefaultAsync();

            if (customercode == null)
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
                                            x.PreparedDate,
                                            x.ForAllocation

                                        }).Where(x => x.Key.IsActive == true)
                                          .Where(x => x.Key.PreparedDate == null)
                                          .Where(x => x.Key.ForAllocation == null)
                                        .Select(x => new OrderDto
                                          {
                                              Farm = x.Key.FarmName,
                                              IsActive = x.Key.IsActive
                                          });

            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);


        }
        public async Task<IReadOnlyList<OrderDto>> GetOrdersForNotification()
        {
            var orders = _context.Orders
                .OrderBy(x => x.OrderDate)
                .Where(x => x.IsActive == true)
                .Where(x => x.PreparedDate == null)
                .Where(x => x.ForAllocation == null)
                // .Where(x => x.AllocatedQuantity != null || x.QuantityOrdered != null)
                .GroupBy(x => new
                {
                    x.FarmName,
                    x.IsActive,
                    x.PreparedDate
                })
                .Select(x => new OrderDto
                {
                    Farm = x.Key.FarmName,
                    IsActive = x.Key.IsActive
                });
            return await orders.ToListAsync();
        }
        public async Task<bool> ValidateOrderAndDateNeeded(Ordering orders)
        {
            var dateNow = DateTime.Now;

            if (Convert.ToDateTime(orders.DateNeeded).Date < dateNow.Date)
                return false;

            return true;
        }
         public async Task<bool> ValidateIfForAllocation(List<Ordering> orders)
                {
                    var totalOrderedPerItem = orders.GroupBy(o => o.ItemCode)
                        .Select(g => new { ItemCode = g.Key, TotalOrdered = g.Sum(o => o.QuantityOrdered) });
                    
                    foreach (var order in orders)
                    {
                        var orderingReserve = await _context.Orders.Where(x => x.IsActive == true)
                                            .Where(x => x.PreparedDate != null)
                                            .Where(x => x.ItemCode == order.ItemCode)
                                            .SumAsync(order => order.AllocatedQuantity ?? (int)order.QuantityOrdered);

                        var receivedStocks = await _context.WarehouseReceived
                            .Where(x => x.ItemCode == order.ItemCode)
                            .Where(x => x.IsActive == true)
                            .SumAsync(x => x.ActualGood);
        
                        var reserve =  receivedStocks - orderingReserve;
                        
                        var totalOrderedForThisItem = totalOrderedPerItem
                            .Single(x => x.ItemCode == order.ItemCode)
                            .TotalOrdered;
        
                        if (totalOrderedForThisItem > reserve)
                        {
                            order.ForAllocation = true;
                        }
                    }
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
                QuantityOrder = x.AllocatedQuantity == null ? x.QuantityOrdered : (decimal)x.AllocatedQuantity,
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
                QuantityOrder = x.AllocatedQuantity == null ? x.QuantityOrdered : (decimal)x.AllocatedQuantity

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
                x.PreparedDate,
                x.IsApproved,
                x.IsActive,
                x.AllocatedQuantity
            }).Where(x => x.Key.IsApproved == null)
              .Where(x => x.Key.PreparedDate != null)
              .Where(x => x.Key.IsActive == true)
               .Select(x => new OrderDto
               {
                   OrderNo = x.Key.OrderNoPKey,
                   Farm = x.Key.FarmName,
                   FarmCode = x.Key.FarmCode,
                   Category = x.Key.FarmType,
                   TotalAllocatedOrder = x.Key.AllocatedQuantity == null ? (int)x.Sum(x => x.QuantityOrdered) : (int)x.Sum(x => x.AllocatedQuantity),
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
                QuantityOrder = x.AllocatedQuantity == null ? x.QuantityOrdered : (decimal)x.AllocatedQuantity,
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
                    x.Id,
                    x.FarmName,
                    x.IsActive,
                    x.IsApproved,
                    x.IsMove

                }).Where(x => x.Key.IsActive == true)
                .Where(x => x.Key.IsApproved == true)
                .Where(x => x.Key.IsMove == false)
                .Select(x => new OrderDto
                {
                    Id = x.Key.Id,
                    Farm = x.Key.FarmName,
                    IsActive = x.Key.IsActive,
                    IsApproved = x.Key.IsApproved != null
                });

            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<IReadOnlyList<TotalListOfPreparedDateDTO>> TotalListOfApprovedPreparedDate(string farm)
            {

            var orders = _context.Orders.GroupBy(x => new
            {

                x.OrderNoPKey,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
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
              TotalOrders = x.Sum(order => order.AllocatedQuantity ?? (int)order.QuantityOrdered),
              PreparedDate = x.Key.PreparedDate.ToString(),
              IsMove = x.Key.IsMove,
              IsReject = x.Key.IsReject != null,
              Remarks = x.Key.Remarks

          });

            var customer = _context.Customers.GroupBy(x => new
            {
                x.CustomerCode,
                x.LocationName,
                x.CompanyName,
                x.CompanyCode,
                x.DepartmentName
            }) 
                .Select(x => new
            {
                    x.Key.CustomerCode,
                    x.Key.LocationName,
                    x.Key.CompanyName,
                    x.Key.CompanyCode,
                    x.Key.DepartmentName
            });

            var oderResult = await (from order in orders
                                join cust in customer on order.FarmCode equals cust.CustomerCode
                                select new TotalListOfPreparedDateDTO
                                {
                                Id = order.Id,
                                FarmName = order.Farm,
                                FarmCode = order.FarmCode,
                                PreparedDate = order.PreparedDate,
                                IsMove = order.IsMove,
                                IsReject = order.IsReject,
                                QuantityOrder = (int)order.TotalOrders,
                                LocationName = cust.LocationName,
                                CompanyName = cust.CompanyName,
                                CompanyCode = cust.CompanyCode,
                                DepartmentName = cust.DepartmentName,
                                Remarks = order.Remarks
                                }).ToListAsync();

            return oderResult;
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
                x.IsActive,
                x.AllocatedQuantity
            }).Select(x => new OrderDto
            {
                ItemCode = x.Key.ItemCode,
                TotalOrders = x.Key.AllocatedQuantity == null ? x.Sum(x => x.QuantityOrdered) : (decimal)x.Sum(x => x.AllocatedQuantity),
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
                              ordering.IsApproved,
                              ordering.AllocatedQuantity
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
                              QuantityOrder = total.Key.AllocatedQuantity == null ? total.Key.QuantityOrdered : (decimal)total.Key.AllocatedQuantity,
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
            var moveorders = _context.MoveOrders.Where(x => x.IsActive == true || x.IsReject == true)
                .Where(x => x.IsRejectForPreparation == null)
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
                              ordering.PreparedDate,
                              ordering.DateNeeded,
                              ordering.FarmName,
                              ordering.FarmCode,
                              ordering.Category,
                              ordering.ItemCode,
                              ordering.ItemDescription,
                              ordering.Uom,
                              ordering.QuantityOrdered,
                              ordering.IsApproved,
                              ordering.AllocatedQuantity,
                              ordering.SetBy,
                              ordering.IsBeingPrepared

                          } into total
                          select new OrderDto
                          {
                              Id = total.Key.Id,
                              OrderNo = total.Key.OrderNoPKey,
                              OrderDate = total.Key.OrderDate.ToString("MM/dd/yyyy"),
                              DateNeeded = total.Key.DateNeeded.ToString("MM/dd/yyyy"),
                              PreparedDate = total.Key.PreparedDate.ToString(),
                              Farm = total.Key.FarmName,
                              FarmCode = total.Key.FarmCode,
                              Category = total.Key.Category,
                              ItemCode = total.Key.ItemCode,
                              ItemDescription = total.Key.ItemDescription,
                              Uom = total.Key.Uom,
                              QuantityOrder = total.Key.AllocatedQuantity == null ? total.Key.QuantityOrdered : (decimal)total.Key.AllocatedQuantity,
                              IsApproved = total.Key.IsApproved != null,
                              PreparedQuantity = total.Sum(x => x.QuantityPrepared),
                              SetBy = total.Key.SetBy,
                              IsBeingPrepared = total.Key.IsBeingPrepared
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
                    QuantityOrder = x.AllocatedQuantity == null ? x.QuantityOrdered : (decimal) x.AllocatedQuantity, 
                    Category = x.Category,
                    OrderDateTime = x.OrderDate,
                    DateNeededDateTime = x.DateNeeded,
                    PreparedDateTime = x.PreparedDate
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
                                                             .Where(x => x.IsActive == true)
                                                             .Where(x => x.ItemCode == itemcode)
                                                             .SumAsync(x => x.QuantityOrdered);

            var totalIssue = await _context.MiscellaneousIssueDetails.Where(x => x.WarehouseId == id)
                                                                     .Where(x => x.IsActive == true)
                                                                     .Where(x => x.IsTransact == true)
                                                                     .SumAsync(x => x.Quantity);

            var totalRemaining = from totalIn in _context.WarehouseReceived
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
                                      Remaining = total.Key.ActualGood - total.Sum(x => x.Out) - totaloutMoveorder - totalIssue
                                  };

            return await totalRemaining.Where(x => x.Remaining != 0)
                                       .FirstOrDefaultAsync();
        }
        public async Task<IReadOnlyList<MoveOrderDto>> ListOfPreparedItemsForMoveOrder(int id)
        {
            var orders = _context.MoveOrders.Where(x => x.OrderNo == id)
                .Where(x => x.IsActive == true || x.IsReject == true)
                .Where(x => x.IsRejectForPreparation == null)
                .Select(x => new MoveOrderDto
                {
                    Id = x.Id,
                    OrderNo = x.OrderNo,
                    BarcodeNo = x.WarehouseId,
                    ItemCode = x.ItemCode,
                    ItemDescription = x.ItemDescription,
                    Quantity = x.QuantityOrdered,
                    Expiration = x.ExpirationDate.ToString(),
                    IsActive = x.IsActive,
                });
            return await orders.ToListAsync();
        }
        public async Task<bool> CancelMoveOrder(MoveOrder moveorder)
        {

            var existingInfo = await _context.MoveOrders.Where(x => x.Id == moveorder.Id)
                                                        .FirstOrDefaultAsync();

            if (existingInfo == null)
                return false;

            existingInfo.IsActive = false;
            existingInfo.IsRejectForPreparation = true;
            existingInfo.CancelledDate = DateTime.Now;

            return true;
        }
        public async Task<bool> AddPlateNumberInMoveOrder(Ordering order)
        {

            var existing = await _context.Orders.Where(x => x.Id == order.Id)
                                                .FirstOrDefaultAsync();

            var existingMoveOrder = await _context.MoveOrders.Where(x => x.OrderNoPKey == order.Id)
                                                             .ToListAsync();

            if (existing == null)
                return false;

            existing.PlateNumber = order.PlateNumber;
            existing.IsMove = true;

            foreach(var items in existingMoveOrder)
            {
                items.PlateNumber = order.PlateNumber;
            }

            return true;
        }
        public async Task<bool> AddDeliveryStatus(OrderDto order)
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
                items.AccountTitles = order.AccountTitles;
                items.LocationName = order.LocationName;
                items.DepartmentName = order.DepartmentName;
                items.DepartmentCode = order.DepartmentCode;
                items.CompanyName = order.DepartmentName;
                items.CompanyCode = order.CompanyCode;
                items.AddedBy = order.AddedBy;
            }

            return true;
        }
        public async Task<bool> ApprovalForMoveOrder(IEnumerable<MoveOrder> moveorder)
        {
            foreach (var order in moveorder)
            {
                var existing = await _context.MoveOrders.Where(x => x.OrderNo == order.OrderNo)
                    .ToListAsync();

                if (existing == null)
                    return false;

                foreach(var items in existing)
                {
                    items.ApprovedDate = DateTime.Now;
                    items.ApproveDateTempo = DateTime.Now;
                    items.IsApprove = true;
                    items.IsReject = null;
                    items.Remarks = null;
                    items.AddedBy = items.AddedBy;
                    items.CheckedBy = order.CheckedBy;
                }
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
                items.IsActive = true;
                items.IsPrepared = true;
                items.IsApproveReject = null;
                items.DeliveryStatus = null;
               // items.IsRejectForPreparation = true;

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
        public async Task<bool> RejectApproveMoveOrder(MoveOrder moveorder)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == moveorder.OrderNo)
                                                     .ToListAsync();

            if (existing == null)
                return false;

            foreach (var items in existing)
            {
                items.RejectBy = moveorder.RejectBy;
                items.RejectedDate = DateTime.Now;
                items.RejectedDateTempo = DateTime.Now;
                items.Remarks = moveorder.Remarks;
                items.IsReject = null;
                items.IsApproveReject = true;
                items.IsActive = false;
                items.IsPrepared = true;
                items.IsApprove = false;
              
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
                items.IsPrepared = true;
                items.IsApprove = null;
                items.IsApproveReject = null;
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
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == null )
                .GroupBy(x => new
                {
                    x.OrderNo,
                    x.FarmName,
                    x.FarmCode,
                    x.FarmType,
                    x.OrderDate,
                    x.PreparedDate,
                    x.IsApprove,
                    x.DeliveryStatus,
                    x.IsPrepared,
                    x.IsActive
                }).Where(x => x.Key.IsApprove != true)
                .Where(x => x.Key.DeliveryStatus != null)
                .Where(x => x.Key.IsActive == true)
                .Select(x => new MoveOrderDto
                {
                    OrderNo = x.Key.OrderNo,
                    FarmName = x.Key.FarmName,
                    FarmCode = x.Key.FarmCode,
                    Category = x.Key.FarmType,
                    Quantity = x.Sum(x => x.QuantityOrdered),
                    OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
                    PreparedDate = x.Key.PreparedDate.ToString(),
                    DeliveryStatus = x.Key.DeliveryStatus,

                });

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);

        }
        public async Task<PagedList<MoveOrderDto>> ForApprovalMoveOrderPaginationOrig(UserParams userParams, string search)
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == null)
                .GroupBy(x => new
            {

                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.DeliveryStatus,
                x.IsPrepared,
                x.IsActive
            }).Where(x => x.Key.IsApprove != true)
              .Where(x => x.Key.DeliveryStatus != null)
              .Where(x => x.Key.IsActive == true)
                .Select(x => new MoveOrderDto
          {
              OrderNo = x.Key.OrderNo,
              FarmName = x.Key.FarmName,
              FarmCode = x.Key.FarmCode,
              Category = x.Key.FarmType,
              Quantity = x.Sum(x => x.QuantityOrdered),
              OrderDate = x.Key.OrderDate.ToString(),
              PreparedDate = x.Key.PreparedDate.ToString(),
              DeliveryStatus = x.Key.DeliveryStatus,

          }).Where(x => Convert.ToString(x.OrderNo).ToLower()
            .Contains(search.Trim().ToLower()));

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<IReadOnlyList<MoveOrderDto>> ViewMoveOrderForApproval(int orderid)
        {
            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
                .Select(x => new MoveOrderDto
            {  

                Id = x.Id,
                OrderNo = x.OrderNo,
                BarcodeNo = x.WarehouseId,
                ItemCode = x.ItemCode,
                ItemDescription = x.ItemDescription,
                Uom = x.Uom,
                FarmName = x.FarmName, 
                ApprovedDate = x.ApprovedDate.ToString(),
                Quantity = x.QuantityOrdered,
                Expiration = x.ExpirationDate.ToString(),
                DeliveryStatus = x.DeliveryStatus,
                PreparedBy = x.PreparedBy,
                CheckedBy = x.CheckedBy
            });

            return await orders.Where(x => x.OrderNo == orderid)
                               .ToListAsync();

        }
        public async Task<PagedList<MoveOrderDto>> ApprovedMoveOrderPagination(UserParams userParams)
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
                .Where(x => x.Key.IsReject != true)
                .Select(x => new MoveOrderDto
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
                });
            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<PagedList<MoveOrderDto>> Approvedination(UserParams userParams)
        {
            var orders = _context.MoveOrders.GroupBy(x => new
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
                x.IsActive
            }).Where(x => x.Key.IsApprove == true)
              .Where(x => x.Key.DeliveryStatus != null)
              .Where(x => x.Key.IsReject != true)
              .Where(X => X.Key.IsActive == true)
             .Select(x => new MoveOrderDto
             {
                 OrderNo = x.Key.OrderNo,
                 FarmName = x.Key.FarmName,
                 FarmCode = x.Key.FarmCode,
                 Category = x.Key.FarmType,
                 Quantity = x.Sum(y => y.QuantityOrdered),
                 PreparedDate = x.Key.PreparedDate.ToString(),
                 DeliveryStatus = x.Key.DeliveryStatus,
                 IsApprove = x.Key.IsApprove != null,
                 IsPrepared = x.Key.IsPrepared,
                 ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                 IsPrint = x.Key.IsPrint != null,
                 IsTransact = x.Key.IsTransact,
             });

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<PagedList<MoveOrderDto>> ApprovedMoveOrderPaginationOrig(UserParams userParams, string search)
        {
            var orders = _context.MoveOrders
              .Where(X => X.IsActive == true)
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
                x.IsTransact
            })
              .Where(x => x.Key.IsApprove == true)
              .Where(x => x.Key.DeliveryStatus != null)
              .Where(x => x.Key.IsReject != true)
              .Select(x => new MoveOrderDto
             {
                 OrderNo = x.Key.OrderNo,
                 FarmName = x.Key.FarmName,
                 FarmCode = x.Key.FarmCode,
                 Category = x.Key.FarmType,
                 Quantity = x.Sum(y => y.QuantityOrdered),
                 PreparedDate = x.Key.PreparedDate.ToString(),
                 DeliveryStatus = x.Key.DeliveryStatus,
                 IsApprove = x.Key.IsApprove != null,
                 IsPrepared = x.Key.IsPrepared,
                 ApprovedDate = x.Key.ApproveDateTempo.ToString(),
                 IsPrint = x.Key.IsPrint != null,
                 IsTransact = x.Key.IsTransact,

             }).Where(x => Convert.ToString(x.OrderNo).ToLower()
               .Contains(search.Trim().ToLower()));

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<PagedList<MoveOrderDto>> RejectedMoveOrderPagination(UserParams userParams)
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true)
                .GroupBy(x => new
            {
                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.DeliveryStatus,
                x.IsReject,
                x.RejectedDateTempo,
                x.Remarks
            })
              .Where(x => x.Key.DeliveryStatus != null)
         
        .Select(x => new MoveOrderDto
        {
            OrderNo = x.Key.OrderNo,
            FarmName = x.Key.FarmName,
            FarmCode = x.Key.FarmCode,
            Category = x.Key.FarmType,
            Quantity = x.Sum(y => y.QuantityOrdered),
            OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
            PreparedDate = x.Key.PreparedDate.ToString(),
            DeliveryStatus = x.Key.DeliveryStatus,
            IsReject = x.Key.IsReject != null,
            RejectedDate = x.Key.RejectedDateTempo.ToString(),
            Remarks = x.Key.Remarks
        });

            return await PagedList<MoveOrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<PagedList<MoveOrderDto>> RejectedMoveOrderPaginationOrig(UserParams userParams, string search)
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true)
                .GroupBy(x => new
            {
                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.DeliveryStatus,
                x.IsReject,
                x.RejectedDateTempo,
                x.Remarks
            })
              .Where(x => x.Key.DeliveryStatus != null)

       .Select(x => new MoveOrderDto
       {
           OrderNo = x.Key.OrderNo,
           FarmName = x.Key.FarmName,
           FarmCode = x.Key.FarmCode,
           Category = x.Key.FarmType,
           Quantity = x.Sum(x => x.QuantityOrdered),
           OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
           PreparedDate = x.Key.PreparedDate.ToString(),
           DeliveryStatus = x.Key.DeliveryStatus,
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
        public async Task<MoveOrderDto> GetAllApprovedMoveOrder(int id)
        {
            var orders = _context.MoveOrders.Where(x => x.OrderNoPKey == id)
           .GroupBy(x => new
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
                x.IsTransact,
                x.DeliveryStatus

            }).Where(x => x.Key.IsApprove == true)
            .Where(x => x.Key.DeliveryStatus != null)
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
               DeliveryStatus = x.Key.DeliveryStatus,
               IsApprove = x.Key.IsApprove != null,
               IsPrepared = x.Key.IsPrepared,
               ApprovedDate = x.Key.ApproveDateTempo.ToString(),
               IsPrint = x.Key.IsPrint != null,
               IsTransact = x.Key.IsTransact != null

           });

            return await orders.FirstOrDefaultAsync();
        }
        public async Task<ItemStocks> GetFirstExpiry(string itemcode)
        {
            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true).GroupBy(x => new
          {
              x.Id,
              x.ItemCode,
              x.ExpirationDays

          }).Select(x => new WarehouseInventory
          {
              WarehouseId = x.Key.Id,
              ItemCode = x.Key.ItemCode,
              ActualGood = x.Sum(x => x.ActualGood),
              ExpirationDays = x.Key.ExpirationDays
              
          });

            var getTransformationPreparation = _context.Transformation_Preparation.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Out = x.Sum(x => x.WeighingScale),
                WarehouseId = x.Key.WarehouseId
            });

            var getMoveorder = _context.MoveOrders.Where(x => x.IsActive == true).GroupBy(x => new
            {
                x.ItemCode,
                x.WarehouseId,

            }).Select(x => new ItemStocks
            {
                ItemCode = x.Key.ItemCode,
                Remaining = x.Sum(x => x.QuantityOrdered),
                WarehouseId = x.Key.WarehouseId
            });

            var totalRemaining = (from warehouse in getWarehouseIn
                                  join preparation in getTransformationPreparation
                                  on warehouse.WarehouseId equals preparation.WarehouseId
                                  into leftJ1
                                  from preparation in leftJ1.DefaultIfEmpty()

                                  join moveorder in getMoveorder
                                  on warehouse.WarehouseId equals moveorder.WarehouseId
                                  into leftJ2
                                  from moveorder in leftJ2.DefaultIfEmpty()

                                  group new
                                  {
                                      warehouse,
                                      preparation,
                                      moveorder
                                  }
                                  
                                  by new
                                  {

                                   warehouse.WarehouseId,
                                   warehouse.ItemCode, 
                                   warehouse.ExpirationDays


                                  } into total

                                  orderby total.Key.ExpirationDays ascending

                                  select new ItemStocks
                                  {
                                      WarehouseId = total.Key.WarehouseId,
                                      ItemCode = total.Key.ItemCode,                     
                                      ExpirationDays = total.Key.ExpirationDays,
                                      Remaining = total.Sum(x => x.warehouse.ActualGood == null ? 0 : x.warehouse.ActualGood) -
                                                  total.Sum(x => x.preparation.Out == null ? 0 : x.preparation.Out) -
                                                  total.Sum(x => x.moveorder.Remaining == null ? 0 : x.moveorder.Remaining) 
                                  });

            return await totalRemaining.Where(x => x.Remaining != 0)
                                       .Where(x => x.ItemCode == itemcode)
                                       .FirstOrDefaultAsync();
        }
        public async Task<bool> SetBeingPrepared(List<Ordering> moveOrders)
        {
            foreach (var moveOrder in moveOrders)
            {
                var existingOrder = await _context.Orders.Where(x => x.OrderNoPKey == moveOrder.OrderNoPKey)
                    .Where(x => x.IsBeingPrepared == null || x.IsBeingPrepared == false)
                    .FirstOrDefaultAsync();
                    
                if (existingOrder == null)
                    return false;

                existingOrder.IsBeingPrepared = moveOrder.IsBeingPrepared;
                existingOrder.SetBy = moveOrder.SetBy;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool>UnsetBeingPrepared(List<Ordering> orderNos)
        {
            foreach (var orderNo in orderNos)
            {
                var existingOrder = await _context.Orders.Where(x => x.IsBeingPrepared == true)
                    .Where(x => x.OrderNoPKey == orderNo.OrderNoPKey)
                    .Where(x => x.SetBy == orderNo.SetBy)
                      .FirstOrDefaultAsync();

                if (existingOrder == null)
                    return false;

                existingOrder.IsBeingPrepared = orderNo.IsBeingPrepared;
                existingOrder.SetBy = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        //-----------------TRANSACT MOVE ORDER------------------------
        public async Task<IReadOnlyList<OrderDto>> TotalListForTransactMoveOrder(bool status)
        {

            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
                                            .Where(x => x.IsTransact == status)
                .GroupBy(x => new
            {
                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.DateNeeded,
                x.PreparedDate,
                x.DeliveryStatus,
                x.IsApprove,
                x.IsTransact,
            }).Where(x => x.Key.IsApprove == true)

           .Select(x => new OrderDto
           {
               OrderNo = x.Key.OrderNo,
               Farm = x.Key.FarmName,
               FarmCode = x.Key.FarmCode,
               FarmType = x.Key.FarmType,
               Category = x.Key.FarmType,
               TotalOrders = x.Sum(x => x.QuantityOrdered),
               DateNeeded = x.Key.DateNeeded.ToString("MM/dd/yyyy"),
               PreparedDate = x.Key.PreparedDate.ToString(),
               DeliveryStatus = x.Key.DeliveryStatus,
               IsApproved = x.Key.IsApprove != null
           });

            return await orders.ToListAsync();

        }
        public async Task<IReadOnlyList<MoveOrderDto>> ListOfMoveOrdersForTransact(int orderid)
        {
            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
                                            .Where(x => x.IsApprove == true)
                .Select(x => new MoveOrderDto
            {
                OrderNoPKey = x.OrderNoPKey,
                OrderNo = x.OrderNo,
                BarcodeNo = x.WarehouseId,
                OrderDate = x.OrderDate.ToString("MM/dd/yyyy"),
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

            return await orders.Where(x => x.OrderNo == orderid).ToListAsync();
        }
        public async Task<bool> TransanctListOfMoveOrders(TransactMoveOrder transact)
        {
            var existing = await _context.MoveOrders.Where(x => x.OrderNo == transact.OrderNo).ToListAsync();
            
            await _context.TransactMoveOrder.AddAsync(transact);


            foreach(var items in existing)
            {
                items.IsTransact = true;
            }    

            return true;

        }
        public async Task<IReadOnlyList<OrderDto>> GetMoveOrdersForNotification()
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

            return await orders.ToListAsync();

        }
        public async Task<IReadOnlyList<OrderDto>> GetAllForTransactMoveOrderNotification()
        {
            var orders = _context.MoveOrders.Where(x => x.IsActive == true)
                                          .Where(x => x.IsTransact == false)
              .GroupBy(x => new
              {
                  x.OrderNo,
                  x.FarmName,
                  x.FarmCode,
                  x.FarmType,
                  x.OrderDate,
                  x.DateNeeded,
                  x.PreparedDate,
                  x.DeliveryStatus,
                  x.IsApprove,
                  x.IsTransact

              }).Where(x => x.Key.IsApprove == true)

         .Select(x => new OrderDto
         {
             OrderNo = x.Key.OrderNo,
             Farm = x.Key.FarmName,
             FarmCode = x.Key.FarmCode,
             FarmType = x.Key.FarmType,
             Category = x.Key.FarmType,
             TotalOrders = x.Sum(x => x.QuantityOrdered),
             OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
             DateNeeded = x.Key.DateNeeded.ToString("MM/dd/yyyy"),
             PreparedDate = x.Key.PreparedDate.ToString(),
             DeliveryStatus = x.Key.DeliveryStatus,
             IsApproved = x.Key.IsApprove != null

         });

            return await orders.ToListAsync();
        }       
        public async Task<IReadOnlyList<MoveOrderDto>> GetForApprovalMoveOrderNotification()
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == null)
             .GroupBy(x => new
             {

                 x.OrderNo,
                 x.FarmName,
                 x.FarmCode,
                 x.FarmType,
                 x.OrderDate,
                 x.PreparedDate,
                 x.IsApprove,
                 x.DeliveryStatus,
                 x.IsPrepared,
                 x.IsActive
             }).Where(x => x.Key.IsApprove != true)
           .Where(x => x.Key.DeliveryStatus != null)
           .Where(x => x.Key.IsPrepared == true)
           .Where(x => x.Key.IsActive == true)

        .Select(x => new MoveOrderDto
        {
            OrderNo = x.Key.OrderNo,
            FarmName = x.Key.FarmName,
            FarmCode = x.Key.FarmCode,
            Category = x.Key.FarmType,
            Quantity = x.Sum(x => x.QuantityOrdered),
            OrderDate = x.Key.OrderDate.ToString(),
            PreparedDate = x.Key.PreparedDate.ToString(),
            DeliveryStatus = x.Key.DeliveryStatus

        });

            return await orders.ToListAsync();

        }

        public async Task<IReadOnlyList<MoveOrderDto>> GetAllapprovedMoveorderNotification()
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
                .Where(x => x.Key.IsReject != true)
                .Select(x => new MoveOrderDto
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
                });

            return await orders.ToListAsync();
        }

        public async Task<IReadOnlyList<MoveOrderDto>> GetRejectMoveOrderNotification()
        {
            var orders = _context.MoveOrders.Where(x => x.IsApproveReject == true)
            .GroupBy(x => new
            {

                x.OrderNo,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.PreparedDate,
                x.IsApprove,
                x.DeliveryStatus,
                x.IsReject,
                x.RejectedDateTempo,
                x.Remarks

            })
          .Where(x => x.Key.DeliveryStatus != null)

        .Select(x => new MoveOrderDto
        {
            OrderNo = x.Key.OrderNo,
            FarmName = x.Key.FarmName,
            FarmCode = x.Key.FarmCode,
            Category = x.Key.FarmType,
            Quantity = x.Sum(x => x.QuantityOrdered),
            OrderDate = x.Key.OrderDate.ToString(),
            PreparedDate = x.Key.PreparedDate.ToString(),
            DeliveryStatus = x.Key.DeliveryStatus,
            IsReject = x.Key.IsReject != null,
            RejectedDate = x.Key.RejectedDateTempo.ToString(),
            Remarks = x.Key.Remarks

        });

            return await orders.ToListAsync();

        }
        public async Task<bool> CancelControlInMoveOrder(Ordering order)
        {

            var existorders = await _context.Orders.Where(x => x.OrderNoPKey == order.OrderNoPKey)
                                             .ToListAsync();

            var existMoveorders = await _context.MoveOrders.Where(x => x.OrderNo == order.OrderNoPKey)
                                                           .ToListAsync();
  
            foreach(var items in existorders)
            {
                items.IsApproved = null;
                items.ApprovedDate = null;
                items.DeliveryStatus = null;
            }

            if(existMoveorders != null)
            {
                foreach (var items in existMoveorders)
                {
                    items.IsActive = false;
                }
            }

            return true;

        }
        public async Task<IReadOnlyList<OrderDto>> GetAllApprovedOrdersForCalendar()
        {

            var orders = _context.Orders.GroupBy(x => new
            {

                x.OrderNoPKey,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.PreparedDate,
                x.IsApproved,
                x.IsMove,
                x.IsReject,
                x.Remarks,
                x.AllocatedQuantity

            })
         .Where(x => x.Key.IsApproved == true)
         .Where(x => x.Key.PreparedDate != null)
         .Where(x => x.Key.IsMove == false)


       .Select(x => new OrderDto
       {
           Id = x.Key.OrderNoPKey,
           Farm = x.Key.FarmName,
           FarmCode = x.Key.FarmCode,
           Category = x.Key.FarmType,
           TotalOrders = x.Key.AllocatedQuantity == null ? x.Sum(x => x.QuantityOrdered) : (decimal)x.Sum(x => x.AllocatedQuantity),
           PreparedDate = x.Key.PreparedDate.ToString(),
           IsMove = x.Key.IsMove,
           IsReject = x.Key.IsReject != null,
           Remarks = x.Key.Remarks
       });
            return await orders.ToListAsync();
        }
        
        //////// ALLOCATION OF ORDERS PER ORDER (ITEMS BASIS) //////////
        
       // public async Task<bool> AllocateOrdersPerItems(List<Ordering> orders)
       // {
       //     foreach (var order in orders)
       //     {
       //         var receivedStocks = await _context.WarehouseReceived
       //             .Where(x => x.ItemCode == order.ItemCode)
       //             .SumAsync(x => x.ActualGood);
       //
       //         var totalOrdersPerItemAndStore = await _context.Orders
       //             .Where(x => x.ItemCode == order.ItemCode && x.FarmName == order.FarmName)
       //             .Where(x => x.AllocatedQuantity == null)
       //             .SumAsync(x => x.QuantityOrdered);
       //
       //         if (receivedStocks >= totalOrdersPerItemAndStore)
       //         {
       //             order.AllocatedQuantity = (int)order.QuantityOrdered;
       //         }
       //         else
       //         {
       //             int allocatedStocks = (int)(order.QuantityOrdered / receivedStocks);
       //             order.AllocatedQuantity = allocatedStocks;
       //         }
       //     }
       //     try
       //     {
       //         await _context.SaveChangesAsync();
       //         return true;
       //     }
       //     catch (Exception e)
       //     {
       //         Console.WriteLine(e.Message);
       //         return false;
       //     }
       // }
       
       // public async Task<int> GetReceivedStocksPerItem(string itemCode)
       // {
       //     return await _context.WarehouseReceived
       //         .Where(x => x.ItemCode == itemCode)
       //         .SumAsync(x => x.Quantity);
       // }
       
       public async Task<IReadOnlyList<AllocationResult>> AllocateOrdersPerItems(List<AllocationDTO> itemCodes)
       {
            // Get a list of orders that have an item code in the `itemCodes` list
            var orders = await _context.Orders.Where(x => x.IsActive == true)
               .Where(x => itemCodes.Select(y =>y.ItemCode).Contains(x.ItemCode))
               .Where(x => x.IsActive == true)
               .Where(x => x.ForAllocation == true)
               .ToListAsync();

            var results = new List<AllocationResult>();
            
           foreach (var order in orders)
           {
                var orderingReserve = await _context.Orders.Where(x => x.IsActive == true)
                                                     .Where(x => x.PreparedDate != null)
                                                     .Where(x => x.ItemCode == order.ItemCode)
                                                     .SumAsync(order => order.AllocatedQuantity ?? (int)order.QuantityOrdered);

                var getIssueOut = await _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                             .Where(x => x.IsTransact == true)
                                                             .Where(x => x.ItemCode == order.ItemCode)
                                                             .SumAsync(x => x.Quantity);
           
                // Get the sum of received stocks for the current order's item code
                var receivedStocks = await _context.WarehouseReceived
                   .Where(x => x.ItemCode == order.ItemCode)
                   .Where(x => x.IsActive == true)
                   .SumAsync(x => x.ActualGood);

               // Get the sum of quantity ordered for the current order's item code and farm name where the allocated quantity is null
               var totalOrdersPerItemAndStore = await _context.Orders
                   .Where(x => x.ItemCode == order.ItemCode)
                   .Where(x => x.AllocatedQuantity == null)
                   .Where(x => x.IsActive == true)
                   .SumAsync(x => x.QuantityOrdered);

               var totalStocks = receivedStocks - (orderingReserve + getIssueOut);

               if (totalStocks <= 0)
               {
                   totalStocks = 0;
               }

               var percentageToAllocate = totalOrdersPerItemAndStore / totalStocks;

               if (totalOrdersPerItemAndStore <= totalStocks)
               {
                   order.AllocatedQuantity = (int)order.QuantityOrdered;
                   order.ForAllocation = null;
               }
               else
               {
                   var allocatedStocks = (int)(order.QuantityOrdered/percentageToAllocate);
                   order.AllocatedQuantity = allocatedStocks;
                   order.ForAllocation = null;
               }
               
               results.Add(new AllocationResult
               {
                   AllocatedQuantity = order.AllocatedQuantity,
                   CustomerName = order.FarmName,
                   OrderNo = order.OrderNo,
               });
           }
           
           return results;
       }
       public async Task<bool> ManualAllocationForOrders(List<ManualAllocation> manualAllocations)
       {
           var orders = await _context.Orders.Where(x => x.IsActive == true)
               .Where(x => manualAllocations.Select(y =>y.Id).Contains(x.OrderNo))
               .Where(x => x.IsActive == true)
               .Where(x => x.ForAllocation == true)
               .ToListAsync();

           foreach (var order in orders)
           {
               var manualAllocation = manualAllocations.FirstOrDefault(x => x.Id == order.OrderNo);
               order.AllocatedQuantity = manualAllocation.QuantityOrdered;
               order.ForAllocation = null;
           }
           return true;
       }
        public async Task<PagedList<OrderDto>> GetAllListofOrdersForAllocationPagination(UserParams userParams)
        {
            
            var orders = _context.Orders.OrderBy(x => x.OrderDate)              
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.IsActive,
                    x.PreparedDate,
                    x.ForAllocation

                }).Where(x => x.Key.IsActive == true)
                .Where(x => x.Key.PreparedDate == null)
                .Where(x => x.Key.ForAllocation != null)
                .Select(x => new OrderDto
                {
                    ItemCode = x.Key.ItemCode,
                    IsActive = x.Key.IsActive
                });
            
            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<IReadOnlyList<OrderDto>> GetAllListofOrdersAllocation(string itemCode)
        {
            var datenow = DateTime.Now;
        
        
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
             x.ItemCode
        
         }).Select(x => new OrderingInventory
         {
             ItemCode = x.Key.ItemCode,
             QuantityOrdered = x.Sum(order => order.AllocatedQuantity ?? (int)order.QuantityOrdered)
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
        
            var getIssueOut = _context.MiscellaneousIssueDetails.Where(x => x.IsActive == true)
                                                            .Where(x => x.IsTransact == true)
            .GroupBy(x => new
            {
                x.ItemCode,
        
            }).Select(x => new IssueInventory
            {
                ItemCode = x.Key.ItemCode,
                Quantity = x.Sum(x => x.Quantity)
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
        
                              join issue in getIssueOut 
                              on warehouse.ItemCode equals issue.ItemCode
                              into leftJ3
                              from issue in leftJ3.DefaultIfEmpty()
        
                              group new
                              {
                                  warehouse,
                                  request,
                                  ordering,
                                  issue
                              }
                              by new
                              {
                                  warehouse.ItemCode,
                                  ordering.QuantityOrdered,
                                  warehouse.ActualGood,
                                  issue.Quantity
        
                              } into total
        
                              select new ReserveInventory
                              {
        
                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Key.ActualGood - ((total.Key.QuantityOrdered != null ? total.Key.QuantityOrdered : 0) + (total.Key.Quantity == null ? 0 : total.Key.Quantity))
                              });

            var orders = (from ordering in _context.Orders
                          where ordering.ItemCode == itemCode && ordering.PreparedDate == null && ordering.IsActive == true && ordering.ForAllocation != null
                          join warehouse in getReserve
                          on ordering.ItemCode equals warehouse.ItemCode
                          into leftJ
                          from warehouse in leftJ.DefaultIfEmpty()
        
                          group new
                          {
                              ordering, 
                              warehouse
                          }
                              by new
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
                              Reserve = warehouse.Reserve != null ? warehouse.Reserve : 0,
        
        
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
                              StockOnHand = total.Key.Reserve
                     //         Days = total.Key.DateNeeded.Subtract(datenow).Days                         
                          });
        
            return await orders.ToListAsync();
        
        }
        public async Task<IReadOnlyList<OrderDto>> GetForAllocationOrdersForNotification()
        {
            var orders = _context.Orders
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.IsActive,
                    x.IsApproved,
                    x.IsMove,
                    x.AllocatedQuantity,
                    x.ForAllocation,
                    x.PreparedDate
        
                })
                .Where(x => x.Key.IsActive == true)
                .Where(x => x.Key.PreparedDate == null)
                .Where(x => x.Key.ForAllocation != null)
                .Select(x => new OrderDto
                {
                    ItemCode = x.Key.ItemCode,
                    IsActive = x.Key.IsActive,
                    AllocatedQuantity = x.Key.AllocatedQuantity
                });
        
            return await orders.ToListAsync();
        
        }
        public async Task<bool> CancelForPendingAllocation(string customer)
        {
            var existing = await _context.Orders.Where(x => x.CustomerName == customer)
                .Where(x => x.IsActive == true)
                .FirstOrDefaultAsync();
        
            if (existing == null)
                return false;
        
            return true;
        }

        
    }
}