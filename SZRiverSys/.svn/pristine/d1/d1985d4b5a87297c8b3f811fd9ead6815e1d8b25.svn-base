using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace SZRiverSys.Comm
{
    public class ReportExcle
    { /// <summary>
        /// 表list
        /// </summary>
        public List<Aspose.Cells.Worksheet> sheetlist;
        /// <summary>
        /// 工作空间
        /// </summary>
        public Aspose.Cells.Workbook workbook;
        /// <summary>
        /// 打开路径
        /// </summary>
        public string OpenPath = null;
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        public bool OpenXLS()
        {

            bool reslut = false;
            try
            {
                if (OpenPath == null || OpenPath == "")
                {
                    workbook = new Aspose.Cells.Workbook();
                    reslut = false;
                }
                else
                {
                    workbook = new Aspose.Cells.Workbook(OpenPath);
                    reslut = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return reslut;
        }
        #region 插入数据到指定位置
        /// <summary>
        /// datatable数据插入excel
        /// </summary>
        /// <param name="datatable">datatable数据源</param>
        /// <param name="savefilepath">保存路径</param>
        /// <param name="Sheetindex">表索引号</param>
        /// <param name="rowindex">起始行索引号，0开始</param>
        /// <param name="colindex">起始列索引号，0开始</param>
        /// <returns></returns>
        public bool DataTableToExcel(DataTable datatable, int Sheetindex, int rowindex, int colindex)
        {

            try
            {
                if (datatable == null)
                {
                    return false;
                }
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[Sheetindex];
                Aspose.Cells.Cells cells = sheet.Cells;


                int nRow = rowindex;
                int nCol = colindex;
                foreach (DataRow row in datatable.Rows)
                {

                    try
                    {
                        for (int i = 0; i < datatable.Columns.Count; i++)
                        {
                            if (row[i].GetType().ToString() == "System.Drawing.Bitmap")
                            {
                                //------插入图片数据-------
                                System.Drawing.Image image = (System.Drawing.Image)row[i];
                                MemoryStream mstream = new MemoryStream();
                                image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                sheet.Pictures.Add(nRow, i + nCol, mstream);
                            }
                            else
                            {
                                cells[nRow, i + nCol].PutValue(row[i]);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw;

                    }
                    nRow++;
                }
                if (sheet != null)
                {
                    sheetlist.Add(sheet);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="savefilepath">保存路径</param>
        public void savefile(string savefilepath)
        {
            Aspose.Cells.Workbook saveworkbook = new Aspose.Cells.Workbook();
            for (int i = 0; i < sheetlist.Count; i++)
            {
                saveworkbook.Worksheets.Add("" + i);
                saveworkbook.Worksheets[i].Copy(sheetlist[i]);
                saveworkbook.Worksheets[i].Name = "Sheet" + (i + 1);

            }
            saveworkbook.Worksheets.RemoveAt(saveworkbook.Worksheets.Count - 1);
            saveworkbook.Save(savefilepath);
        }

        #endregion
    }
}