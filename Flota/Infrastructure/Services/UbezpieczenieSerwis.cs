using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class UbezpieczenieSerwis : IUbezpieczenieSerwis
{
    private readonly FleetDbContext _context;

    public UbezpieczenieSerwis(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ubezpieczenie>> PobierzDlaPojazduAsync(int pojazdId)
    {
        return await _context.Ubezpieczenia
            .Where(u => u.PojazdId == pojazdId)
            .OrderByDescending(u => u.DataZakonczenia)
            .ToListAsync();
    }

    public async Task DodajAsync(Ubezpieczenie u)
    {
        _context.Ubezpieczenia.Add(u);
        await _context.SaveChangesAsync();
    }

    public async Task UsunAsync(int id)
    {
        var ubezp = await _context.Ubezpieczenia.FindAsync(id);
        if (ubezp != null)
        {
            _context.Ubezpieczenia.Remove(ubezp);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Ubezpieczenie>> PobierzWygasajaceAsync(int dni)
    {
        var dataGraniczna = DateTime.Now.AddDays(dni);
        return await _context.Ubezpieczenia
            .Include(u => u.Pojazd)
            .Where(u => u.DataZakonczenia >= DateTime.Now && u.DataZakonczenia <= dataGraniczna)
            .ToListAsync();
    }
}
