using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    // Klasa reprezentująca Czytelnika (Patrona) biblioteki
    public class Patron
    {
        // Klucz główny – unikalny identyfikator czytelnika
        public int Id { get; set; }

        // Imię czytelnika – pole obowiązkowe
        [Required(ErrorMessage = "Imię jest wymagane")]
        [StringLength(50)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        // Nazwisko czytelnika – pole obowiązkowe
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [StringLength(50)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        // Adres e-mail – wymagany i sprawdzany pod kątem poprawnego formatu
        [Required(ErrorMessage = "Adres e-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Błędny format adresu e-mail")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        // Numer telefonu – pole opcjonalne (znak '?' przy string)
        [Phone(ErrorMessage = "Błędny format numeru telefonu")]
        [Display(Name = "Numer telefonu")]
        public string? PhoneNumber { get; set; }

        // Relacja: Jeden czytelnik może mieć wiele wpisów o wypożyczeniach (1:N)
        public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();

        // Właściwość wyliczalna łącząca imię i nazwisko
        // Bardzo pomocna przy wyświetlaniu list wyboru (np. komu wypożyczamy książkę)
        [Display(Name = "Imię i nazwisko")]
        public string FullName => $"{FirstName} {LastName}";
    }
}