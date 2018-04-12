using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace Pillar
{
    public class PillarProcessor
    {
        public static string ROOT = "C:/pillar/sites/";

        public HttpListenerRequest request;
        public HttpListenerResponse response;

        public PillarProcessor(HttpListenerRequest request, HttpListenerResponse response) {
            this.request = request;
            this.response = response;
        }

        public void Start() {
            new Thread(delegate () {
                try {
                    string requestUrl = request.RawUrl.Split('?')[0];
                    if (requestUrl[requestUrl.Length - 1] == '/') requestUrl += "index.phtml";

                    string mediaType = "phtml";
                    if (requestUrl.IndexOf('.') > -1) mediaType = requestUrl.Split('.')[1].ToLower();
                    else {
                        requestUrl += ".phtml";
                        mediaType = "phtml";
                    }

                    string method = request.HttpMethod.ToUpperInvariant();
                    Console.WriteLine("{0}: {1}", method, requestUrl);

                    Dictionary<string, string> POST = new Dictionary<string, string>();
                    if (method == "POST") {
                        StreamReader body = new StreamReader(request.InputStream, request.ContentEncoding);
                        string postContent = body.ReadToEnd();

                        if (postContent.IndexOf('&') > -1) {
                            string[] keyvalues = postContent.Split('&');

                            for (int i = 0; i < keyvalues.Length; i++) {
                                if (keyvalues[i].IndexOf('=') > -1) {
                                    string[] keyvalue = keyvalues[i].Split(new string[] { "=" }, StringSplitOptions.None);
                                    POST.Add(keyvalue[0], keyvalue[1]);
                                }
                            }
                        } else if (postContent.IndexOf('=') > -1) {
                            string[] keyvalue = postContent.Split(new string[] { "=" }, StringSplitOptions.None);
                            POST.Add(keyvalue[0], keyvalue[1]);
                        }
                    }
                    Dictionary<string, string> GET = new Dictionary<string, string>();
                    foreach (string key in request.QueryString.Keys) GET.Add(key, request.QueryString[key]);

                    foreach (string key in POST.Keys) Console.WriteLine("POST-DATA: {0}={1}", key, POST[key]);
                    foreach (string key in GET.Keys) Console.WriteLine("GET-DATA: {0}={1}", key, GET[key]);
                    
                    byte[] buffer = new byte[0];
                    switch (mediaType) {
                        case "phtml":
                            try {
                                string phtml = File.ReadAllText(ROOT + requestUrl);
                                buffer = request.ContentEncoding.GetBytes(phtml.ToHtml(POST, GET));
                            } catch { buffer = request.ContentEncoding.GetBytes("Page not Found"); }
                            break;
                        default:
                            try {
                                buffer = File.ReadAllBytes(ROOT + requestUrl);
                            } catch { buffer = new byte[0]; }
                            break;
                    }

                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Flush();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }).Start();
        }
    }
}
