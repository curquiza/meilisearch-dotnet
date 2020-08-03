﻿using System;
using System.Net.Http;
using Microsoft.Extensions.Http;

namespace Meilisearch.Tests
{
    public class DocumentFixture : IDisposable
    {
        public Index documentIndex { get; private set; }

        public Index DocumentDeleteIndex { get; private set; }

        public DocumentFixture()
        {
           SetUp();
           SetUpForDelete();
        }

        private void SetUpForDelete()
        {
            try
            {
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex("MoviesToDelete").Result;
                if (index == null)
                {
                    this.DocumentDeleteIndex = client.CreateIndex("MoviesToDelete").Result;
                }
                else
                {
                    this.DocumentDeleteIndex = index;
                }
                var movies = new[]
                {
                    new Movie {Id = "10", Name = "SuperMan"},
                };
                var updateStatus = this.DocumentDeleteIndex.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        public void SetUp()
        {
            try
            {
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex("Movies").Result;

                if (index == null)
                {
                    this.documentIndex = client.CreateIndex("Movies").Result;
                }
                else
                {
                    this.documentIndex = index;
                }

                var movies = new[]
                {
                    new Movie {Id = "10", Name = "SuperMan"},
                    new Movie {Id = "11", Name = "SpiderMan"},
                    new Movie {Id = "12", Name = "IronMan"},
                    new Movie {Id = "13", Name = "SpiderMan"},
                    new Movie {Id = "14", Name = "IronMan"},
                    new Movie {Id = "15", Name = "SpiderMan"},
                    new Movie {Id = "16", Name = "IronMan"}
                };
                var updateStatus = this.documentIndex.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void Dispose()
        {
        }
    }
}
