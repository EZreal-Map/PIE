using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SZRiverSys.Comm
{
    public class FileHelper
    {
        /// <summary>
        /// 复制文件或文件夹下的文件
        /// </summary>
        /// <param name="srcPath">源目录文件</param>
        /// <param name="aimPath">目标文件</param>
        public void copyDir(string srcPath, string aimPath)
        {
            try
            {
                //如果不存在目标路径，则创建之
                if (!System.IO.Directory.Exists(aimPath))
                {
                    System.IO.Directory.CreateDirectory(aimPath);
                }
                //令目标路径为aimPath\srcPath
                string srcdir = System.IO.Path.Combine(aimPath, System.IO.Path.GetFileName(srcPath));
                //如果源路径是文件夹，则令目标目录为aimPath\srcPath\
                if (Directory.Exists(srcPath))
                    srcdir += Path.DirectorySeparatorChar;
                // 如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(srcdir))
                {
                    System.IO.Directory.CreateDirectory(srcdir);
                }
                //获取源文件下所有的文件
                String[] files = Directory.GetFileSystemEntries(srcPath);
                foreach (string element in files)
                {
                    //如果是文件夹，循环
                    if (Directory.Exists(element))
                        copyDir(element, srcdir);
                    else
                        File.Copy(element, srcdir + Path.GetFileName(element), true);
                }
            }
            catch
            {
                Console.WriteLine("无法复制");
            }
        }


        /// <summary>
        /// 删除整个文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {

            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        FileInfo fi = new FileInfo(d);
                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                            fi.Attributes = FileAttributes.Normal;
                        File.Delete(d);//直接删除其中的文件   
                    }
                    else
                        DeleteFolder(d);//递归删除子文件夹   
                }
                Directory.Delete(dir);//删除已空文件夹   
            }
        }
        private static void CopyFolder(string from, string to)
        {
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);
            // 子文件夹
            foreach (string sub in Directory.GetDirectories(from))
                CopyFolder(sub + "\\", to + Path.GetFileName(sub) + "\\");
            // 文件
            foreach (string file in Directory.GetFiles(from))
                File.Copy(file, to + Path.GetFileName(file), true);
        }


    }
}