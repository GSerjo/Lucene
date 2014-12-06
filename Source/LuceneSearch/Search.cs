using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneSearch.Domain;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace LuceneSearch
{
    public sealed class Search
    {
        private const string IndexDirectoryName = "LuceneIndex";
        private static readonly Occur[] _searchFlags = { Occur.SHOULD, Occur.SHOULD, Occur.SHOULD };
        private readonly string[] _fields = { "Id", "FirstName", "LastName" };
        private readonly string _indexDirectoryPath;
        private FSDirectory _indexDirectory;

        public Search()
        {
            _indexDirectoryPath = Directory.CreateDirectory(IndexDirectoryName).FullName;
        }

        public void AddOrUpdateIndex(params Person[] persons)
        {
            FSDirectory indexDirectory = GetIndexDirectory();
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    foreach (Person person in persons)
                    {
                        AddPersonToIndex(person, writer);
                    }
                }
            }
        }

        public void ClearIndex()
        {
            FSDirectory indexDirectory = GetIndexDirectory();
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(indexDirectory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.DeleteAll();
                }
            }
        }

        public void OptimizeIndex()
        {
            FSDirectory indexDirectory = GetIndexDirectory();
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.Optimize();
                }
            }
        }

        public List<Person> SearchPersons(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return new List<Person>();
            }

            int limit = 10;
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var searcher = new IndexSearcher(GetIndexDirectory(), true))
                {
                    Query query = MultiFieldQueryParser.Parse(Version.LUCENE_30, searchQuery, _fields, _searchFlags, analyzer);
                    ScoreDoc[] hits = searcher.Search(query, null, limit, Sort.INDEXORDER).ScoreDocs;
                    List<Person> results = hits.Select(hit => CreatePerson(searcher.Doc(hit.Doc))).ToList();
                    return results;
                }
            }
        }

        private void AddPersonToIndex(Person person, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("Id", person.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            var document = new Document();

            document.Add(new Field("Id", person.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field("FirstName", person.FirstName, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field("LastName", person.LastName, Field.Store.YES, Field.Index.ANALYZED));

            writer.AddDocument(document);
        }

        private Person CreatePerson(Document document)
        {
            return new Person
            {
                Id = Convert.ToInt32(document.Get("Id")),
                FirstName = document.Get("FirstName"),
                LastName = document.Get("LastName")
            };
        }

        private FSDirectory GetIndexDirectory()
        {
            if (_indexDirectory == null)
            {
                _indexDirectory = FSDirectory.Open(_indexDirectoryPath);
            }
            if (IndexWriter.IsLocked(_indexDirectory))
            {
                IndexWriter.Unlock(_indexDirectory);
            }
            string lockFile = Path.Combine(_indexDirectoryPath, "write.lock");
            if (File.Exists(lockFile))
            {
                File.Delete(lockFile);
            }
            return _indexDirectory;
        }

        private Query ParseQuery(string searchQuery, QueryParser parser)
        {
            searchQuery = searchQuery.Trim();
            try
            {
                return parser.Parse(searchQuery);
            }
            catch (ParseException)
            {
                return parser.Parse(QueryParser.Escape(searchQuery));
            }
        }
    }
}
