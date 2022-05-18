using HookMaker.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HookMaker.Data.Context;

public class WebHooksDbContext
{
    public DbSet<WebHook> WebHooks { get; set; }

}