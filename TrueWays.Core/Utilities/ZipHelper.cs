using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

namespace TrueWays.Core.Utilities
{
    /// <summary>
    /// Zip压缩与解压缩 
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件</param>
        /// <param name="zipedFile">压缩后的文件</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="blockSize">每次写入大小</param>
        public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (var zipFile = File.Create(zipedFile))
            {
                using (var zipStream = new ZipOutputStream(zipFile))
                {
                    using (var streamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);

                        var zipEntry = new ZipEntry(fileName);

                        zipStream.PutNextEntry(zipEntry);

                        zipStream.SetLevel(compressionLevel);

                        var buffer = new byte[blockSize];

                        var sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                                zipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        streamToZip.Close();
                    }

                    zipStream.Finish();
                    zipStream.Close();
                }

                zipFile.Close();
            }
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        public static void ZipFile(string fileToZip, string zipedFile)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (var fs = File.OpenRead(fileToZip))
            {
                var buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                using (var zipFile = File.Create(zipedFile))
                {
                    using (var zipStream = new ZipOutputStream(zipFile))
                    {
                        var fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        var zipEntry = new ZipEntry(fileName);
                        zipStream.PutNextEntry(zipEntry);
                        zipStream.SetLevel(5);

                        zipStream.Write(buffer, 0, buffer.Length);
                        zipStream.Finish();
                        zipStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 压缩多层目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="zipedFile">The ziped file.</param>
        public static void ZipFileDirectory(string strDirectory, string zipedFile)
        {
            using (var zipFile = File.Create(zipedFile))
            {
                using (var s = new ZipOutputStream(zipFile))
                {
                    var zipedFileName = Path.GetFileName(zipedFile);
                    ZipSetp(strDirectory, s, "", zipedFileName);
                    s.Flush();
                }
            }
        }

        /// <summary>
        /// 递归遍历目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="s">The ZipOutputStream Object.</param>
        /// <param name="parentPath">The parent path.</param>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath, string zipedFileName)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.AltDirectorySeparatorChar;
            }
            var crc = new Crc32();//循环冗余校验码
            var filenames = Directory.GetFileSystemEntries(strDirectory).Where(p => zipedFileName == "" || !p.Contains(zipedFileName)).ToArray();
            foreach (var file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    var pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("\\") + 1);
                    pPath += "\\";
                    var entry = new ZipEntry(pPath);
                    s.PutNextEntry(entry);
                    ZipSetp(file, s, pPath, "");
                }
                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    using (var fs = File.OpenRead(file))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        var fileName = file.Substring(file.LastIndexOf("\\") + 1);
                        var entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;

                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);

                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="strDirectory">The STR directory.</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZip(string zipedFile, string strDirectory, string password, bool overWrite)
        {

            if (strDirectory == "")
                strDirectory = Directory.GetCurrentDirectory();
            if (!strDirectory.EndsWith("\\"))
                strDirectory = strDirectory + "\\";

            using (var s = new ZipInputStream(File.OpenRead(zipedFile)))
            {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var directoryName = "";
                    var pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    var fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(strDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                        {
                            using (var streamWriter = File.Create(strDirectory + directoryName + fileName))
                            {
                                var size = 2048;
                                var data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                s.Close();
            }
        }
    }
}