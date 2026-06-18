using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Flota.Domain.Entities;
using Flota.Domain.Enums;

namespace Flota.Infrastructure.Data;

public static class DbInitializer 
{
    public static void Initialize(FleetDbContext context)
    {
        context.Database.Migrate();

        if (context.Pojazdy.Any())
        {
            return; // Baza już ma dane
        }

        // 1. Kierowcy
        var kierowca1 = new Kierowca { Imie = "Jan", Nazwisko = "Kowalski", NumerPrawaJazdy = "12345/09/11", Email = "jan.kowalski@flotatrack.pl", Telefon = "+48 501 202 303", Adres = "Toruńska 5, Bydgoszcz", Pesel = "85030212345", LataDoswiadczenia = 10 };
        var kierowca2 = new Kierowca { Imie = "Adam", Nazwisko = "Nowak", NumerPrawaJazdy = "54321/11/15", Email = "adam.nowak@flotatrack.pl", Telefon = "+48 602 303 404", Adres = "Warszawska 12, Toruń", Pesel = "90050854321", LataDoswiadczenia = 5 };
        var kierowca3 = new Kierowca { Imie = "Anna", Nazwisko = "Wiśniewska", NumerPrawaJazdy = "99999/19/22", Email = "anna.w@flotatrack.pl", Telefon = "+48 703 404 505", Adres = "Gdańska 1, Gdynia", Pesel = "98111299999", LataDoswiadczenia = 3 };

        context.Kierowcy.AddRange(kierowca1, kierowca2, kierowca3);
        context.SaveChanges(); // Zapisujemy kierowców by otrzymać ich Id

        // 2. Pojazdy
        var pojazd1 = new Pojazd { Marka = "Toyota", Model = "Corolla", NumerRejestracyjny = "CB 12345", RokProdukcji = 2022, Przebieg = 45000, PojemnoscZbiornika = 50, Status = StatusPojazdu.Dostepny };
        var pojazd2 = new Pojazd { Marka = "Skoda", Model = "Octavia", NumerRejestracyjny = "WY 98765", RokProdukcji = 2021, Przebieg = 120000, PojemnoscZbiornika = 55, Status = StatusPojazdu.WUzytkowaniu };
        var pojazd3 = new Pojazd { Marka = "Ford", Model = "Transit", NumerRejestracyjny = "KR 55555", RokProdukcji = 2020, Przebieg = 185000, PojemnoscZbiornika = 80, Status = StatusPojazdu.WSerwisie };
        var pojazd4 = new Pojazd { Marka = "Opel", Model = "Astra", NumerRejestracyjny = "GD 11111", RokProdukcji = 2019, Przebieg = 95000, PojemnoscZbiornika = 52, Status = StatusPojazdu.Dostepny };

        context.Pojazdy.AddRange(pojazd1, pojazd2, pojazd3, pojazd4);
        context.SaveChanges(); // Zapisujemy pojazdy

        // 3. Tankowania
        var tankowanie1 = new Tankowanie { PojazdId = pojazd1.Id, KierowcaId = kierowca1.Id, Data = DateTime.Now.AddDays(-2), IloscLitrow = 42, CenaZaLitr = 6.20m, LacznyKoszt = 260.40m, CzyDoPelna = true, Przebieg = 44800 };
        var tankowanie2 = new Tankowanie { PojazdId = pojazd2.Id, KierowcaId = kierowca2.Id, Data = DateTime.Now.AddDays(-1), IloscLitrow = 50, CenaZaLitr = 6.15m, LacznyKoszt = 307.50m, CzyDoPelna = true, Przebieg = 119950 };

        context.Tankowania.AddRange(tankowanie1, tankowanie2);

        // 4. Przydziały
        var przydzialAktywny = new Przydzial { PojazdId = pojazd2.Id, KierowcaId = kierowca2.Id, DataRozpoczecia = DateTime.Now.AddDays(-3), PrzebiegPoczatkowy = 119800, DataZakonczenia = null, PrzebiegKoncowy = null };
        var przydzialHistoryczny = new Przydzial { PojazdId = pojazd4.Id, KierowcaId = kierowca1.Id, DataRozpoczecia = DateTime.Now.AddDays(-10), PrzebiegPoczatkowy = 94200, DataZakonczenia = DateTime.Now.AddDays(-5), PrzebiegKoncowy = 95000 };

        context.Przydzialy.AddRange(przydzialAktywny, przydzialHistoryczny);

        // 5. Harmonogramy Przeglądów
        var harmonogram1 = new HarmonogramPrzegladow { PojazdId = pojazd1.Id, InterwalKm = 15000, InterwalDni = 365, DataOstatniegoPrzegladu = DateTime.Now.AddMonths(-5), PrzebiegOstatniegoPrzegladu = 35000 };
        var harmonogram2 = new HarmonogramPrzegladow { PojazdId = pojazd2.Id, InterwalKm = 20000, InterwalDni = 365, DataOstatniegoPrzegladu = DateTime.Now.AddMonths(-11), PrzebiegOstatniegoPrzegladu = 100000 };
        var harmonogram3 = new HarmonogramPrzegladow { PojazdId = pojazd3.Id, InterwalKm = 15000, InterwalDni = 365, DataOstatniegoPrzegladu = DateTime.Now.AddMonths(-1), PrzebiegOstatniegoPrzegladu = 180000 };

        context.Harmonogramy.AddRange(harmonogram1, harmonogram2, harmonogram3);

        // 6. Ubezpieczenia
        var ubezpieczenie1 = new Ubezpieczenie { PojazdId = pojazd1.Id, NumerPolisy = "POL-99128", Ubezpieczyciel = "PZU S.A.", DataRozpoczecia = DateTime.Now.AddMonths(-6), DataZakonczenia = DateTime.Now.AddMonths(6), Koszt = 1200m };
        var ubezpieczenie2 = new Ubezpieczenie { PojazdId = pojazd2.Id, NumerPolisy = "POL-00234", Ubezpieczyciel = "Warta", DataRozpoczecia = DateTime.Now.AddMonths(-11), DataZakonczenia = DateTime.Now.AddDays(15), Koszt = 1500m };

        context.Ubezpieczenia.AddRange(ubezpieczenie1, ubezpieczenie2);

        // 7. Wpisy Serwisowe
        var serwis1 = new WpisSerwisowy { PojazdId = pojazd3.Id, Opis = "Wymiana klocków hamulcowych i tarcz z przodu", Koszt = 1200m, NazwaWarsztatu = "AutoSerwis Bydgoszcz", DataZgloszenia = DateTime.Now.AddDays(-2), Status = "W Trakcie", TypNaprawy = TypNaprawy.Eksploatacja };
        var serwis2 = new WpisSerwisowy { PojazdId = pojazd4.Id, Opis = "Wymiana filtrów i oleju silnikowego", Koszt = 450m, NazwaWarsztatu = "Warsztat Toruń", DataZgloszenia = DateTime.Now.AddDays(-10), Status = "Zakończone", TypNaprawy = TypNaprawy.Eksploatacja };

        context.WpisSerwisowy.AddRange(serwis1, serwis2);

        context.SaveChanges();
    }
}