using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportTableSchema
{
    public class SqlServerOperation : IDbOperation
    {
        private string _ip;
        private string _port;
        private string _dbName;
        private string _account;
        private string _password;
        private string GetDbConnetStr()
        {
            return $"Data Source={_ip},{_port};Initial Catalog={_dbName};User ID={_account};password={_password};";
        }

        public DataSet GetTableSchema(string ip, string port, string dbName, string account, string password)
        {
            _ip = ip;
            _password = password;
            _port = port;
            _dbName = dbName;
            _account = account;
            var connstr = GetDbConnetStr();
            var ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connstr))
            {
                var sql = @"declare  @rowCont int,@i int
                                    if exists(select * from tempdb..sysobjects where id=object_id('tempdb..#temp_tables'))
                                    drop table #temp_tables
                                    SELECT
	                                    row_number() over(order by D.name) i,
                                         表名    =   D.name,
                                         表说明  =   isnull(F.value,'')
                                     Into #temp_tables
                                     FROM
                                         syscolumns A
                                     Inner Join
                                         sysobjects D
                                     On
                                         A.id=D.id  and D.xtype='U' and  D.name<>'dtproperties'
                                     Left Join
                                     sys.extended_properties F
                                     On
                                         D.id=F.major_id and F.minor_id=0
                                     Where A.colorder = 1
                                     set @rowCont = @@rowcount

                                    set @i = 1;

                                    declare @tableName varchar(40),@tableDesc nvarchar(40)

                                    while @i<=@rowCont
                                    begin
	                                    select @tableName=表名,@tableDesc=convert(nvarchar(40),表说明) from #temp_tables where i = @i
	                                    SELECT
                                         表名       = Case When A.colorder=1 Then D.name Else '' End,
                                         表说明     = Case When A.colorder=1 Then isnull(F.value,'') Else '' End,
                                         字段序号   = A.colorder,
                                         字段名     = A.name,
                                         字段说明   = isnull(G.[value],''),
                                         自增标识       = Case When COLUMNPROPERTY( A.id,A.name,'IsIdentity')=1 Then '√'Else '' End,
                                         主键       = Case When exists(SELECT 1 FROM sysobjects Where xtype='PK' and parent_obj=A.id and name in (
                                                          SELECT name FROM sysindexes WHERE indid in( SELECT indid FROM sysindexkeys WHERE id = A.id AND colid=A.colid))) then '√' else '' end,
                                         类型       = B.name,
                                         占用字节数 = A.Length,
                                         长度       = COLUMNPROPERTY(A.id,A.name,'PRECISION'),
                                         小数位数   = isnull(COLUMNPROPERTY(A.id,A.name,'Scale'),0),
                                         允许空     = Case When A.isnullable=1 Then '√'Else '' End,
                                         默认值     = isnull(E.Text,'')
                                     FROM
                                         syscolumns A
                                     Left Join
                                         systypes B
                                     On
                                         A.xusertype=B.xusertype
                                     Inner Join
                                         sysobjects D
                                     On
                                         A.id=D.id  and D.xtype='U' and  D.name<>'dtproperties'
                                     Left Join
                                         syscomments E
                                     on
                                         A.cdefault=E.id
                                     Left Join
                                     sys.extended_properties  G
                                     on
                                         A.id=G.major_id and A.colid=G.minor_id
                                     Left Join

                                     sys.extended_properties F
                                     On
                                         D.id=F.major_id and F.minor_id=0
                                         where d.name=@tableName    --如果只查询指定表,加上此条件
                                     Order By
                                         A.id,A.colorder
	                                    set @i=@i+1
                                    end";
                using (SqlDataAdapter sda = new SqlDataAdapter(sql, connection))
                {
                    //创建并填充 Dataset
                    sda.Fill(ds);
                }
            }
            return ds;
        }
    }
}
