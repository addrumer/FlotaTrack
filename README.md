# FlotaTrack - System Zarządzania Flotą Pojazdów

**FlotaTrack** to nowoczesna, responsywna aplikacja webowa do zarządzania flotą pojazdów służbowych, ewidencjonowania tankowań, przydzielania kierowców oraz planowania przeglądów okresowych i napraw. Aplikacja została zbudowana w oparciu o technologię **ASP.NET Core Blazor (InteractiveServer)** i **.NET 10.0**.

---

## Stos Technologiczny

* **Backend / Logika**: C# 15 / .NET 10.0
* **Frontend**: Blazor InteractiveServer, Razor Components
* **Baza danych & ORM**: MS SQL Server (LocalDB), Entity Framework Core 10.0 (Code-First)
* **Stylizacja / UI**: Bootstrap 5 + dedykowany arkusz stylów CSS (z pełną obsługą zmiennych CSS, responsywnym menu bocznym oraz stylem dopasowanym do wydruku).

---

## Główne Funkcjonalności

1. **Panel Główny (Dashboard)**:
   * Wyświetlanie kluczowych liczników (pojazdy dostępne, w trasie, w serwisie).
   * System alertów informujący o wygasających w ciągu 30 dni polisach ubezpieczeniowych oraz pojazdach wymagających przeglądu okresowego.
   * Ostatnie 5 aktywności (wydania i zwroty).
   * Skróty do szybkich akcji.

2. **Baza Pojazdów**:
   * Ewidencja pojazdów (marka, model, rejestracja, rok produkcji, pojemność baku, aktualny przebieg).
   * Wyszukiwanie pojazdów w czasie rzeczywistym.
   * Bezpieczne logiczne usuwanie pojazdów (status `Wyłączony` uniemożliwiający dalsze przydzielanie).

3. **Karta i Szczegóły Pojazdu**:
   * Wizualny wskaźnik zużycia do następnego przeglądu (pasek postępu obliczany na podstawie limitów dni i przejechanych km).
   * Formularz konfiguracji harmonogramu przeglądów (interwał km, interwal dni, dane ostatniego przeglądu).
   * Historia przydzielonych kierowców, rejestr tankowań, polisy ubezpieczeniowe oraz naprawy powiązane z konkretnym autem.

4. **Kartoteka Kierowców**:
   * Zarządzanie danymi pracowników (PESEL, prawo jazdy, dane kontaktowe, doświadczenie).
   * Wyszukiwanie kierowców po nazwisku, prawie jazdy lub numerze PESEL.
   * **Karta Kierowcy (Modal)**: szczegółowy podgląd historii tras, sumarycznych wydatków na paliwo oraz historii tankowań.
   * Zabezpieczenie przed usunięciem kierowcy, który aktualnie jedzie pojazdem (status `W trasie`).

5. **Dyspozytornia (Przydziały)**:
   * System wydawania pojazdów dostępnych dla wolnych kierowców (walidacja blokująca wydanie zajętemu kierowcy lub niesprawnemu autu).
   * Rejestrowanie zwrotów wraz z aktualizacją stanu licznika i powrotem auta do puli dostępnych.
   * Generowanie i drukowanie dedykowanego **Protokołu Przekazania Pojazdu** (wsparcie stylów `@media print`).

6. **Ewidencja Tankowań**:
   * Rejestrowanie ilości paliwa, ceny za litr, przebiegu podczas tankowania oraz oznaczanie tankowań "do pełna".
   * Automatyczna aktualizacja przebiegu pojazdu w bazie głównej na podstawie wpisu z tankowania.
   * Lista tankowań z wyszukiwarką pojazdów.

7. **Centrum Napraw i Serwisu**:
   * Spisy zgłoszeń serwisowych z podziałem na awarie, eksploatację i wymianę opon.
   * Automatyczna synchronizacja statusów: zgłoszenie serwisowe typu `W Trakcie` blokuje możliwość wydania pojazdu i zmienia jego status na `WSerwisie`. Zakończenie naprawy przywraca status `Dostepny`.
   * Obsługa wyjątków i komunikatów o błędach biznesowych bezpośrednio na interfejsie.

8. **Raporty finansowe i analityka**:
   * Łączne koszty floty (paliwo + serwis).
   * Średni koszt utrzymania pojedynczego auta.
   * Zestawienie TOP 5 najdroższych pojazdów w utrzymaniu wraz z podziałem na koszty eksploatacyjne i paliwo.
   * Struktura floty w postaci wykresów procentowych.

---

## Architektura Projektu

Projekt podzielony jest na logiczne foldery:
* **[Domain/](file:///D:/Projekty/Programowanie_Windows/Flota/Flota/Domain)**: Klasy domenowe (encje bazodanowe), typy wyliczeniowe (Enums) oraz interfejsy serwisów biznesowych.
* **[Infrastructure/](file:///D:/Projekty/Programowanie_Windows/Flota/Flota/Infrastructure)**: Warstwa danych (Entity Framework Core, DbContext, inicjalizacja bazy danych) oraz implementacja logiki biznesowej (podział na niezależne serwisy: `PojazdSerwis`, `KierowcaSerwis`, `TankowanieSerwis`, `UbezpieczenieSerwis`, `SerwisPojazdu`, `PrzydzialSerwis`, `HarmonogramSerwis`).
* **[Components/](file:///D:/Projekty/Programowanie_Windows/Flota/Flota/Components)**: Komponenty widoków Razor Pages i szablony Layout aplikacji.
* **[wwwroot/](file:///D:/Projekty/Programowanie_Windows/Flota/Flota/wwwroot)**: Pliki statyczne, favicony oraz główny arkusz stylów `app.css`.

---

## Pierwsze Uruchomienie (Seeding i Migracje)

### 1. Wymagania wstępne
* Zainstalowane SDK .NET 10.
* Lokalna instancja bazy danych LocalDB zainstalowana wraz z Visual Studio.

### 2. Konfiguracja połączenia
Connection string zdefiniowany jest w pliku [appsettings.json](file:///D:/Projekty/Programowanie_Windows/Flota/Flota/appsettings.json). Domyślnie aplikacja łączy się z bazą danych LocalDB:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FlotaDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. Automatyczne zasilenie danymi testowymi (Seed Data)
Aplikacja została wyposażona w klasę [DbInitializer.cs](file:///D:/Projekty/Programowanie_Windows/Flota/Flota/Infrastructure/Data/DbInitializer.cs). Przy pierwszym uruchomieniu aplikacja:
1. Automatycznie utworzy bazę danych (jeśli nie istnieje) i zaaplikuje wszystkie migracje.
2. Jeśli baza jest pusta, automatycznie zasili ją **bogatym zestawem danych testowych**:
   * Przykładowi kierowcy (w tym dane PESEL, kontakt i staż),
   * Przykładowe pojazdy z różnymi statusami (Dostępne, W trasie, W serwisie),
   * Aktywne i historyczne przydziały,
   * Historia tankowań, przeglądów okresowych i aktywne naprawy (generujące alarmy i alerty na pulpicie głównym).

### 4. Uruchomienie aplikacji z poziomu terminala
Przejdź do folderu projektu [Flota/](file:///D:/Projekty/Programowanie_Windows/Flota/Flota) i uruchom komendę:
```bash
dotnet run
```
Aplikacja uruchomi się i będzie dostępna pod domyślnym adresem URL wskazanym w konsoli (zazwyczaj `https://localhost:5001` lub `http://localhost:5000`).

---

## Przydatne polecenia deweloperskie

Aplikacja wykorzystuje mechanizm EF Core Migrations. Poniższe polecenia należy uruchamiać w folderze [Flota/](file:///D:/Projekty/Programowanie_Windows/Flota/Flota):

* **Kompilacja projektu**:
  ```bash
  dotnet build
  ```
* **Dodanie nowej migracji po zmianie modelu**:
  ```bash
  dotnet ef migrations add NazwaMigracji
  ```
* **Ręczna aktualizacja schematu bazy danych**:
  ```bash
  dotnet ef database update
  ```
* **Usunięcie bazy danych (np. do testów seederów)**:
  ```bash
  dotnet ef database drop -f
  ```
