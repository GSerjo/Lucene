using System;

namespace LuceneSearch.Domain
{
    public sealed class Person
    {
        public string FirstName { get; set; }
        public int Id { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return string.Format(" Id: {0}, FirstName: {1}, LastName: {2}", Id, FirstName, LastName);
        }
    }
}
