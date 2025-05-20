using System.Text.Json.Serialization;

namespace Gamification.Models.DTO;

public class CategoriesResponseDTO
{
    [JsonPropertyName("categories")]
    public List<CategoriesDTO> categorie { get; set; }
}