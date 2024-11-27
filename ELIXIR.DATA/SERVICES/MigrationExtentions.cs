using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ELIXIR.DATA.SERVICES;

public static class MigrationExtentions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using StoreContext dbContext = scope.ServiceProvider.GetRequiredService<StoreContext>();

        //dbContext.Database.Migrate();
    } 
}
