using XStreamFast.Models;
using XStreamFast.Repositories.Implementations;
using XStreamFast.Repositories.Interfaces;
using XStreamFast.Services.Interfaces;

namespace XStreamFast.Services.Implementations
{
    public class PostgresServices(IPostgresRepo postgresRepo) : IPostgresServices
    {
        private readonly IPostgresRepo _postgresRepo = postgresRepo;
        public async Task<BaseResponseModel<int>> IdentifyUser(int id)
        {
            BaseResponseModel<int> baseResponseModel = new();
            baseResponseModel.Data = await _postgresRepo.IdentifyUser(id);
            if (baseResponseModel != null)
            {
                if (baseResponseModel.Data > 0)
                {
                    return baseResponseModel;
                }
                else
                {
                    baseResponseModel.StatusCode = 204;
                    baseResponseModel.StatusMessage = "User Not Found";

                    return baseResponseModel;
                }
            }
            else {
                return baseResponseModel ?? new();
            }
        }
    }
}
