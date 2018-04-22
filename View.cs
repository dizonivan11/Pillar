using System;
using System.Collections.Generic;
using CSScriptLibrary;
using System.Text;

namespace Pillar
{
    public static class View
    {
        // Contains extensions for string
        const string CSSTART = "<csharp>";
        const string CSEND = "</csharp>";
        const string CSFSTART = "<csharp-func>";
        const string CSFEND = "</csharp-func>";
        const string CSCSTART = "<csharp-class>";
        const string CSCEND = "</csharp-class>";
        private const string ScriptFormat =
                    "using System;" +
                    "using System.Collections.Generic;" +
                    "using System.Collections.Specialized;" +
                    "using System.Text;" +

                    "public class Script {{" +
                    "   StringBuilder returnValue = new StringBuilder();" +
                    "   public Dictionary<string, object> VAR = new Dictionary<string, object>();" +

                    "   public void Echo(string format, params object[] args) {{" +
                    "       returnValue.Append(string.Format(format, args));" +
                    "   }}" +

                    "   public string Run(Dictionary<string, string> POST, Dictionary<string, string> GET, Dictionary<string, object> VAR) {{" +
                    "       this.VAR = VAR;" +
                    "       try {{" +
                    "           {0}" +
                    "       }} catch (Exception ex) {{" +
                    "           Echo(\"<b style='color: red;'>RUNTIME ERROR: \" + ex.Message + \"</b>\");" +
                    "       }}" +
                    "       return returnValue.ToString();" +
                    "   }}" +
                    
                    "   {1}" +
                    "}}" +
                    
                    "{2}";

        public static string ToHtml(this string phtml, Dictionary<string, string> POST, Dictionary<string, string> GET) {
            // normalize html variable first before returning it.
            // This variable contains internal CSharp code (using special tags) which must be converted to html.
            string html = phtml;
            
            StringBuilder classes = new StringBuilder();
            int startCTag = html.IndexOf(CSCSTART);
            while (startCTag > -1) {
                int endCTag = html.IndexOf(CSCEND);
                string csCTag = html.Substring(startCTag, endCTag - startCTag + CSCEND.Length);
                string csCCode = csCTag.Substring(CSCSTART.Length, csCTag.Length - CSCSTART.Length - CSCEND.Length);

                classes.AppendLine(csCCode);
                html = html.Replace(csCTag, "");
                startCTag = html.IndexOf(CSCSTART);
            }

            StringBuilder functions = new StringBuilder();
            int startFTag = html.IndexOf(CSFSTART);
            while (startFTag > -1) {
                int endFTag = html.IndexOf(CSFEND);
                string csFTag = html.Substring(startFTag, endFTag - startFTag + CSFEND.Length);
                string csFCode = csFTag.Substring(CSFSTART.Length, csFTag.Length - CSFSTART.Length - CSFEND.Length);

                functions.AppendLine(csFCode);
                html = html.Replace(csFTag, "");
                startFTag = html.IndexOf(CSFSTART);
            }
            
            Dictionary<string, object> VAR = new Dictionary<string, object>();
            int startTag = html.IndexOf(CSSTART);
            while (startTag > -1) {
                int endTag = html.IndexOf(CSEND);
                string csTag = html.Substring(startTag, endTag - startTag + CSEND.Length);
                string csCode = csTag.Substring(CSSTART.Length, csTag.Length - CSSTART.Length - CSEND.Length);
                
                try {
                    dynamic script = CSScript.Evaluator.LoadCode(string.Format(ScriptFormat, csCode, functions.ToString(), classes.ToString()));
                    string returnValue = script.Run(POST, GET, VAR);
                    VAR = script.VAR;
                    html = html.Replace(csTag, returnValue);
                } catch (Exception ex) {
                    string errorHtml = string.Empty;
                    errorHtml += "<!DOCTYPE html>";
                    errorHtml += "<html>";
                    errorHtml += "    <head>";
                    errorHtml += "        <title>COMPILE ERROR</title>";
                    errorHtml += "    </head>";
                    errorHtml += "    <body>";
                    errorHtml += "        <h1>COMPILE ERROR</h1>";
                    errorHtml += "        <h3>" + ex.Message + "</h3>";
                    errorHtml += "    </body>";
                    errorHtml += "</html>";
                    return errorHtml;
                }

                startTag = html.IndexOf(CSSTART);
            }

            return html;
        }
    }
}