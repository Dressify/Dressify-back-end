using Dressify.DataAccess.Dtos;
using Dressify.Models;
using Dressify.Utility;
using Newtonsoft.Json;
using System.Text;

namespace dressify.Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly HttpClient _httpClient;

        public RecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        public async Task<List<int>> GetRecommendedProducts(Dictionary<string, double> averageRatings)
        {
            var apiUrl = $"{SD.AIUrl}/recommendations";
            var json = JsonConvert.SerializeObject(averageRatings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                var recommendedProducts = JsonConvert.DeserializeObject<List<int>>(jsonResult);
                return recommendedProducts;
            }

            // Handle error scenarios if needed
            throw new Exception("Failed to retrieve recommended products from the external API.");
        }



        public async Task<bool> SendProductToAiSystem(Product product)
        {
            // Create the JSON payload
            var payload = new
            {
                ProductId = product.ProductId,
                category = product.Category,
                subCategory = product.SubCategory,
                ratingAvg = 0,
                ratingCount = 0
            };

            // Convert the payload to JSON
            var jsonPayload = JsonConvert.SerializeObject(payload);

            // Send the JSON payload to the AI system
            var apiUrl = $"{SD.AIUrl}/addProduct"; // Replace with the actual API URL
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)

                return true;
            else
                return false;
            
        }


    }
}
