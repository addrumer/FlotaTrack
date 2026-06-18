using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class PojazdSerwis : IPojazdSerwis {
    private readonly FleetDbContext _context;
    public PojazdSerwis(FleetDbContext context) { _context = context; }
    public async Task<List<Pojazd>> PobierzWszystkieAsync() => await _context.Pojazdy.ToListAsync();
    public async Task DodajAsync(Pojazd p) { _context.Pojazdy.Add(p); await _context.SaveChangesAsync(); }
    public async Task UsunAsync(int id)
    {
        var pojazd = await _context.Pojazdy.FindAsync(id);
        if (pojazd != null)
        {
            pojazd.Status = Domain.Enums.StatusPojazdu.Wylaczony;
        
            await _context.SaveChangesAsync();
        }
    }
}
