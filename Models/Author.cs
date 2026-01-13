using LibraryManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    // Klasa reprezentująca tabelę Autorzy w bazie danych
    public class Author
    {
        // Klucz główny - unikalny identyfikator autora w bazie danych
        public int Id { get; set; }

        // Imię autora - pole wymagane z komunikatem po polsku
        [Required(ErrorMessage = "Imię jest wymagane")]
        // Ograniczenie długości tekstu w bazie do 50 znaków
        [StringLength(50)]
        // Nazwa wyświetlana w etykietach (labelach) na widoku
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        // Nazwisko autora - pole wymagane
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        // Data urodzenia - typ DataType.Date wymusza kalendarz w przeglądarce
        [DataType(DataType.Date)]
        [Display(Name = "Data urodzenia")]
        // Znak '?' oznacza, że pole może być puste (null) w bazie
        public DateTime? BirthDate { get; set; }

        // Opcjonalna krótka biografia autora
        [StringLength(500, ErrorMessage = "Biografia może mieć maksymalnie 500 znaków")]
        [Display(Name = "Biografia")]
        public string? Biography { get; set; }

        // Relacja: Jeden autor może mieć przypisanych wiele książek (1:N)
        // ICollection pozwala na dostęp do listy książek danego autora
        public ICollection<Book> Books { get; set; } = new List<Book>();

        // Właściwość wyliczalna (nie tworzy kolumny w bazie)
        // Łączy imię i nazwisko w jeden ciąg znaków, wygodne do list rozwijanych
        [Display(Name = "Autor")]
        public string FullName => $"{FirstName} {LastName}";
    }
}