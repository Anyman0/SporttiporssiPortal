namespace SporttiporssiPortal.Interfaces
{
    public interface ISeriesService
    {
        Task<List<string>> GetSeriesListAsync(string sportName);
        Task<List<string>> GetSportListAsync();
    }
}
