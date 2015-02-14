using System;
using System.Collections.Generic;

namespace LuceneSearch.Domain
{
    public sealed class PersonRepository
    {
        public List<Person> GetAll()
        {
            return new List<Person>
            {
                new Person { Id = 0, FirstName = "Mike", LastName = "Lee Loo" },
                new Person { Id = 0, FirstName = "Mike1", LastName = "Lee Loo1" },
                new Person { Id = 1, FirstName = "John", LastName = "Lee" },
                new Person { Id = 2, FirstName = "Ted", LastName = "Loo" }
            };
        }
    }
}
