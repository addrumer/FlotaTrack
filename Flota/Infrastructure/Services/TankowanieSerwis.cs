using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class TankowanieSerwis : ITankowanieSerwis
{
    private readonly FleetDbContext _context;

    public TankowanieSerwis(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tankowanie>> PobierzWszystkieAsync()
    {
        return await _context.Tankowania
            .Include(t => t.Pojazd)
            .OrderByDescending(t => t.Data)
            .ToListAsync();
    }

    public async Task DodajAsync(Tankowanie t)
    {
        if (t.LacznyKoszt == 0 && t.IloscLitrow > 0 && t.CenaZaLitr > 0)
        {
            t.LacznyKoszt = t.IloscLitrow * t.CenaZaLitr;
        }

        _context.Tankowania.Add(t);

        //Pobieramy auto, żeby zaktualizować jego przebieg
        var pojazd = await _context.Pojazdy.FindAsync(t.PojazdId);
        if (pojazd != null && t.Przebieg > pojazd.Przebieg)
        {
            pojazd.Przebieg = t.Przebieg;
        }

        await _context.SaveChangesAsync();
    }
}
