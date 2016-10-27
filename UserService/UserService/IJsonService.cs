using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using UserService.Model;

namespace UserService
{
    [ServiceKnownType(typeof(User))]
    [ServiceKnownType(typeof(Permission))]
    [ServiceContract]
    public interface IJsonService
    {
        [OperationContract]
        Storage CreateJson(string folder, string model);

        [OperationContract]
        Task<JsonModel> GetById(Storage storage, Guid id);

        [OperationContract]
        Task<IEnumerable<JsonModel>> Query(Storage storage);

        [OperationContract]
        Task Add(Storage storage, JsonModel item);

        [OperationContract]
        Task Update(Storage storage, JsonModel item);

        [OperationContract]
        Task Delete(Storage storage, Guid id);
    } 
}
