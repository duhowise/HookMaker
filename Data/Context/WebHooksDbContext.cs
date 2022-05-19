using HookMaker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HookMaker.Data.Context;

public class WebHooksDbContext:DbContext
{
    public WebHooksDbContext(DbContextOptions options) :base(options:options)
    {
        
    }
    public DbSet<WebHook> WebHooks { get; set; }

}