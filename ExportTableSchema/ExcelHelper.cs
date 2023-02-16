using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportTableSchema
{
    public class ExcelHelper
    {
        public static System.IO.Stream ExcelStream(DataSet ds)
        {
            var hssfworkbook = GenerateExcel(ds);
            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);
            file.Seek(0, SeekOrigin.Begin);
            return file;
        }
        public static HSSFWorkbook GenerateExcel(DataSet ds)
        {
            HSSFWorkbook hssfWorkbook = new HSSFWorkbook();
            for (var i = 0; i < ds.Tables.Count; i++)
            {
                var table = ds.Tables[i];
                GenerateExcel(table, hssfWorkbook);
            }
            return hssfWorkbook;
        }

        public static ISheet GenerateExcel(DataTable table, HSSFWorkbook hssfworkbook)
        {
            ISheet sheet = hssfworkbook.CreateSheet(table.TableName);
            IRow rowHeader = sheet.CreateRow(0);

            for (var i = 0; i < table.Columns.Count; i++)
            {
                var head = table.Columns[i];
                rowHeader.CreateCell(i).SetCellValue(head.ColumnName);
            }
            for (var i = 0; i < table.Rows.Count; i++)
            {
                var tableRow = table.Rows[i];
                IRow row = sheet.CreateRow(i + 1);
                for (var j = 0; j < table.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(tableRow[table.Columns[j]].ToString());
                }
            }
            return sheet;
        }

        private static void SetIncomeCostCell(ISheet sheet, int runnum, int cellnum, ICellStyle cellstyle, IFont font)
        {
            ICell cell = sheet.GetRow(runnum).GetCell(cellnum);
            cell.CellStyle.SetFont(font);
            cell.CellStyle = cellstyle;
        }


        
        /// <summary>
        /// 对单元格进行判断取值
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown: //无法识别类型
                default: //默认类型
                    return cell.ToString();//
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        /// <summary>
        /// datatable 去空行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DataTable RemoveEmpty(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool rowdataisnull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {

                        rowdataisnull = false;
                    }
                }
                if (rowdataisnull)
                {
                    removelist.Add(dt.Rows[i]);
                }

            }
            //移除空行
            for (int i = 0; i < removelist.Count; i++)
            {
                dt.Rows.Remove(removelist[i]);
            }
            return dt;
        }


    }
}
