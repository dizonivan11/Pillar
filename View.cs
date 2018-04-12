using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.CSharp;
using CSScriptLibrary;

namespace Pillar {
    public static class View {
        // Contains extensions for string
        const string CSSTART = "<csharp>";
        const string CSEND = "</csharp>";

        public static string ToHtml(this string phtml, Dictionary<string, string> POST, Dictionary<string, string> GET) {
            // normalize html variable first before returning it.
            // This variable contains internal CSharp code (using special tags) which must be converted to html.
            string html = phtml;

            Dictionary<string, object> VAR = new Dictionary<string, object>();
            int startTag = html.IndexOf(CSSTART);
            while (startTag > -1) {
                int endTag = html.IndexOf(CSEND);
                string csTag = html.Substring(startTag, endTag - startTag + CSEND.Length);
                string csCode = csTag.Substring(CSSTART.Length, csTag.Length - CSSTART.Length - CSEND.Length);

                CSharpCodeProvider cscp = new CSharpCodeProvider();
                string format =
                    "using System;" +
                    "using System.Collections.Generic;" +
                    "using System.Collections.Specialized;" +
                    "using System.Text;" +
                    // css_include <filename>
                    "" +
                    "public class Script {{" +
                    "   StringBuilder returnValue = new StringBuilder();" +
                    "   public Dictionary<string, string> POST = new Dictionary<string, string>();" +
                    "   public Dictionary<string, string> GET = new Dictionary<string, string>();" +
                    "   public Dictionary<string, object> VAR = new Dictionary<string, object>();" +
                    "" +
                    "   public void Echo(string format, params object[] args) {{" +
                    "       returnValue.Append(string.Format(format, args));" +
                    "   }}" +
                    "" +
                    "   public string Run(Dictionary<string, string> POST, Dictionary<string, string> GET, Dictionary<string, object> VAR) {{" +
                    "       this.POST = POST;" +
                    "       this.GET = GET;" +
                    "       this.VAR = VAR;" +
                    "       try {{" +
                    "           {0}" +
                    "       }} catch (Exception ex) {{" +
                    "           Echo(\"<b style='color: red;'>RUNTIME ERROR: \" + ex.Message + \"</b>\");" +
                    "       }}" +
                    "       return returnValue.ToString();" +
                    "   }}" +
                    "}}";

                try {
                    dynamic script = CSScript.Evaluator.LoadCode(string.Format(format, csCode));
                    string returnValue = script.Run(POST, GET, VAR);
                    VAR = script.VAR;
                    html = html.Replace(csTag, returnValue);
                }
                catch (Exception ex) {
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
