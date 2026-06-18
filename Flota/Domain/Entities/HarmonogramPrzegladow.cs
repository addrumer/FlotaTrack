using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flota.Domain.Entities;

public class HarmonogramPrzegladow
{
    [Key]
    public int Id { get; set; }

    public int PojazdId { get; set; }
    public virtual Pojazd Pojazd { get; set; } = null!;

    public int InterwalKm { get; set; }

    public int InterwalDni { get; set; }

    public DateTime DataOstatniegoPrzegladu { get; set; }

    [Column(TypeName = "decimal(12,1)")]
    public decimal PrzebiegOstatniegoPrzegladu { get; set; }
    
    public bool CzyWymaganyPrzeglad(decimal aktualnyPrzebieg)
    {
        var dniMinely = (DateTime.Now - DataOstatniegoPrzegladu).TotalDays >= InterwalDni;
        var kmMinely = (aktualnyPrzebieg - PrzebiegOstatniegoPrzegladu) >= InterwalKm;

        return dniMinely || kmMinely;
    }
}