using FoodDeliveryBot.Models;

namespace FoodDeliveryBot.Repositories
{
    public class ConversationInfoRepository : BaseRepository<ConversationInfo>
    {
        public ConversationInfoRepository(string collectionName) : base(collectionName)
        {
        }
    }
}