using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportTableSchema
{
    public interface IDbOperation
    {
        /// <summary>
        /// 查出表结构，一张表一个table， 且前两个字段必须是表名和表说明
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="dbName"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        System.Data.DataSet GetTableSchema(string ip, string port, string dbName, string account, string password);
    }
}
