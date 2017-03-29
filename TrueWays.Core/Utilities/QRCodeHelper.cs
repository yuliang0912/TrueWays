using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueWays.Core.Utilities
{
    public class QrCodeHelper
    {
        public static bool GetQrCode(string strContent, MemoryStream ms)
        {
            var ecl = ErrorCorrectionLevel.M; //误差校正水平   
            var content = strContent;//待编码内容  
            var quietZones = QuietZoneModules.Two;  //空白区域   
            var moduleSize = 17;//大小  
            var encoder = new QrEncoder(ecl);
            QrCode qr;
            if (encoder.TryEncode(content, out qr))//对内容进行编码，并保存生成的矩阵  
            {
                var render = new GraphicsRenderer(new FixedModuleSize(moduleSize, quietZones));
                render.WriteToStream(qr.Matrix, ImageFormat.Png, ms);
            }
            else
            {
                return false;
            }
            return true;
        }

        public static void Generate5(string strContent, string logoPath, MemoryStream ms)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(strContent);

            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(17, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            Graphics g = Graphics.FromImage(map);
            render.Draw(g, qrCode.Matrix);

            Image middlImg = Image.FromFile(logoPath);
            int middleImgW = Math.Min((int)(map.Width / 3.5), middlImg.Width);
            int middleImgH = Math.Min((int)(map.Height / 3.5), middlImg.Height);
            int middleImgL = (map.Width - middleImgW) / 2;
            int middleImgT = (map.Height - middleImgH) / 2;

            Point imgPoint = new Point(middleImgL, middleImgT);
            g.DrawImage(middlImg, middleImgL, middleImgT, middleImgW, middleImgH);
            map.Save(ms, ImageFormat.Png);
        }

        public static MemoryStream GetQRCode(string content, string iconPath, int moduleSize = 17)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode = qrEncoder.Encode(content);

            GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(moduleSize, QuietZoneModules.Two), Brushes.Black, Brushes.White);

            DrawingSize dSize = render.SizeCalculator.GetSize(qrCode.Matrix.Width);
            Bitmap map = new Bitmap(dSize.CodeWidth, dSize.CodeWidth);
            Graphics g = Graphics.FromImage(map);
            render.Draw(g, qrCode.Matrix);

            //追加Logo图片 ,注意控制Logo图片大小和二维码大小的比例
            //PS:追加的图片过大超过二维码的容错率会导致信息丢失,无法被识别
            Image img = Image.FromFile(iconPath);

            Point imgPoint = new Point((map.Width - img.Width) / 2, (map.Height - img.Height) / 2);
            g.DrawImage(img, imgPoint.X, imgPoint.Y, img.Width, img.Height);

            MemoryStream memoryStream = new MemoryStream();
            map.Save(memoryStream, ImageFormat.Png);

            return memoryStream;

            //生成图片的代码： map.Save(fileName, ImageFormat.Jpeg);//fileName为存放的图片路径
        }
}
}
