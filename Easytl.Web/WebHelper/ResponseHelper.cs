using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Web;
using System.Reflection;

namespace Easytl.Web.WebHelper
{
    public class ResponseHelper : WebHelper
    {
        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="length">随机码长度</param>
        /// <param name="RandomCode">随机码</param>
        public static string CreateVerificationImage(int length, string RandomCode = "")
        {
            string randomcode = string.Empty;
            if (string.IsNullOrEmpty(RandomCode))
            {
                //生成随机码
                int randint;
                char code;

                //生成一定长度的验证码
                System.Random random = new Random();
                for (int i = 0; i < length; i++)
                {
                    randint = random.Next();

                    if (randint % 3 == 0)
                    {
                        code = (char)('A' + (char)(randint % 26));
                    }
                    else
                    {
                        code = (char)('0' + (char)(randint % 10));
                    }

                    randomcode += code.ToString();
                }
            }
            else
            {
                //生成固定码
                randomcode = RandomCode;
            }

            int randAngle = 45; //随机转动角度
            int mapwidth = (int)(randomcode.Length * 16);
            Bitmap map = new Bitmap(mapwidth, 22);//创建图片背景
            Graphics graph = Graphics.FromImage(map);
            graph.Clear(Color.AliceBlue);//清除画面，填充背景
            graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框
            //graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//模式

            Random rand = new Random();

            //背景噪点生成
            Pen blackPen = new Pen(Color.LightGray, 0);
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(0, map.Width);
                int y = rand.Next(0, map.Height);
                graph.DrawRectangle(blackPen, x, y, 1, 1);
            }


            //验证码旋转，防止机器识别
            char[] chars = randomcode.ToCharArray();//拆散字符串成单字符数组

            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
            //定义字体
            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            int cindex = rand.Next(7);

            for (int i = 0; i < chars.Length; i++)
            {
                int findex = rand.Next(5);

                Font f = new System.Drawing.Font(font[findex], 14, System.Drawing.FontStyle.Bold);//字体样式(参数2为字体大小)
                Brush b = new System.Drawing.SolidBrush(c[cindex]);

                Point dot = new Point(14, 14);
                //graph.DrawString(dot.X.ToString(),fontstyle,new SolidBrush(Color.Black),10,150);//测试X坐标显示间距的
                float angle = rand.Next(-randAngle, randAngle);//转动的度数

                graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                //graph.DrawString(chars[i].ToString(),fontstyle,new SolidBrush(Color.Blue),1,1,format);
                graph.RotateTransform(-angle);//转回去
                graph.TranslateTransform(-2, -dot.Y);//移动光标到指定位置，每个字符紧凑显示，避免被软件识别
            }
            //生成图片
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/gif";
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            graph.Dispose();
            map.Dispose();

            return randomcode;
        }


        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="Columns">列名和显示名的键值对</param>
        /// <param name="DataList">数据列表</param>
        /// <param name="ExcelName">导出的excel文件名称</param>
        /// <param name="encodingType">字符集</param>
        public static void ExportExcel<T>(Dictionary<string, string> Columns, List<T> DataList, string ExcelName, string encodingType = "UTF-8")
        {
            Type TType = typeof(T);

            Dictionary<PropertyInfo, string> ColumnPros = new Dictionary<PropertyInfo, string>();
            foreach (string ProName in Columns.Keys)
            {
                ColumnPros.Add(TType.GetProperty(ProName), Columns[ProName]);
            }

            string shtnl = "";
            shtnl = "<table border='1' cellspacing='1' cellpadding='1'>";
            shtnl = shtnl + "<thead>";

            foreach (string Th_Text in ColumnPros.Values)
            {
                shtnl = shtnl + "<th>" + Th_Text + "</th>";
            }
            shtnl = shtnl + "</thead><tbody>";

            for (int i = 0; i < DataList.Count; i++)
            {
                shtnl = shtnl + "<tr>";
                foreach (PropertyInfo Pro in ColumnPros.Keys)
                {
                    shtnl = shtnl + "<td>" + Pro.GetValue(DataList[i], null) + "</td>";
                }
                shtnl = shtnl + "</tr>";
            }
            shtnl = shtnl + "</tbody></table>";

            string HEADER = "<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">" +
                                          "<meta http-equiv=Content-Type content=\"text/html; charset=\"gb2312\">" +
                                          "<head>" +
                                          "<!--[if gte mso 9]><xml>" +
                                           "<x:ExcelWorkbook>" +
                                               "<x:ExcelWorksheets>" +
                                                   "<x:ExcelWorksheet>" +
                                                       "<x:Name>Sheet1</x:Name>" +
                                                       "<x:WorksheetOptions>" +
                                                           "<x:Print>" +
                                                               "<x:ValidPrinterInfo />" +
                                                           "</x:Print>" +
                                                       "</x:WorksheetOptions>" +
                                                   "</x:ExcelWorksheet>" +
                                               "</x:ExcelWorksheets>" +
                                           "</x:ExcelWorkbook>" +
                                       "</xml>" +
                                       "<![endif]-->";

            Encoding encoding = Encoding.GetEncoding(encodingType);
            HttpContext.Current.Response.ContentEncoding = encoding;
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(ExcelName, encoding));
            HttpContext.Current.Response.ContentType = "ms-excel/application";

            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendFormat(@"{0}</head>  
                         <body>{1}</body>  
                         </html>", HEADER, shtnl);

            HttpContext.Current.Response.Write(sbHtml.ToString());
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.End();  
        }
    }
}
