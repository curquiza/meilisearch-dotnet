using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search Query for Meilisearch class.
    /// </summary>
    public class SearchQuery
    {
        public SearchQuery(string query)
        {
            Q = query;
        }

        /// <summary>
        /// Gets or sets query string.
        /// </summary>
        [JsonPropertyName("q")]
        public string Q { get; set; }

        /// <summary>
        /// Gets or sets offset for the Query.
        /// </summary>
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets limits the number of results.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the filter to apply to the query.
        /// </summary>
        [JsonPropertyName("filter")]
        public dynamic Filter { get; set; }

        /// <summary>
        /// Gets or sets attributes to retrieve.
        /// </summary>
        [JsonPropertyName("attributesToRetrieve")]
        public IEnumerable<string> AttributesToRetrieve { get; set; }

        /// <summary>
        /// Gets or sets attributes to crop.
        /// </summary>
        [JsonPropertyName("attributesToCrop")]
        public IEnumerable<string> AttributesToCrop { get; set; }

        /// <summary>
        /// Gets or sets length used to crop field values.
        /// </summary>
        [JsonPropertyName("cropLength")]
        public int? CropLength { get; set; }

        /// <summary>
        /// Gets or sets attributes to highlight.
        /// </summary>
        [JsonPropertyName("attributesToHighlight")]
        public IEnumerable<string> AttributesToHighlight { get; set; }

        /// <summary>
        /// Gets or sets the crop marker to apply before and/or after cropped part selected within an attribute defined in `attributesToCrop` parameter.
        /// </summary>
        [JsonPropertyName("cropMarker")]
        public string CropMarker { get; set; }

        /// <summary>
        /// Gets or sets the tag to put before the highlighted query terms.
        /// </summary>
        [JsonPropertyName("highlightPreTag")]
        public string HighlightPreTag { get; set; }

        /// <summary>
        /// Gets or sets the tag to put after the highlighted query terms.
        /// </summary>
        [JsonPropertyName("highlightPostTag")]
        public string HighlightPostTag { get; set; }

        /// <summary>
        /// Gets or sets the facets for the query.
        /// </summary>
        [JsonPropertyName("facets")]
        public IEnumerable<string> Facets { get; set; }

        /// <summary>
        /// Gets or sets showMatchesPosition. It defines whether an object that contains information about the matches should be returned or not.
        /// </summary>
        [JsonPropertyName("showMatchesPosition")]
        public bool? ShowMatchesPosition { get; set; }

        /// <summary>
        /// Gets or sets the sorted attributes.
        /// </summary>
        [JsonPropertyName("sort")]
        public IEnumerable<string> Sort { get; set; }

        /// <summary>
        /// Gets or sets the words matching strategy.
        /// </summary>
        [JsonPropertyName("matchingStrategy")]
        public string MatchingStrategy { get; set; }
    }
}
