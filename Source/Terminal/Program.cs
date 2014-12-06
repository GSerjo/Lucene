using System;
using System.Collections.Generic;
using LuceneSearch;
using LuceneSearch.Domain;

namespace Terminal
{
    internal class Program
    {
        private static readonly Search _search = new Search();

        private static void Main()
        {
            var personRepository = new PersonRepository();

            _search.ClearIndex();
            _search.AddOrUpdateIndex(personRepository.GetAll().ToArray());

            ConsoleKey consoleKey;
            do
            {
                Console.WriteLine("Enter query: FirstName, LastName or Id");
                string query = Console.ReadLine();

                List<Person> persons = _search.SearchPersons(query);
                PrintSearchResult(persons);

                Console.WriteLine("Press any key to continue");
                consoleKey = Console.ReadKey().Key;
            }
            while (consoleKey != ConsoleKey.Escape);
        }

        private static void PrintSearchResult(List<Person> persons)
        {
            if (persons.Count == 0)
            {
                Console.WriteLine("Search returned nothing");
                return;
            }
            Console.WriteLine("Search result:");
            persons.ForEach(x =>
            {
                Console.WriteLine(x);
                Console.WriteLine(Environment.NewLine);
            });
        }
    }
}
