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
                          where ordering.FarmName == farms && ordering.PreparedDate == null && ordering.IsActive == true
                          join warehouse in totalRemaining
                          on ordering.ItemCode equals warehouse.ItemCode
                          into leftJ

                          from warehouse in leftJ.DefaultIfEmpty()

                          //join totalorders in totalOrders
                          //on warehouse.ItemCode equals totalorders.ItemCode

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
                              ordering.IsPrepared
                          //    totalorders.TotalOrders

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
                              StockOnHand = total.Sum(x => x.Remaining)
                            //  TotalOrders = total.Key.TotalOrders


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
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                               .FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;


            existingOrder.IsApproved = true;

            return true;

        }

        public async Task<bool> RejectPreparedDate(Ordering orders)
        {
            var existingOrder = await _context.Orders.Where(x => x.Id == orders.Id)
                                              .FirstOrDefaultAsync();

            if (existingOrder == null)
                return false;


            existingOrder.IsReject = true;

            return true;
        }

        public async Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo)
        {

            var orders = _context.Orders.Where(x => x.OrderDate >= DateTime.Parse(DateFrom) && x.OrderDate <= DateTime.Parse(DateTo))
                                        .Where(x => x.IsActive == true)
                                        .Select(x => new OrderDto
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
     
            var orders = _context.Orders              
                                        .GroupBy(x => new
                                        {
                                            x.FarmName,
                                            x.OrderDate,
                                            x.IsActive,
                                            x.PreparedDate

                                        }).Where(x => x.Key.IsActive == true)
                                          .Where(x => x.Key.PreparedDate == null)
                                          .OrderBy(x => x.Key.OrderDate)

                                          .Select(x => new OrderDto
                                          {

                                              Farm = x.Key.FarmName,
                                              OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
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

        public async Task<PagedList<OrderDto>> GetAllListForMoveOrderPagination(UserParams userParams)
        {

            var orders = _context.Orders
                                      .GroupBy(x => new
                                      {
                                          x.FarmName,
                                          x.OrderDate,
                                          x.IsActive,
                                          x.IsApproved,
                                          x.PreparedDate,

                                      }).Where(x => x.Key.IsActive == true)
                                        .Where(x => x.Key.IsApproved == true)
                                        .Where(x => x.Key.PreparedDate != null)
                                        .OrderBy(x => x.Key.OrderDate)

                                        .Select(x => new OrderDto
                                        {
                                            Farm = x.Key.FarmName,
                                            OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
                                            IsActive = x.Key.IsActive,
                                            IsApproved = x.Key.IsApproved != null,
                                            PreparedDate = x.Key.PreparedDate.ToString()
                                        });

            return await PagedList<OrderDto>.CreateAsync(orders, userParams.PageNumber, userParams.PageSize);


        }

        public async Task<IReadOnlyList<OrderDto>> GetAllListOfApprovedPreparedDate(string farm)
        {

            var orders = _context.Orders.GroupBy(x => new
            {

             //   x.Id,
                x.FarmName,
                x.FarmCode,
                x.FarmType,
                x.OrderDate,
                x.DateNeeded,
                x.PreparedDate,
                x.IsApproved,

            }).Where(x => x.Key.IsApproved == true)
              .Where(x => x.Key.PreparedDate != null)
     
            .Select(x => new OrderDto
            {
         //       Id = x.Key.Id,
                Farm = x.Key.FarmName, 
                FarmCode = x.Key.FarmCode,
                Category = x.Key.FarmType,
                TotalOrders = x.Sum(x => x.QuantityOrdered),
                OrderDate = x.Key.OrderDate.ToString("MM/dd/yyyy"),
                DateNeeded = x.Key.DateNeeded.ToString("MM/dd/yyyy"),
                PreparedDate = x.Key.PreparedDate.ToString()
      
            });

            return await orders.ToListAsync();

        }
    }
}
