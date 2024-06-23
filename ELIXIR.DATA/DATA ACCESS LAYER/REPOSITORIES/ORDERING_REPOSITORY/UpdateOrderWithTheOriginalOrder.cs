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
            public int Id { get; set; }
            public int TransactionId { get; set; }
            public decimal? QuantityOrder { get; set; }
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
           
            
            foreach (var transaction in request.Transaction)
            {
                _context.Orders.Where(x => 
                x.Id == transaction.Id &&
                x.ItemCode == transaction.ItemCode && 
                x.TransactId == transaction.TransactionId)
                    .ExecuteUpdate(c => c.SetProperty(b => b.OriginalQuantityOrdered,  c => transaction.QuantityOrder));
            }

            // Save changes after processing all transactions
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}