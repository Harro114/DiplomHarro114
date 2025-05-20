using System.Text.Json.Serialization;

namespace Gamification.Models.DTO;

public class OrdersResponseDTO
{    
    [JsonPropertyName("orders")]
    public List<OrdersDTO> order { get; set; }

}