using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Database
{
    public interface IStorage
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : JsonModel;

        Task<IEnumerable<T>> QueryAsync<T>() where T : JsonModel;

        Task AddAsync<T>(T item) where T : JsonModel;

        Task UpdateAsync<T>(T item) where T : JsonModel;

        Task DeleteAsync(Guid id);

        Task SaveChanges();

        void SetModel(JsonModel model);       
    }
}
