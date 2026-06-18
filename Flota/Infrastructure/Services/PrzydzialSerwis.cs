using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Interfaces;
using Flota.Infrastructure.Data;

namespace Flota.Infrastructure.Services;

public class PrzydzialSerwis : IPrzydzialSerwis
{
    private readonly FleetDbContext _context;

    public PrzydzialSerwis(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<List<Przydzial>> PobierzAktywneAsync()
    {
        return await _context.Przydzialy
            .Include(p => p.Pojazd).Include(p => p.Kierowca)
            .Where(p => p.DataZakonczenia == null) //Tylko te, które trwają
            .ToListAsync();
    }

    public async Task<List<Przydzial>> PobierzHistorieAsync()
    {
        return await _context.Przydzialy
            .Include(p => p.Pojazd).Include(p => p.Kierowca)
            .Where(p => p.DataZakonczenia != null)
            .OrderByDescending(p => p.DataZakonczenia)
            .ToListAsync();
    }

    public async Task WydajPojazdAsync(Przydzial p)
    {
        //1. SPRAWDZENIE: Czy kierowca nie ma już aktywnego pojazdu?
        bool kierowcaZajety = await _context.Przydzialy
            .AnyAsync(x => x.KierowcaId == p.KierowcaId && x.DataZakonczenia == null);

        if (kierowcaZajety)
        {
            throw new Exception("Ten kierowca ma już przypisany aktywny pojazd! Najpierw zwróć poprzedni.");
        }

        //2. SPRAWDZENIE: Czy pojazd na pewno jest dostępny?
        var pojazd = await _context.Pojazdy.FindAsync(p.PojazdId);
        if (pojazd == null) throw new Exception("Pojazd nie istnieje");

        //To blokuje wydanie auta będącego w serwisie lub u innego kierowcy
        if (pojazd.Status != Domain.Enums.StatusPojazdu.Dostepny)
        {
            throw new Exception($"Pojazd nie jest dostępny! Jego status to: {pojazd.Status}");
        }

        p.DataRozpoczecia = DateTime.Now;
        p.PrzebiegPoczatkowy = pojazd.Przebieg;
        p.DataZakonczenia = null;

        pojazd.Status = Domain.Enums.StatusPojazdu.WUzytkowaniu;

        _context.Przydzialy.Add(p);
        await _context.SaveChangesAsync();
    }

    public async Task ZwrocPojazdAsync(int przydzialId, decimal przebiegKoncowy, DateTime dataZwrotu)
    {
        var przydzial = await _context.Przydzialy.Include(p => p.Pojazd)
            .FirstOrDefaultAsync(p => p.Id == przydzialId);
        if (przydzial == null) throw new Exception("Nie znaleziono przydziału");

        //1. Zamknij przydział
        przydzial.DataZakonczenia = dataZwrotu;
        przydzial.PrzebiegKoncowy = przebiegKoncowy;

        //2. Zaktualizuj przebieg pojazdu w bazie głównej
        if (przydzial.Pojazd != null)
        {
            przydzial.Pojazd.Przebieg = przebiegKoncowy;
            przydzial.Pojazd.Status = Domain.Enums.StatusPojazdu.Dostepny; //Pojazd wraca do puli
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Przydzial?> PobierzPoIdAsync(int id)
    {
        return await _context.Przydzialy
            .Include(p => p.Pojazd)
            .Include(p => p.Kierowca)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
