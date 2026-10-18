using Microsoft.EntityFrameworkCore;

namespace MadWorldEU.Meishi;

public sealed class MeishiContext(DbContextOptions<MeishiContext> options) : DbContext(options)
{
    
}