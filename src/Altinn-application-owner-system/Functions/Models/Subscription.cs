using System.Text.Json;
using System.Text.Json.Serialization;

namespace AltinnApplicationOwnerSystem.Functions 
{
    /// <summary>
    /// Subscription model
    /// </summary>
    public class Subscription 
    {
        /// <summary>
        /// Gets or sets the subscription id
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the subscription description
        /// </summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the subscription endpoint
        /// </summary>
        [JsonPropertyName("endPoint")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the subscription sourceFilter
        /// </summary>
        [JsonPropertyName("sourceFilter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SourceFilter { get; set; }

        /// <summary>
        /// Gets or sets the subscription subjectFilter
        /// </summary>
        [JsonPropertyName("subjectFilter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SubjectFilter { get; set; }

        /// <summary>
        /// Gets or sets the subscription alternativeSubjectFilter
        /// </summary>
        [JsonPropertyName("alternativeSubjectFilter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AlternativeSubjectFilter { get; set; }

        /// <summary>
        /// Gets or sets the subscription typeFilter
        /// </summary>
        [JsonPropertyName("typeFilter")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string TypeFilter { get; set; }

        /// <summary>
        /// Gets or sets the subscription consumer
        /// </summary>
        [JsonPropertyName("consumer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Consumer { get; set; }

        /// <summary>
        /// Gets or sets the subscription createdBy
        /// </summary>
        [JsonPropertyName("createdBy")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the subscription created
        /// </summary>
        [JsonPropertyName("created")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Created { get; set; }

        /// <summary>
        /// Deserializes the subscription from a JSON string.        
        /// </summary>
        /// <param name="json">The JSON string to deserialize</param>
        /// <returns>The deserialized subscription</returns>
        public static Subscription FromJson(string json) 
        {
            return JsonSerializer.Deserialize<Subscription>(json);
        }

        /// <summary>
        /// Serializes the subscription to a JSON string.
        /// </summary>
        /// <returns>Serialized subscription</returns>
        public string ToJson() 
        {
            return JsonSerializer.Serialize(this);
        }
    }
}