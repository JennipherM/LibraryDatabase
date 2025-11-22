using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LibraryActivity
{
    internal class Book
    {
        private string title;
        private string author;
        private string genre;
        private bool isAvailable;

        public Book()
        {
            title = "N/A";
            author = "N/A";
            genre = "N/A";
            isAvailable = true;
        }
        public Book(string Title, string Author, string Genre, bool IsAvailable)
        {
            title = Title;
            author = Author;
            genre = Genre;
            isAvailable = IsAvailable;
        }

        public string Title
        {
            get { return title; }
        }
        public string Author
        {
            get { return author; }
        }
        public string Genre
        {
            get { return genre; }
        }
        public bool IsAvailable
        {
            get { return isAvailable; }
            set { isAvailable = value; }
        }

        public void checkOut()
        {
            if (isAvailable)
            {
                isAvailable = false;
                return;
            }
            Console.WriteLine("Book not available");
        }
        public void checkIn()
        {
            if (!isAvailable)
            {
                isAvailable = true;
                return;
            }
            Console.WriteLine("Book already checked in");
        }

    }

    class TranslatedBook : Book 
    {
        private string origLanguage;
        private List<string> languages; 

        public TranslatedBook(string Title, string Author, string Genre, bool IsAvailable, string OrgLanguage) : base (Title, Author, Genre, false)
        {
            origLanguage = OrgLanguage;
            languages = new List<string>();
        }      

        public void addTranslation(List<string> languageList)
        {
            languages = languageList;
        }

        public void showLanguages()
        {
            Console.WriteLine("Available languages: ");

            foreach (string language in languages)
            {
                Console.Write($"{language}, ");
            }
            return;
        }
    }

    class ChildrensBook : Book
    {
        public string ageRange;
        public bool hasIllustrations;
        public string illustratorName;
        public bool isInteractive;

        public ChildrensBook(string Title, string Author, string Genre, bool IsAvailable, string AgeRange, bool HasIllustrations, bool interactive) : base(Title, Author, Genre, false)
        {
            ageRange = AgeRange;
            hasIllustrations = HasIllustrations;
            isInteractive = false;
            illustratorName = "N/A";
        }

        public void addIllustrator(string name)
        {
            hasIllustrations = true;
            illustratorName = name;
        }
        public void displayIllustrator()
        {
            if (!hasIllustrations)
            {
                Console.WriteLine("No illustrator");
                return;
            }
            Console.WriteLine(illustratorName);
        }
    }

    class AudioBook : Book
    {
        public string duration;
        public string narratorName;
        public string format;
        public string summary;

        public AudioBook(string Title, string Author, string Genre, bool IsAvailable, string Duration, string NarratorName, string Format) : base(Title, Author, Genre, false)
        {
            duration = Duration;
            narratorName = NarratorName;
            format = Format;
            summary = "N/A";
        }

        public void addSummary(string Summary)
        {
            summary = Summary;
            Console.WriteLine("Summary added!");
        }

        public void displaySummary()
        {
            if(summary == "N/A")
            {
                Console.WriteLine("No summary available");
                return;
            }
            Console.WriteLine(summary);
        }
    }
}
