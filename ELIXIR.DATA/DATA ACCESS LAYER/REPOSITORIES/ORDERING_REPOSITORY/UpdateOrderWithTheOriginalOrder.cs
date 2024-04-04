using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.ORDERING_REPOSITORY;
[Route("api/update-orders"), ApiController]

public class UpdateOrderWithTheOriginalOrder : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateOrderWithTheOriginalOrder(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderWithTheOriginalOrderCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    public class UpdateOrderWithTheOriginalOrderCommand : IRequest<Unit>
    {
        
        public ICollection<Transactions> Transaction { get; set; }
        
        public class Transactions
        {
            public int TransactionId { get; set; }
            public decimal QuantityOrder { get; set; }
            public string ItemCode { get; set; }
        }
       
    }

    public class Handler : IRequestHandler<UpdateOrderWithTheOriginalOrderCommand, Unit>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateOrderWithTheOriginalOrderCommand request,
            CancellationToken cancellationToken)
        {
            const int batchSize = 1000; // Number of rows to update per session

            foreach (var transaction in request.Transaction)
            {
                // Get the last updated order ID for this transaction
                var lastUpdatedOrderId = await _context.Orders
                    .Where(o => o.TransactId == transaction.TransactionId && o.OriginalQuantityOrdered != 0)
                    .MaxAsync(o => (int?)o.Id, cancellationToken) ?? 0;

                // Calculate total number of orders for this transaction
                var totalCount = await _context.Orders
                    .Where(o => o.TransactId == transaction.TransactionId && o.OriginalQuantityOrdered != 0 &&
                                o.Id > lastUpdatedOrderId)
                    .CountAsync(cancellationToken);

                // Calculate number of sessions required based on batchSize
                var totalSessions = (int)Math.Ceiling((double)totalCount / batchSize);

                // Process each session
                for (var session = 0; session < totalSessions; session++)
                {
                    // Calculate skip count for pagination
                    var skipCount = session * batchSize;

                    // Retrieve orders for the current session
                    var ordersToUpdate = await _context.Orders
                        .Where(o => o.TransactId == transaction.TransactionId && o.ItemCode == transaction.ItemCode &&
                                    o.OriginalQuantityOrdered != 0 && o.Id > lastUpdatedOrderId)
                        .OrderBy(o => o.Id) // Assuming there's a unique identifier for the orders
                        .Skip(skipCount)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);

                    // Update original quantity ordered for each order
                    ordersToUpdate.ForEach(order => order.OriginalQuantityOrdered = transaction.QuantityOrder);
                }
            }

            // Save changes after processing all transactions
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}