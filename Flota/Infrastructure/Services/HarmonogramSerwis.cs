using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class HarmonogramSerwis : IHarmonogramSerwis
{
    private readonly FleetDbContext _context;

    public HarmonogramSerwis(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<HarmonogramPrzegladow?> PobierzDlaPojazduAsync(int pojazdId)
    {
        return await _context.Harmonogramy
            .FirstOrDefaultAsync(h => h.PojazdId == pojazdId);
    }

    public async Task UstawHarmonogramAsync(HarmonogramPrzegladow h)
    {
        var istniejacy = await _context.Harmonogramy
            .FirstOrDefaultAsync(x => x.PojazdId == h.PojazdId);

        if (istniejacy != null)
        {
            //Aktualizacja
            istniejacy.InterwalKm = h.InterwalKm;
            istniejacy.InterwalDni = h.InterwalDni;
            istniejacy.DataOstatniegoPrzegladu = h.DataOstatniegoPrzegladu;
            istniejacy.PrzebiegOstatniegoPrzegladu = h.PrzebiegOstatniegoPrzegladu;
        }
        else
        {
            //Nowy wpis
            _context.Harmonogramy.Add(h);
        }

        await _context.SaveChangesAsync();
    }
}
