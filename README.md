# 📚 System Zarządzania Biblioteką (Library Management System)

Prosta i funkcjonalna aplikacja webowa do zarządzania zasobami bibliotecznymi, zbudowana w technologii **ASP.NET Core MVC**. Projekt umożliwia ewidencję książek, autorów oraz czytelników, a także proces wypożyczania i zwrotów.

## 🚀 Główne Funkcjonalności

* **Zarządzanie Książkami:** Pełny system CRUD (dodawanie, wyświetlanie, edycja, usuwanie).
* **Katalog Autorów:** Przeglądanie autorów wraz z automatycznie zliczaną liczbą ich dzieł.
* **System Wypożyczeń:** Możliwość przypisywania książek do czytelników z automatycznym wyliczaniem terminu zwrotu.
* **Monitorowanie Przeterminowań:** System wizualnie ostrzega o książkach, których termin zwrotu minął.
* **Bezpieczeństwo (Identity):** Podział na Role (Administrator może zarządzać danymi, użytkownik tylko przeglądać).
* **Testowanie API:** Wbudowany endpoint JSON oraz widok testowy z wykorzystaniem JavaScript (Fetch API).

## 🛠️ Technologie

* **Backend:** .NET 8.0 / ASP.NET Core MVC
* **Baza danych:** Entity Framework Core z SQLite
* **Frontend:** Bootstrap 5, JavaScript (Fetch API)
* **Bezpieczeństwo:** ASP.NET Core Identity

## 📦 Inicjalizacja Danych (Seed Data)

Aplikacja posiada wbudowany mechanizm **Seed Data** w pliku `Program.cs`. Przy pierwszym uruchomieniu system automatycznie:
1.  Stworzy bazę danych SQLite.
2.  Wygeneruje tabele (Migrations).
3.  Doda rolę `Administrator` oraz konto:
    * **Login:** `admin@biblioteka.pl`
    * **Hasło:** `Admin123!`
4.  Wypełni bazę przykładowymi autorami (m.in. Sapkowski, Tokarczuk) i książkami.

## ⚙️ Jak uruchomić?

1.  Upewnij się, że masz zainstalowane **.NET 8 SDK**.
2.  Sklonuj repozytorium: `git clone [link-do-twojego-repo]`
3.  W folderze projektu wykonaj: `dotnet run`
4.  Otwórz przeglądarkę pod adresem: `https://localhost:5001` (lub wskazanym w konsoli).