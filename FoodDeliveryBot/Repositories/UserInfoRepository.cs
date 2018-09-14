namespace FoodDeliveryBot.Repositories
{
    using FoodDeliveryBot.Models;

    public class UserInfoRepository : BaseRepository<UserInfo>
    {
        public UserInfoRepository(string collectionName) : base(collectionName)
        {
        }
    }
}