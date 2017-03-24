using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
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
    }
}
