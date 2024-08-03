using XStreamFast.Models;

namespace XStreamFast.Services.Interfaces
{
    public interface IPostgresServices
    {
        Task<BaseResponseModel<int>> IdentifyUser(int id);
    }
}
