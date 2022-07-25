using Furion.DatabaseAccessor;

using Microsoft.EntityFrameworkCore;

namespace Ra2RulesEditorAPI.EntityFramework.Core;

[AppDbContext("Sqlite", DbProvider.Sqlite)]
public class DefaultDbContext : AppDbContext<DefaultDbContext>
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }
}