using System.Threading.Tasks;
using Azure;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using static ELIXIR.API.Features.Setup.User_Roles.AddNewUserRole;
using static ELIXIR.API.Features.Setup.User_Roles.GetAllUserRolesAsync;
using static ELIXIR.API.Features.Setup.User_Roles.UpdateUserRole;
using static ELIXIR.API.Features.Setup.User_Roles.UpdateUserRoleStatus;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class UserRoleEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/user-roles");

        group.MapPost("", AddNewUserRole);
        group.MapPatch("/status/{id}", UpdateUserRoleStatus);
        group.MapPut("/{id}", UpdateUserRole);
        group.MapGet("/page", GetUserRoles);
    }

    public static async Task<IResult> AddNewUserRole(AddNewUserRoleCommand request, IMediator _mediator)
    {
        var result = await _mediator.Send(request);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }

    public static async Task<IResult> UpdateUserRoleStatus(int id, IMediator _mediator)
    {
        var command = new UpdateUserRoleStatusCommand
        {
            Id = id
        };

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }

    public static async Task<IResult> UpdateUserRole(UpdateUserRoleCommand command, int id, IMediator _mediator)
    {
        command.Id = id;

        var result = await _mediator.Send(command);

        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }

    public static async Task<IResult> GetUserRoles(
        int PageNumber, 
        int PageSize,
        bool Status,
        string Search,
        IMediator _mediator)
    {
        var request = new GetAllUserRolesAsyncQuery
        {
            PageNumber = PageNumber,
            PageSize = PageSize,
            Status = Status,
            Search = Search
        };

        var roles = await _mediator.Send(request);

        var rolesResult = new
        {
            roles,
            roles.CurrentPage,
            roles.PageSize,
            roles.TotalCount,
            roles.TotalPages,
            roles.HasNextPage,
            roles.HasPreviousPage
        };
        return Results.Ok(rolesResult);
    }
}
