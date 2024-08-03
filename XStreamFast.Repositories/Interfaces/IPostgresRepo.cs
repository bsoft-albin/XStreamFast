namespace XStreamFast.Repositories.Interfaces
{
    public interface IPostgresRepo
    {
        Task<int> IdentifyUser(int id);
    }
}
