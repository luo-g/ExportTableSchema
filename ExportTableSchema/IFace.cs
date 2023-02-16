using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportTableSchema
{
    public interface IFace
    {
        IDbOperation dbOperation { get; }
        /// <summary>
        /// 数据库图标
        /// </summary>
        string GetDbIco();

        DataSet GetTableSchema(string ip,string port,string dbName,string account,string password);
    }
}
