using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class KierowcaSerwis : IKierowcaSerwis
{
    private readonly FleetDbContext _context;
    public KierowcaSerwis(FleetDbContext context) { _context = context; }

    public async Task<List<Kierowca>> PobierzWszystkichAsync() 
        => await _context.Kierowcy.ToListAsync();

    public async Task DodajAsync(Kierowca k) 
    {
        _context.Kierowcy.Add(k);
        await _context.SaveChangesAsync();
    }
    public async Task UsunAsync(int id)
    {
        var kierowca = await _context.Kierowcy.FindAsync(id);
        if (kierowca != null)
        {
            _context.Kierowcy.Remove(kierowca);
            await _context.SaveChangesAsync();
        }
    }
}
