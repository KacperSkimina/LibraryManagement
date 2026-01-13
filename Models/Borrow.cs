using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    // Klasa reprezentująca wypożyczenie książki – tabela łącząca Książkę i Czytelnika
    public class Borrow
    {
        // Klucz główny wypożyczenia
        [Key]
        public int Id { get; set; }

        // Pole wymagane – identyfikator wypożyczanej książki
        [Required(ErrorMessage = "Książka jest wymagana")]
        [Display(Name = "Książka")]
        public int BookId { get; set; }

        // Właściwość nawigacyjna do pełnego obiektu Książki
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;

        // Pole wymagane – identyfikator czytelnika (Patrona)
        [Required(ErrorMessage = "Czytelnik jest wymagany")]
        [Display(Name = "Czytelnik")]
        public int PatronId { get; set; }

        // Właściwość nawigacyjna do pełnego obiektu Czytelnika
        [ForeignKey("PatronId")]
        public Patron Patron { get; set; } = null!;

        // Data dokonania wypożyczenia
        [DataType(DataType.Date)]
        [Display(Name = "Data wypożyczenia")]
        public DateTime BorrowDate { get; set; }

        // Termin zwrotu – pole wymagane
        [Required(ErrorMessage = "Termin zwrotu jest wymagany")]
        [DataType(DataType.Date)]
        [Display(Name = "Termin zwrotu")]
        // Własna walidacja sprawdzająca, czy data nie jest z przeszłości
        [FutureDate(ErrorMessage = "Termin zwrotu musi być datą przyszłą")]
        public DateTime DueDate { get; set; }

        // Data faktycznego zwrotu – znak '?' oznacza, że pole może być puste (książka jeszcze nie wróciła)
        [DataType(DataType.Date)]
        [Display(Name = "Data zwrotu")]
        public DateTime? ReturnDate { get; set; }

        // Logiczna właściwość sprawdzająca czy książka została oddana
        // Nie tworzy kolumny w bazie, obliczana na podstawie obecności ReturnDate
        [Display(Name = "Czy zwrócono")]
        public bool IsReturned => ReturnDate != null;
    }

    // Własny atrybut walidacji danych (Custom Validation)
    public class FutureDateAttribute : ValidationAttribute
    {
        // Metoda sprawdzająca poprawność wprowadzonej daty
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                // Sprawdzenie czy data jest wcześniejsza lub równa dzisiejszej
                if (date.Date <= DateTime.Now.Date)
                {
                    // Zwrócenie błędu walidacji
                    return new ValidationResult(ErrorMessage ?? "Data musi być w przyszłości");
                }
            }
            // Jeśli wszystko jest w porządku, walidacja kończy się sukcesem
            return ValidationResult.Success!;
        }
    }
}