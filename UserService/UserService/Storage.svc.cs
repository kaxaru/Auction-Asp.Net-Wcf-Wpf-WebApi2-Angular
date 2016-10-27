using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UserService.Model;

namespace UserService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Storage : IJsonService
    {
        private static readonly string JsonStorageMutexName = "JsonStorage";
        private string _folder;
        private string _model;

        public Storage()
        {
        }

        public Storage(string folder, string model)
        {
            _folder = folder;
            _model = model;
        }

        public string FilePath { get; set; }

        public string Model { get; set; }

        private Lazy<List<JsonModel>> DataProvider { get; set; }

        private List<JsonModel> Data => DataProvider.Value;

        public async Task Add(Storage storage, JsonModel item)
        {
            await Task.Run(() =>
            {
                Configure(storage);                
                Data.Add(item);
                SaveChanges();
            });
        }

        public async Task Delete(Storage storage, Guid id)
        {
            await Task.Run(() =>
            {
                Configure(storage);
                Data.RemoveAll(d => d.Id == id);
                SaveChanges();
            });
        }

        public async Task<JsonModel> GetById(Storage storage, Guid id)
        {
            return await Task<JsonModel>.Run(() =>
            {
                Configure(storage);
                return Data.FirstOrDefault(d => d.Id == id);
            }); 
        }

        public async Task Update(Storage storage, JsonModel item)
        {
            await Task.Run(() =>
            {
                Configure(storage);
                Data.RemoveAll(d => d.Id == item.Id);
                item.UpdateOn = DateTime.Now;
                Data.Add(item);
                SaveChanges();
            });
        }

        public List<JsonModel> GetData()
        {
                var path = string.Empty;
                if (FilePath != string.Empty && FilePath != null)
                {
                    path = FilePath;
                }

                if (FilePath == null)
                {
                    path = Path.Combine(_folder, _model);
                }

                if (!File.Exists(path))
                {
                    using (var sw = new StreamWriter(path))
                    {
                        sw.Write("\n");
                    }
                }

                var stringData = File.ReadAllText(path);
                switch (Model)
                {
                    case "User":
                        var userList = JsonConvert.DeserializeObject<List<User>>(stringData);
                        var users = userList.Cast<JsonModel>().ToList();
                        return users ?? new List<JsonModel>();
                    case "Permission":
                        var permissionList = JsonConvert.DeserializeObject<List<Permission>>(stringData);
                        var permissions = permissionList.Cast<JsonModel>().ToList();
                        return permissions ?? new List<JsonModel>();
                    default:
                        return new List<JsonModel>();
                } 
        }

        public Storage CreateJson(string folder, string model)
        {
            Storage storage = new Storage(folder, model);
            return storage;
        }

        public async Task<IEnumerable<JsonModel>> Query(Storage storage)
        {
            Task<IEnumerable<JsonModel>> taskInvoke = Task<IEnumerable<JsonModel>>.Factory.StartNew(() =>
            {
                Configure(storage);
                return Data.OfType<JsonModel>();
            });

            return await taskInvoke;
        }

        private void Configure(Storage storage)
        {
            FilePath = storage.FilePath;
            Model = storage.Model;
            DataProvider = new Lazy<List<JsonModel>>(GetData, true);
        }

        private void SaveChanges()
        {
            var mutex = new Mutex(false, JsonStorageMutexName);
            var path = string.Empty;
            if (FilePath != string.Empty && FilePath != null)
            {
                path = FilePath;
            }

            if (FilePath == null)
            {
                path = Path.Combine(_folder, _model);
            }

            try
            {
                using (var sw = new StreamWriter(path, false))
                {
                    var serializeData = JsonConvert.SerializeObject(Data);
                    mutex.WaitOne();
                    sw.Write(serializeData);
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
