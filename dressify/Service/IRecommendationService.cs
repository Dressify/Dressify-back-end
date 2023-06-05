using Dressify.DataAccess.Dtos;
using Dressify.Models;

namespace dressify.Service
{
    public interface IRecommendationService
    {
        Task<List<int>> GetRecommendedProducts(Dictionary<string, double> averageRatings);

        Task<bool> SendProductToAiSystem(Product product);


    }
}
