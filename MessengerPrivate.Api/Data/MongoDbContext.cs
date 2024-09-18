namespace MessengerPrivate.Api.Data
{
    using MessengerPrivate.Api.Data.Entities;
    using MessengerPrivate.Api.SignalR.Models;
    using MongoDB.Driver;
    using System;

    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Roles");
        public IMongoCollection<Contact> Contacts => _database.GetCollection<Contact>("Contacts");
        public IMongoCollection<Conversation> Conversations => _database.GetCollection<Conversation>("Conversations");
        public IMongoCollection<Messenger> Messengers => _database.GetCollection<Messenger>("Messengers");
        public IMongoCollection<CallSession> CallSessions => _database.GetCollection<CallSession>("CallSessions");

        //public IMongoCollection<Group> Groups => _database.GetCollection<Group>("Groups");

        //public IMongoCollection<Connection> Connections => _database.GetCollection<Connection>("Connections");

        public IMongoCollection<Icon> Icons => _database.GetCollection<Icon>("Icons");


    }


    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }


}
