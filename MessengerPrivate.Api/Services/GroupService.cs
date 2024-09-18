//using MessengerPrivate.Api.Data;
//using MessengerPrivate.Api.SignalR.Models;
//using MongoDB.Driver;

//namespace MessengerPrivate.Api.Services
//{
//    public class GroupService
//    {

//        private readonly MongoDbContext _dbContext;

//        public GroupService(MongoDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task AddConnectionToGroup(string groupId, Connection connection)
//        {
//            var filter = Builders<Group>.Filter.Eq(g => g.Id, groupId);
//            var update = Builders<Group>.Update.AddToSet(g => g.Connections, connection);
//            await _dbContext.Groups.UpdateOneAsync(filter, update);
//        }

//        public async Task RemoveConnectionFromGroup(string groupId, string connectionId)
//        {
//            var filter = Builders<Group>.Filter.Eq(g => g.Id, groupId);
//            var update = Builders<Group>.Update.PullFilter(g => g.Connections, c => c.UserId == connectionId);
//            await _dbContext.Groups.UpdateOneAsync(filter, update);
//        }

//        public async Task<Group> GetGroupById(string groupId)
//        {
//            return await _dbContext.Groups.Find(g => g.Id == groupId).FirstOrDefaultAsync();
//        }

//        public async Task<Group> GetGroupForConnection(string connectionId)
//        {
//            var filter = Builders<Group>.Filter.ElemMatch(g => g.Connections, c => c.UserId == connectionId);
//            return await _dbContext.Groups.Find(filter).FirstOrDefaultAsync();
//        }

//    }
//}
