using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using CSScriptLibrary;
using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace Pillar {
    public static class View {
        // Contains extensions for string
        const string CSSTART = "<csharp>";
        const string CSEND = "</csharp>";

        public static string ToHtml(this string phtml, NameValueCollection POST, NameValueCollection GET) {
            // normalize html variable first before returning it.
            // This variable contains internal CSharp code (using special tags) which must be converted to html.
            string html = phtml;
            
            int startTag = html.IndexOf(CSSTART);
            while (startTag > -1) {
                int endTag = html.IndexOf(CSEND);
                string csTag = html.Substring(startTag, endTag - startTag + CSEND.Length);
                string csCode = csTag.Substring(CSSTART.Length, csTag.Length - CSSTART.Length - CSEND.Length);

                CSharpCodeProvider cscp = new CSharpCodeProvider();
                string format =
                    "using System;" +
                    "using System.Collections.Generic;" +
                    "using System.Text;" +
                    // css_include <filename>
                    "" +
                    "public class Script {{" +
                    "   StringBuilder returnValue = new StringBuilder();" +
                    "   DataManager POST = new DataManager();" +
                    "   DataManager GET = new DataManager();" +
                    "   VariableManager VAR = new VariableManager();" +
                    "" +
                    "   public Script() {{" +
                    "       {0}" +
                    "   }}" +
                    "" +
                    "   public void Echo(string format, params object[] args) {{" +
                    "       returnValue.Append(string.Format(format, args));" +
                    "   }}" +
                    "" +
                    "   public string Main() {{" +
                    "       try {{" +
                    "           {1}" +
                    "       }} catch (Exception ex) {{" +
                    "           Echo(\"<b style='color: red;'>RUNTIME ERROR: \" + ex.Message + \"</b>\");" +
                    "       }}" +
                    "       return returnValue.ToString();" +
                    "   }}" +
                    "}}" +
                    "" +
                    "public class DataManager : Dictionary<string, string> {{" +
                    "   public string this[string key] {{" +
                    "       get {{" +
                    "           if (!ContainsKey(key)) Add(key, string.Empty);" +
                    "           return base[key];" +
                    "       }}" +
                    "   }}" +
                    "}}" +
                    "" +
                    "public class VariableManager : Dictionary<string, object> {{" +
                    "   public object this[string key] {{" +
                    "       get {{" +
                    "           if (!ContainsKey(key)) Add(key, null);" +
                    "           return base[key];" +
                    "       }}" +
                    "       set {{" +
                    "           if (!ContainsKey(key)) Add(key, value);" +
                    "           else base[key] = value;" +
                    "       }}" +
                    "   }}" +
                    "}}";

                try {
                    StringBuilder data = new StringBuilder();
                    foreach (string key in POST.Keys)
                        data.AppendFormat("     POST.Add(\"{0}\", \"{1}\");\n",
                            HttpUtility.UrlDecode(key),
                            HttpUtility.UrlDecode(POST[key]));
                    foreach (string key in GET.Keys)
                        data.AppendFormat("     GET.Add(\"{0}\", \"{1}\");\n",
                            HttpUtility.UrlDecode(key),
                            HttpUtility.UrlDecode(GET[key]));
                    dynamic script = CSScript.Evaluator.LoadCode(string.Format(format, data, csCode));
                    string returnValue = script.Main();
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
