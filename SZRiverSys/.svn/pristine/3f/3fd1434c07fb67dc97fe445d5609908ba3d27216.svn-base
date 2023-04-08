using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SZRiverSys.Comm
{
    public class ZIP
    {
        /// <summary>
        /// 将多个流进行zip压缩，返回压缩后的流
        /// </summary>
        /// <param name="streams">key：文件名；value：文件名对应的要压缩的流</param>
        /// <returns>压缩后的流</returns>
        public static MemoryStream PackageManyZip(Dictionary<string, Stream> streams)
        {
            byte[] buffer = new byte[6500];
            MemoryStream returnStream = new MemoryStream();
            var zipMs = new MemoryStream();
            using (ZipOutputStream zipStream = new ZipOutputStream(zipMs))
            {
                zipStream.SetLevel(9);
                foreach (var kv in streams)
                {
                    string fileName = kv.Key;
                    using (var streamInput = kv.Value)
                    {
                        zipStream.PutNextEntry(new ZipEntry(fileName));
                        while (true)
                        {
                            var readCount = streamInput.Read(buffer, 0, buffer.Length);
                            if (readCount > 0)
                            {
                                zipStream.Write(buffer, 0, readCount);
                            }
                            else
                            {
                                break;
                            }
                        }
                        zipStream.Flush();
                    }
                }
                zipStream.Finish();
                zipMs.Position = 0;
                zipMs.CopyTo(returnStream, 5600);
            }
            returnStream.Position = 0;
            return returnStream;
        }


        public static void Download(IEnumerable<string> files, string zipFileName,string path,string deletepath)
        {
            HttpResponse Response = HttpContext.Current.Response;
            //根据所选文件打包下载
            MemoryStream ms = new MemoryStream();
            byte[] buffer = null;

            using (ZipFile file = ZipFile.Create(ms))
            {
                file.BeginUpdate();
                file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。

                file.Add(path);
                file.CommitUpdate();
                buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
            }

            //删除没用的文件
            FileHelper.DeleteFolder(deletepath);

            Response.AddHeader("content-disposition", "attachment;filename=" + zipFileName);
            Response.BinaryWrite(buffer);
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// 将目录制作成压缩文件
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="s">ZipOutputStream对象</param>
        public static void Compress(string source, ZipOutputStream s)
        {
            string[] filenames = Directory.GetFileSystemEntries(source);
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    Compress(file, s);  //递归压缩子文件夹
                }
                else
                {
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[4 * 1024];
                        //Path.GetPathRoot(file)
                        string path= HttpContext.Current.Server.MapPath("~/Upfile/downzip/");//去掉的原路径
                        ZipEntry entry = new ZipEntry(file.Replace(path, ""));     //此处去掉盘符，如D:\123\1.txt 去掉D:
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);

                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
            }
        }
    }
    

    public class MyNameTransfom : ICSharpCode.SharpZipLib.Core.INameTransform
    {

        #region INameTransform 成员

        public string TransformDirectory(string name)
        {
            return null;
        }

        public string TransformFile(string name)
        {
            return Path.GetFileName(name);
        }

        #endregion
    }
}