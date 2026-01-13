using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    // Klasa reprezentująca encję (tabelę) Książka w bazie danych
    public class Book
    {
        // Klucz główny - unikalne ID każdej książki
        [Key]
        public int Id { get; set; }

        // Pole wymagane - Tytuł książki
        [Required(ErrorMessage = "Tytuł jest wymagany")]
        // Walidacja długości tekstu (od 3 do 200 znaków)
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tytuł musi mieć 3-200 znaków")]
        // Nazwa wyświetlana na etykiecie w formularzu
        [Display(Name = "Tytuł")]
        public string Title { get; set; } = string.Empty;

        // Pole wymagane - Numer ISBN
        [Required(ErrorMessage = "ISBN jest wymagany")]
        // Wyrażenie regularne sprawdzające, czy wpisano dokładnie 13 cyfr
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ISBN musi mieć dokładnie 13 cyfr")]
        [Display(Name = "Numer ISBN")]
        public string ISBN { get; set; } = string.Empty;

        // Walidacja zakresu - liczba stron nie może być zerem lub liczbą ujemną
        [Range(1, int.MaxValue, ErrorMessage = "Liczba stron musi być większa od 0")]
        [Display(Name = "Liczba stron")]
        public int PageCount { get; set; }

        // Określenie typu danych jako Data (wyświetli kalendarz w przeglądarce)
        [DataType(DataType.Date)]
        [Display(Name = "Data publikacji")]
        public DateTime PublicationDate { get; set; }

        // Klucz obcy do tabeli Autorzy
        [Display(Name = "Autor")]
        public int AuthorId { get; set; }

        // Definicja powiązania klucza obcego z właściwością nawigacyjną Author
        [ForeignKey("AuthorId")]
        // Atrybut zapobiegający walidacji całego obiektu Autora podczas dodawania samej książki
        [ValidateNever]
        public Author Author { get; set; } = null!;

        // Kolekcja wypożyczeń - relacja jeden-do-wielu (jedna książka może mieć wiele rekordów wypożyczeń)
        public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
    }
}