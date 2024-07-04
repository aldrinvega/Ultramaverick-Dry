using RDF.Arcana.API.Common;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class UserRoleErrors
{
    public static Error UserRoleAlreadyExist(string userRole) => new ("UserRole.UserRoleAlreadyExist", $"{userRole} is already exist, try someting else.");
    public static Error UserRoleNotFound() => new ("UserRole.UserRoleNotFound", "No user role found.");

}
