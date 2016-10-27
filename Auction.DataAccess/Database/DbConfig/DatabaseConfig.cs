using System.IO;
using Auction.DataAccess.Database;
using Auction.DataAccess.DatabaseContext;
using Auction.DataAccess.JSONParsing;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.DbConfig
{
    public class DatabaseConfig : IDatabaseConfig
    {
        private readonly ISqlStorage _sqlstorage;
        private IStorage _storage;
        private IJsonStorage _jsonstorage;

        public DatabaseConfig()
        {
        }

        public DatabaseConfig(IJsonStorage json, ISqlStorage sql)
        {
            _jsonstorage = json;
            _sqlstorage = sql;
        }

        public IStorage GetDatabase(Models.Auction auction)
        {
            switch (auction.Type)
            {
                case TypeAuction.Json:
                    if (_jsonstorage.FilePath != auction.Location)
                    {
                        _jsonstorage = _jsonstorage.FabricStorage();
                        _jsonstorage.SetPathToFile(auction.Location);
                    }

                    _storage = (IStorage)_jsonstorage;
                    break;
                case TypeAuction.Sql:
                    var filePath = Path.Combine(auction.Location, string.Format("{0}.{1}", auction.Name, "mdf"));
                    _storage = _sqlstorage.SetConnectionString(filePath);
                    break;
            }

            return _storage;
        }

        public IStorage GetGlobalDatabase()
        {
            return (IStorage)_jsonstorage;
        }
    }
}
