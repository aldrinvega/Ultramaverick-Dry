using System.Threading.Tasks;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static ELIXIR.API.Features.Setup.User_Roles.AddNewUserRole;
using static ELIXIR.API.Features.Setup.User_Roles.GetAllRoleWithModules;
using static ELIXIR.API.Features.Setup.User_Roles.GetAllUserRolesAsync;
using static ELIXIR.API.Features.Setup.User_Roles.GetAllUserRolesPaginationAsync;
using static ELIXIR.API.Features.Setup.User_Roles.GetRoleModuleById;
using static ELIXIR.API.Features.Setup.User_Roles.GetRolesByStatus;
using static ELIXIR.API.Features.Setup.User_Roles.GetUntagModulesByRoleId;
using static ELIXIR.API.Features.Setup.User_Roles.TagModule;
using static ELIXIR.API.Features.Setup.User_Roles.UntagModules;
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
        group.MapGet("/page", GetUserRolesPagination);
        group.MapGet("/modules", GetRoleWithModules);
        group.MapGet("/untag/{id}/{menuId}", GetUntagModulesByRoleId);
        group.MapGet("module/{id}", GetRoleModuleById);
        group.MapGet("", GetUserRoles);
        group.MapGet("/{status}", GetRolesByStatus);
        group.MapPatch("/untag", UntagModule);
        group.MapPatch("/tag", TagModule);
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

    public static async Task<IResult> GetUserRolesPagination(
        int PageNumber, 
        int PageSize,
        bool Status,
        string Search,
        IMediator _mediator)
    {
        var request = new GetAllUserRolesPaginationAsyncQuery
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

    public static async Task<IResult> GetUserRoles(
         
        IMediator _mediator)
    {
        var query = new GetAllUserRolesAsyncQuery();
        var result = await _mediator.Send(query);

        return Results.Ok(result);
    }

    public static async Task<IResult> GetRoleWithModules(IMediator _mediator)
    {
        var query = new GetAllRolesWithModulesQuery();
        var result = await _mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> GetUntagModulesByRoleId(int Id, int MenuId, IMediator _mediator)
    {
        var query = new GetUntagModulesByRoleIdRequest
        {
            Id = Id,
            MenuId = MenuId
        };
        var result = await _mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> GetRoleModuleById(
        int id,
        IMediator _mediator)
    {

        var query = new GetRoleModuleByIdRequest
        {
            Id = id,
        };
        var rolemodule = await _mediator.Send(query);
        return Results.Ok(rolemodule);
    }

    public static async Task<IResult> GetRolesByStatus(bool Status, IMediator _mediator)
    {
        var query = new GetRolesByStatusQuery
        {
            Status = Status
        };
        var result = await _mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> UntagModule(UntagModulesCommand query,IMediator _mediator)
    {
        var result = await _mediator.Send(query);
        return Results.Ok(result);
    }

    public static async Task<IResult> TagModule(TagModuleCommand query, IMediator _mediator)
    {
        var result = await _mediator.Send(query);
        return Results.Ok(result);
    }
}
