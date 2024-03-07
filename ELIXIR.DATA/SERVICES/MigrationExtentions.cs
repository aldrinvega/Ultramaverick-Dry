using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RDF.Arcana.API.Common;

public static class MigrationExtentions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using StoreContext dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();

        dbContext.Database.Migrate();
    } 
}
