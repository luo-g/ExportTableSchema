# ExportTableSchema
导出数据库表结构说明

### 开发环境 
.net5.0  winform  vs2019

### IFace
开发者可自行实现IFace接口即可扩展实现不同数据库导出功能
> IDbOperation
> 该接口实现数据库操作返回DataSet
> 
> 暂时只实现了sqlserver、后期可实现mysql、oracle ...

