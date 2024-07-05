using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RDF.Arcana.API.Common;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class AddNewUserRole
{
    public class AddNewUserRoleCommand : IRequest<Result>
    {
        public string RoleName { get; set; }
        public string AddedBy { get; set; }
    }

    public class Handler : IRequestHandler<AddNewUserRoleCommand, Result>
    {
        private readonly StoreContext _context;
        public Handler(StoreContext context)
        {
            _context = context;
        }
        async Task<Result> IRequestHandler<AddNewUserRoleCommand, Result>.Handle(AddNewUserRoleCommand request, CancellationToken cancellationToken)
        {
            List<Error> errors = new();

            var existingRole = await _context.Roles.AnyAsync(r => r.RoleName == request.RoleName);

            if (existingRole)
            {
                errors.Add(UserRoleErrors.UserRoleAlreadyExist(request.RoleName));
            }

            if (string.IsNullOrEmpty(request.RoleName))
            {
                errors.Add(UserRoleErrors.RoleNameRequired());
            }

            if (errors.Any())
            {
                return Result.Failure(errors);
            }

            var role = new UserRole
            {
                RoleName = request.RoleName,
                AddedBy = request.AddedBy,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                IsActive = true
            };

            await _context.AddAsync(role, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

    }
}
