using Colten.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Colten.DAL.Helper
{
    public abstract class DbFactoryBase
    {
        private readonly ConnectionStrings _connectionStrings;

        public DbFactoryBase(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings.Value;
        }

        internal string DbConnectionString => _connectionStrings.ColtenConnectionString;
        internal IDbConnection DbConnection => new SqlConnection(_connectionStrings.ColtenConnectionString);

        public virtual async Task<IEnumerable<T>> DbQueryAsync<T>(string sql, object parameters = null)
        {
            using (IDbConnection dbCon = DbConnection)
            {
                dbCon.Open();
                if (parameters == null)
                    return await dbCon.QueryAsync<T>(sql, commandType: CommandType.StoredProcedure);

                return await dbCon.QueryAsync<T>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }
        public virtual async Task<T> DbQuerySingleAsync<T>(string sql, object parameters)
        {
            using (IDbConnection dbCon = DbConnection)
            {
                return await dbCon.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public virtual T DbQuerySingleWithoutAsync<T>(string sql, object parameters)
        {
            using (IDbConnection dbCon = DbConnection)
            {
                return dbCon.QueryFirstOrDefault<T>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public virtual async Task<int> DbExecuteAsync<T>(string sql, object parameters)
        {
            using (IDbConnection dbCon = DbConnection)
            {
                return await dbCon.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public virtual async Task<int> DbExecuteScalarAsync(string sql, object parameters)
        {
            using (IDbConnection dbCon = DbConnection)
            {
                return await dbCon.ExecuteScalarAsync<int>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public virtual async Task<T> DbExecuteScalarDynamicAsync<T>(string sql, object parameters = null)
        {
            using (IDbConnection dbCon = DbConnection)
            {
                if (parameters == null)
                    return await dbCon.ExecuteScalarAsync<T>(sql);

                return await dbCon.ExecuteScalarAsync<T>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public virtual async Task<(IEnumerable<T> Data, int RecordCount)> DbQueryMultipleAsync<T>(string sql, object parameters = null)
        {
            IEnumerable<T> data = null;
            int totalRecords = 0;

            using (IDbConnection dbCon = DbConnection)
            {
                using (GridReader results = await dbCon.QueryMultipleAsync(sql, parameters))
                {
                    data = await results.ReadAsync<T>();
                    totalRecords = await results.ReadSingleAsync<int>();
                }
            }

            return (data, totalRecords);
        }

        public virtual async Task<Tuple<T1, IEnumerable<T2>>> DbQueryReadMultipleAsync<T1, T2>(string sql, object parameters = null)
        {
            T1 dataT1;
            IEnumerable<T2> dataT2 = null;
            Tuple<T1, IEnumerable<T2>> tpl = null;
            using (IDbConnection dbCon = DbConnection)
            {
                using (GridReader results = await dbCon.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure))
                {
                    dataT1 = await results.ReadFirstOrDefaultAsync<T1>();
                    if (dataT1 != null)
                    {
                        dataT2 = await results.ReadAsync<T2>();
                    }
                }
                tpl = Tuple.Create(dataT1, dataT2);
            }

            return tpl;
        }
        public virtual async Task<Tuple<T1, IEnumerable<T2>, IEnumerable<T3>>> DbQueryReadMultipleAsync<T1, T2, T3>(string sql, object parameters = null)
        {
            T1 dataT1;
            IEnumerable<T2> dataT2 = null;
            IEnumerable<T3> dataT3 = null;
            Tuple<T1, IEnumerable<T2>, IEnumerable<T3>> tpl = null;
            using (IDbConnection dbCon = DbConnection)
            {
                using (GridReader results = await dbCon.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure))
                {
                    dataT1 = await results.ReadFirstOrDefaultAsync<T1>();
                    if (dataT1 != null)
                    {
                        dataT2 = await results.ReadAsync<T2>();
                        dataT3 = await results.ReadAsync<T3>();
                    }
                }
                tpl = Tuple.Create(dataT1, dataT2, dataT3);
            }

            return tpl;
        }
        public virtual async Task<Tuple<T1, T2, IEnumerable<T3>, IEnumerable<T4>>> DbQueryReadMultipleAsync<T1, T2, T3, T4>(string sql, object parameters = null)
        {
            T1 dataT1;
            T2 dataT2 = default(T2);
            IEnumerable<T3> dataT3 = null;
            IEnumerable<T4> dataT4 = null;
            Tuple<T1, T2, IEnumerable<T3>, IEnumerable<T4>> tpl = null;
            using (IDbConnection dbCon = DbConnection)
            {
                using (GridReader results = await dbCon.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure))
                {
                    dataT1 = await results.ReadFirstOrDefaultAsync<T1>();
                    if (dataT1 != null)
                    {
                        dataT2 = await results.ReadFirstOrDefaultAsync<T2>();
                        dataT3 = await results.ReadAsync<T3>();
                        dataT4 = await results.ReadAsync<T4>();
                    }
                }
                tpl = Tuple.Create(dataT1, dataT2, dataT3, dataT4);
            }

            return tpl;
        }
        public virtual async Task<Tuple<T1, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>>
            DbQueryReadMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(string sql, object parameters = null)
        {
            T1 dataT1;
            IEnumerable<T2> dataT2 = null;
            IEnumerable<T3> dataT3 = null;
            IEnumerable<T4> dataT4 = null;
            IEnumerable<T5> dataT5 = null;
            IEnumerable<T6> dataT6 = null;
            IEnumerable<T7> dataT7 = null;
            Tuple<T1, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>> tpl = null;
            using (IDbConnection dbCon = DbConnection)
            {
                using (GridReader results = await dbCon.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure))
                {
                    dataT1 = await results.ReadFirstOrDefaultAsync<T1>();
                    if (dataT1 != null)
                    {
                        dataT2 = await results.ReadAsync<T2>();
                        dataT3 = await results.ReadAsync<T3>();
                        dataT4 = await results.ReadAsync<T4>();
                        dataT5 = await results.ReadAsync<T5>();
                        dataT6 = await results.ReadAsync<T6>();
                        dataT7 = await results.ReadAsync<T7>();
                    }
                }
                tpl = Tuple.Create(dataT1, dataT2, dataT3, dataT4, dataT5, dataT6, dataT7);
            }

            return tpl;
        }
        public virtual async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>>
            DbQueryReadMultipleAsync<T1, T2, T3, T4, T5,T6>(string sql, object parameters = null)
        {
            IEnumerable<T1> dataT1 = null;
            IEnumerable<T2> dataT2 = null;
            IEnumerable<T3> dataT3 = null;
            IEnumerable<T4> dataT4 = null;
            IEnumerable<T5> dataT5 = null;
            IEnumerable<T6> dataT6 = null;
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>> tpl = null;
            using (IDbConnection dbCon = DbConnection)
            {
                using (GridReader results = await dbCon.QueryMultipleAsync(sql, parameters, commandType: CommandType.StoredProcedure))
                {
                    dataT1 = await results.ReadAsync<T1>();
                    dataT2 = await results.ReadAsync<T2>();
                    dataT3 = await results.ReadAsync<T3>();
                    dataT4 = await results.ReadAsync<T4>();
                    dataT5 = await results.ReadAsync<T5>();
                    dataT6 = await results.ReadAsync<T6>();
                }
                tpl = Tuple.Create(dataT1, dataT2, dataT3, dataT4, dataT5,dataT6);
            }

            return tpl;
        }
    }
}
