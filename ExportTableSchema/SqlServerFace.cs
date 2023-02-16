using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportTableSchema
{
    public class SqlServerFace : IFace
    {
        public IDbOperation dbOperation { get
            {
                return new SqlServerOperation();
            }
        }
        public new string GetDbIco()
        {
            return "\\Resources\\sqlserver.jpg";
        }

        public DataSet GetTableSchema(string ip, string port, string dbName, string account, string password)
        {
            return dbOperation.GetTableSchema(ip, port, dbName, account, password);
        }
    }
}
