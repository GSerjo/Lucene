using System;
using System.Collections.Generic;

namespace LuceneSearch.Domain
{
    public class PersonRepository
    {
        public List<Person> GetAll()
        {
            return new List<Person>
            {
                new Person { Id = 0, FirstName = "Mike", LastName = "Lee" },
                new Person { Id = 1, FirstName = "John", LastName = "Lee" },
                new Person { Id = 1, FirstName = "Ted", LastName = "Loo" }
            };
        }
    }
}
