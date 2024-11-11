using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    [JsonConverter(typeof(ISearchableJsonConverterFactory))]
    public interface ISearchable<T>
    {
        /// <summary>
        /// The uid of the index
        /// </summary>
        [JsonPropertyName("indexUid")]
        string IndexUid { get; }

        /// <summary>
        /// Results of the query.
        /// </summary>
        [JsonPropertyName("hits")]
        IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Number of documents skipped.
        /// </summary>
        [JsonPropertyName("offset")]
        int Offset { get; }

        /// <summary>
        /// Number of documents to take.
        /// </summary>
        [JsonPropertyName("limit")]
        int Limit { get; }

        /// <summary>
        /// Gets the estimated total number of hits returned by the search.
        /// </summary>
        [JsonPropertyName("estimatedTotalHits")]
        int EstimatedTotalHits { get; }

        /// <summary>
        /// Returns the number of documents matching the current search query for each given facet.
        /// </summary>
        [JsonPropertyName("facetDistribution")]
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> FacetDistribution { get; }

        /// <summary>
        /// Processing time of the query.
        /// </summary>
        [JsonPropertyName("processingTimeMs")]
        int ProcessingTimeMs { get; }

        /// <summary>
        /// Query originating the response.
        /// </summary>
        [JsonPropertyName("query")]
        string Query { get; }

        /// <summary>
        /// Contains the location of each occurrence of queried terms across all fields.
        /// </summary>
        [JsonPropertyName("_matchesPosition")]
        IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> MatchesPosition { get; }

        /// <summary>
        /// Returns the numeric min and max values per facet of the hits returned by the search query.
        /// </summary>
        [JsonPropertyName("facetStats")]
        IReadOnlyDictionary<string, FacetStat> FacetStats { get; }
    }
}
