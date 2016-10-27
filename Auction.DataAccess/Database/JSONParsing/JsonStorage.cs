using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;
using Newtonsoft.Json;

namespace Auction.DataAccess.JSONParsing
{
  public class JsonStorage : IJsonStorage, IStorage
    {
        private static readonly string JsonStorageMutexName = "JsonStorage";
        private static readonly JsonSerializerSettings SerializationSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        private readonly string _folder;
        private string model;
        private Lazy<List<JsonModel>> _dataProvider;       

        public JsonStorage(string folder)
        {
            _folder = folder;
            _dataProvider = new Lazy<List<JsonModel>>(GetData, true);
        }

        public string FilePath { get; set; }

        private List<JsonModel> Data => _dataProvider.Value;

        public IJsonStorage FabricStorage()
        {
            return new JsonStorage(_folder);
        }

        public async Task AddAsync<T>(T item) where T : JsonModel
        {
          await Task.Run(() => Data.Add(item));
        }

        public async Task DeleteAsync(Guid id)
        {
            await Task.Run(() => Data.RemoveAll(d => d.Id == id));
        }

        public Task<T> GetByIdAsync<T>(Guid id) where T : JsonModel
        {
            return Task<T>.Run(() => Data.FirstOrDefault(d => d.Id == id) as T);
        }

        public Task<IEnumerable<T>> QueryAsync<T>() where T : JsonModel
        {
           return Task<IEnumerable<T>>.Run(() => Data.OfType<T>());  
        }

        public async Task SaveChanges()
        {
            await Task.Factory.StartNew(() =>
            {
                var mutex = new Mutex(false, JsonStorageMutexName);
                var path = string.Empty;
                ////for Data Init
                if (FilePath != string.Empty && FilePath != null)
                {
                    path = Path.Combine(_folder, FilePath, model);
                }

                if (FilePath == null)
                {
                    path = Path.Combine(_folder, model);
                }

                try
                {
                    using (var sw = new StreamWriter(path, false))
                    {
                        sw.Write(JsonConvert.SerializeObject(Data, SerializationSettings));
                        mutex.WaitOne();
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            });           
        }

        public async Task UpdateAsync<T>(T item) where T : JsonModel
        {
            await Task.Run(() =>
            {
                Data.RemoveAll(d => d.Id == item.Id);
                item.UpdateOn = DateTime.Now;
                Data.Add(item);
            });
        }

        public void SetPathToFile(string filepath)
        {
            FilePath = filepath;
        }       

        public List<JsonModel> GetData()
        {
            var path = string.Empty;
            if (FilePath != string.Empty && FilePath != null) 
            {
                path = Path.Combine(_folder, FilePath, model);
            }

            if (FilePath == null)
            {
                path = Path.Combine(_folder, model);
            }

            if (!File.Exists(path))
            {
                using (var sw = new StreamWriter(path))
                {
                    sw.Write("\n");
                }
            }

            var stringData = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<List<JsonModel>>(stringData, SerializationSettings);
            return data ?? new List<JsonModel>();
        }

        public void SetModel(JsonModel modelJSON)
        {
            model = modelJSON.ToString();
        }
    }
}
