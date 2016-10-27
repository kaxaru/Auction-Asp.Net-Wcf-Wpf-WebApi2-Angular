namespace Auction.DataAccess.JSONParsing
{
    public interface IJsonStorage
    {
        string FilePath { get; set; }

        void SetPathToFile(string filePath);

        IJsonStorage FabricStorage();
    }
}
