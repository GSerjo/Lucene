using System;
using System.Collections.Generic;
using System.Linq;
using LuceneSearch;
using LuceneSearch.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UnitTests
{
    public sealed class ForTests
    {
        [Fact]
        public void Search_AbsentFirstName_False()
        {
            var personRepository = new PersonRepository();
            var search = new Search();
            search.AddOrUpdateIndex(personRepository.GetAll().ToArray());
            string searchFirstName = Guid.NewGuid().ToString();

            List<Person> result = search.SearchPersons(searchFirstName);

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void Search_ExistsFirstName_True()
        {
            var personRepository = new PersonRepository();
            var search = new Search();
            search.AddOrUpdateIndex(personRepository.GetAll().ToArray());
            string searchFirstName = "Mike";

            List<Person> result = search.SearchPersons(searchFirstName);

            Assert.True(result.Count == 1);
            Assert.Equal(searchFirstName, result.Single().FirstName);
        }

        [Fact]
        public void Search_NewPerson_True()
        {
            var personRepository = new PersonRepository();
            var search = new Search();
            search.AddOrUpdateIndex(personRepository.GetAll().ToArray());
            string searchFirstName = Guid.NewGuid().ToString();

            List<Person> result = search.SearchPersons(searchFirstName);

            Assert.True(result.Count == 0);

            search.AddOrUpdateIndex(new Person { Id = 100, FirstName = searchFirstName, LastName = "NewLastName" });

            result = search.SearchPersons(searchFirstName);
            Assert.True(result.Count == 1);
            Assert.Equal(searchFirstName, result.Single().FirstName);
        }

        [Fact]
        public void Test()
        {
            var person = new Person { Id = 0, FirstName = "Mike", LastName = "Lee Loo" };
            string json = JsonConvert.SerializeObject(person);

            JObject rawJson = JObject.Parse(json);
            List<string> properties = rawJson.Properties().Select(x => x.Name).ToList();
            properties.ForEach(x => Console.WriteLine(x));
        }
    }
}
