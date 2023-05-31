using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.CANCELLED_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.CANCELLED_ORDERS
{
    public class CancelledOrdersRepository : ICancelledOrders
    {
        private readonly StoreContext _context;

        public CancelledOrdersRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task<bool> VoidOrder(CancelledOrders cancelledOrder)
        {
            var existing = await _context.Orders.Where(x => x.Id == cancelledOrder.OrderId)
                .Where(x => x.IsActive == true)
                .FirstOrDefaultAsync();

            if (existing == null)
                return false;

            existing.IsCancelledOrder = true;
            await _context.CancelledOrders.AddAsync(cancelledOrder);
            return true;
        }


        public async Task<PagedList<CancelledOrderDTO>> GetAllcancelledOrdersPagination(UserParams userParams)
        {
            var customers = _context.Customers
                .Where(x => x.CancelledOrders.Any())
                .Include(x => x.CancelledOrders)
                .ThenInclude(x => x.Order)
                .Select(x => new CancelledOrderDTO
                {
                    CustomerId = x.Id,
                    CustomerName = x.CustomerName,
                    CustomerCode = x.CustomerCode,
                    LocationName = x.LocationName,
                    DepartmentName = x.DepartmentName,
                    FarmTypeId = x.FarmTypeId,
                    CompanyName = x.CompanyName,
                    CompanyCode = x.CompanyCode,
                    MobileNumber = x.MobileNumber,
                    LeadMan = x.LeadMan,
                    Address = x.Address,
                    CancelledOrders = x.CancelledOrders.Select(x => new OrdersforCancelledPaginationDTO
                    {
                        Id = x.Id,
                        OrderId = x.OrderId,
                        Reason = x.Reason,
                        CancellationDate = x.CancellationDate,
                        ItemCode = x.Order.ItemCode,
                        ItemDescription = x.Order.ItemDescription,
                        Category = x.Order.Category,
                        UOM = x.Order.Uom,
                        QuantityOrdered = x.Order.QuantityOrdered,
                        OrderDate = x.Order.OrderDate.ToString("dd/MM/yyyy"),
                        DateNeeded = x.Order.DateNeeded.ToString("dd/MM/yyyy")
                    }).ToList()
                });

            return await PagedList<CancelledOrderDTO>.CreateAsync(customers, userParams.PageNumber, userParams.PageSize);
        }

        //public async Task<Customer> GetAllCancelledOrdersByCustomer(int customerId)
        //{
        //    var customers = await _context.Customers
        //        .Include(x => x.CancelledOrders)
        //        .ThenInclude(c => c.Order)
        //        .FirstOrDefaultAsync(x => x.Id == customerId);

        //    if (customers != null)
        //    {
        //        var cancelledOrders = await _context.CancelledOrders
        //            .Where(co => co.Order.CustomerId == customerId)
        //            .ToListAsync();

        //        foreach (var cancelledOrder in cancelledOrders)
        //        {
        //            // Add the cancelled order to the corresponding order in the customer's Orders property
        //            var order = customers.Orders.FirstOrDefault(o => o.Id == cancelledOrder.OrderId);
        //            if (order != null)
        //            {
        //                order.CancelledOrders.Add(cancelledOrder);
        //            }
        //        }
        //    }
        //    return customers;
        //}
        public async Task<CancelledOrderDTO> GetCancelledOrdersById(int customerId)
        {
            return await _context.Customers
                .Include(x => x.CancelledOrders)
                .ThenInclude(x => x.Order)
                .Select(x => new CancelledOrderDTO
                {
                    CustomerId = x.Id,
                    CustomerName = x.CustomerName,
                    CustomerCode = x.CustomerCode,
                    LocationName = x.LocationName,
                    DepartmentName = x.DepartmentName,
                    FarmTypeId = x.FarmTypeId,
                    CompanyName = x.CompanyName,
                    CompanyCode = x.CompanyCode,
                    MobileNumber = x.MobileNumber,
                    LeadMan = x.LeadMan,
                    Address = x.Address,
                    CancelledOrders = x.CancelledOrders.Select(x => new OrdersforCancelledPaginationDTO
                    {
                        Id = x.Id,
                        OrderId = x.OrderId,
                        Reason = x.Reason,
                        CancellationDate = x.CancellationDate,
                        ItemCode = x.Order.ItemCode,
                        ItemDescription = x.Order.ItemDescription,
                        Category = x.Order.Category,
                        UOM = x.Order.Uom,
                        QuantityOrdered = x.Order.QuantityOrdered,
                        OrderDate = x.Order.OrderDate.ToString("dd/MM/yyyy"),
                        DateNeeded = x.Order.DateNeeded.ToString("dd/MM/yyyy")
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            //if (customers != null)
            //{
            //    var cancelledOrders = await _context.CancelledOrders
            //        .Where(co => co.Order.CustomerId == customerId)
            //        .ToListAsync();
            //    var orders = await _context.Orders
            //        .Where(o => o.IsCancelledOrder == null)
            //        .ToListAsync();
            //    //foreach (var cancelledOrder in cancelledOrders)
            //    //{
            //    //    Add the cancelled order to the corresponding order in the customer's Orders property
            //    //    var order = customers.Orders.FirstOrDefault(o => o.Id == cancelledOrder.OrderId);
            //    //    if (order != null)
            //    //    {
            //    //        order.CancelledOrders.Add(cancelledOrder);
            //    //    }
            //    //}
            //}
        }

        //public async Task<IEnumerable<Customer>> GetAllOrderandcancelledOrders()
        //{
        //    var customers = await _context.Customers
        //        .Include(x => x.CancelledOrders)
        //        .Include(x => x.Orders)
        //        .ToListAsync();

        //    if (customers != null)
        //    {
        //        var cancelledOrders = await _context.CancelledOrders.ToListAsync();
        //        var orders = await _context.Orders
        //            .Where(o => o.IsCancelledOrder == null)
        //            .ToListAsync();
        //    }
        //    return customers;
        //}

    }
}