using XStreamFast.DbEngine;
using XStreamFast.Frameworks.CommonMeths;
using XStreamFast.Repositories.Interfaces;

namespace XStreamFast.Repositories.Implementations
{
    public class PostgresRepo(IPostgresMapper postgres) : IPostgresRepo
    {
        private readonly IPostgresMapper _postgresMapper = postgres;
        public async Task<int> IdentifyUser(int id)
        {
            int result = 0;
            try
            {
                throw new Exception();
                result = await _postgresMapper.ExecuteScalarAsync<int>($"select count(*) from users where id = {id};");
            }
            catch (Exception x)
            {
                await XStreamFastLoggers.WriteExceptionLog(x);
            }

            return result;
        }
    }
}
