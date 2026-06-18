using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class SerwisPojazdu : ISerwisPojazdu
{
    private readonly FleetDbContext _context;

    public SerwisPojazdu(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<List<WpisSerwisowy>> PobierzWszystkieAsync()
    {
        return await _context.WpisSerwisowy
            .Include(z => z.Pojazd)
            .OrderByDescending(z => z.DataZgloszenia)
            .ToListAsync();
    }

    public async Task DodajAsync(WpisSerwisowy z)
    {
        //1. Walidacja: Jeśli próbujemy dodać serwis "W Trakcie", sprawdzamy czy auto już nie jest w serwisie
        if (z.Status == "W Trakcie")
        {
            bool czyJuzWSerwisie = await _context.WpisSerwisowy
                .AnyAsync(x => x.PojazdId == z.PojazdId && x.Status == "W Trakcie");

            if (czyJuzWSerwisie)
            {
                throw new Exception(
                    "To auto ma już aktywny serwis 'W Trakcie'! Zakończ poprzedni, zanim rozpoczniesz nowy.");
            }
        }

        //2. Pobieramy pojazd i zmieniamy mu status
        var pojazd = await _context.Pojazdy.FindAsync(z.PojazdId);

        if (pojazd != null && z.Status == "W Trakcie")
        {
            pojazd.Status = Domain.Enums.StatusPojazdu.WSerwisie;
        }

        _context.WpisSerwisowy.Add(z);
        await _context.SaveChangesAsync();
    }

    public async Task ZmienStatusAsync(int id, string nowyStatus)
    {
        //1. Pobieramy zgłoszenie wraz z pojazdem
        var zgloszenie = await _context.WpisSerwisowy
            .Include(z => z.Pojazd)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zgloszenie == null) return;

        //2. WALIDACJA
        // Jeśli próbujemy ustawić status na "W Trakcie", sprawdzamy inne zgłoszenia
        if (nowyStatus == "W Trakcie")
        {
            bool czyInnySerwisTrwa = await _context.WpisSerwisowy
                .AnyAsync(x => x.PojazdId == zgloszenie.PojazdId
                               && x.Id != id
                               && x.Status == "W Trakcie");

            if (czyInnySerwisTrwa)
            {
                throw new Exception("Nie można rozpocząć tego serwisu! To auto jest już w trakcie innej naprawy.");
            }
        }

        //3. Aktualizacja statusu zgłoszenia
        zgloszenie.Status = nowyStatus;

        //4. Automatyczna zmiana statusu POJAZDU
        if (zgloszenie.Pojazd != null)
        {
            if (nowyStatus == "Zakończone")
            {
                zgloszenie.Pojazd.Status = Domain.Enums.StatusPojazdu.Dostepny;
            }
            else if (nowyStatus == "W Trakcie")
            {
                zgloszenie.Pojazd.Status = Domain.Enums.StatusPojazdu.WSerwisie;
            }
        }

        await _context.SaveChangesAsync();
    }
}
