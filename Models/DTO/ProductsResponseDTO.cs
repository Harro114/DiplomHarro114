using System.Text.Json.Serialization;

namespace Gamification.Models.DTO;

public class ProductsResponseDTO
{
    [JsonPropertyName("products")]
    public List<ProductsDTO> product { get; set; }
}