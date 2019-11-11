using System;
using System.Collections.Generic;
using System.Text;

namespace Dairy {
    public class Export {
        public string Content { get; private set; }

        private Export() { }

        public Export(ExportEcoding ecoding, List<DB.Dairy> dairies) {
            Content = GetContent(ecoding, dairies);
        }

        private string GetContent(ExportEcoding ecoding, List<DB.Dairy> dairies) {
            string result;
            switch (ecoding) {
                case ExportEcoding.text:
                    result = GenerateContentWithText(dairies);
                    break;
                case ExportEcoding.html:
                    result = GenerateContentWithHtml(dairies);
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }

        public enum ExportEcoding {
            text,
            html
        }

        private string GenerateContentWithText(List<DB.Dairy> dairies) {
            var sb = new StringBuilder();
            foreach (var dairy in dairies) {
                sb.AppendLine($"{dairy.WroteDate}      {dairy.Wheather}");
                sb.AppendLine(dairy.Thema);
                sb.AppendLine(dairy.Content);
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private string GenerateContentWithHtml(List<DB.Dairy> dairies) {
            var sb = new StringBuilder();
            foreach (var dairy in dairies) {
                sb.AppendLine($"<h2><span>{dairy.WroteDate}</span><span style=\"margin: 0 0 0 40; \">天气：</span><span>{dairy.Wheather}</span></h2>");
                sb.AppendLine($"<h4>{dairy.Thema}</h4>");
                var sections = dairy.Content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var sction in sections) {
                    sb.AppendLine($"<p>{sction}</p>");
                }
                sb.AppendLine("<hr/>");
                sb.AppendLine();
                sb.AppendLine();
            }
            sb.Remove(sb.Length - 11, 7);
            return sb.ToString();
        }
    }
}
