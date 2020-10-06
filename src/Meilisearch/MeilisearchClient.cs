namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Typed client for MeiliSearch.
    /// </summary>
    public class MeilisearchClient
    {
        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Default client for Meilisearch API.
        /// </summary>
        /// <param name="url">URL to connect to meilisearch client.</param>
        /// <param name="apiKey">API key for the usage.</param>
        public MeilisearchClient(string url, string apiKey = default)
        {
            this.client = new HttpClient { BaseAddress = new Uri(url) };
            if (!string.IsNullOrEmpty(apiKey))
            {
                this.client.DefaultRequestHeaders.Add("X-Meili-API-Key", apiKey);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchClient"/> class.
        /// Custom client for Meilisearch API. Use it with proper Http Client Factory.
        /// </summary>
        /// <param name="client">Injects the reusable Httpclient.</param>
        /// <param name="apiKey">API Key for MeilisearchClient. Best practice is to use HttpClient default header rather than this parameter.</param>
        public MeilisearchClient(HttpClient client, string apiKey = default)
        {
            this.client = client;
            if (!string.IsNullOrEmpty(apiKey))
            {
                this.client.DefaultRequestHeaders.Add("X-Meili-API-Key", apiKey);
            }
        }

        /// <summary>
        /// Gets the current MeiliSearch version. For more details on response
        /// https://docs.meilisearch.com/references/version.html#get-version-of-meilisearch.
        /// </summary>
        /// <returns>Returns the MeiliSearch version with commit and build version.</returns>
        public async Task<MeiliSearchVersion> GetVersion()
        {
            var response = await this.client.GetAsync("/version");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<MeiliSearchVersion>();
        }

        /// <summary>
        /// Creates and index with an UID and a primary key.
        /// BEWARE : Throws error if the index already exist. Use GetIndex before using Create.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <param name="primaryKey">Primary key for documents.</param>
        /// <returns>Returns Index.</returns>
        public async Task<Index> CreateIndex(string uid, string primaryKey = default)
        {
            Index index = new Index(uid, primaryKey);
            var response = await this.client.PostAsJsonAsync("/indexes", index);

            // TODO : Revisit the Exception, We need to handle it better.
            return response.IsSuccessStatusCode ? index.WithHttpClient(this.client) : throw new Exception("Not able to create index. May be Index already exist");
        }

        /// <summary>
        /// Gets all the Indexes for the instance. Throws error if the index does not exist.
        /// </summary>
        /// <returns>Return Enumerable of Index.</returns>
        public async Task<IEnumerable<Index>> GetAllIndexes()
        {
            var response = await this.client.GetAsync("/indexes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Index>>();
            return content
                .Select(p => p.WithHttpClient(this.client));
        }

        /// <summary>
        /// Gets and index with the unique ID.
        /// </summary>
        /// <param name="uid">UID of the index.</param>
        /// <returns>Returns Index or Null if the index does not exist.</returns>
        public async Task<Index> GetIndex(string uid)
        {
            var response = await this.client.GetAsync($"/indexes/{uid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Index>();
                return content.WithHttpClient(this.client);
            }

            return null;  // TODO:  Yikes!! returning Null  Need to come back to solve this.
        }

        /// <summary>
        /// Gets the index instance or creates the index if it does not exist.
        ///
        /// /!\ Really basics. An error handler should be created to check the errorCode.
        /// cf https://docs.meilisearch.com/errors/#index_already_exists.
        /// </summary>
        /// <param name="uid">Unique Id.</param>
        /// <param name="primaryKey">Primary key for documents.</param>
        /// <returns>Returns Index.</returns>
        public async Task<Index> GetOrCreateIndex(string uid, string primaryKey = default)
        {
            try
            {
                return await this.CreateIndex(uid, primaryKey);
            }
            catch (Exception e)
            {
                if (e.Message == "Not able to create index. May be Index already exist")
                {
                    return await this.GetIndex(uid);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Gets stats of all indexes.
        /// </summary>
        /// <returns>Returns stats of all indexes.</returns>
        public Task<Stats> GetStats()
        {
            return this.client.GetFromJsonAsync<Stats>("/stats");
        }

        /// <summary>
        /// Gets health state of the server.
        /// </summary>
        /// <returns>Returns whether server is healthy or not.</returns>
        public async Task<bool> GetHealth()
        {
            try
            {
                var result = await this.client.GetFromJsonAsync<HealthObject>("/health");
                return result.Health;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates server health status.
        /// </summary>
        /// <param name="value">New health value.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task UpdateHealth(bool value)
        {
            var response = await this.client.PutAsJsonAsync("/health", new HealthObject { Health = value });
            response.EnsureSuccessStatusCode();
        }
    }
}
