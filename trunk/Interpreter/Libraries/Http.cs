using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Shiro.Interpreter;
using Shiro.Interop;
using Kayak;
using Kayak.Http;

namespace Shiro.Libraries
{
    internal enum HttpMode
    {
        SimpleHandler = 1,
        ObjectMap = 2,
        FileSystem = 3
    }
    
    [ShiroClass("HttpInternal", KeepClassLoaded = true)]
    public class Http : ShiroBase
    {
        internal static string Callback;
        internal static Http ActiveHttp;

        internal static IScheduler scheduler;
        

        [ShiroMethod("httpStartInt", 2)]
        public void httpStart(int port, Token callback)
        {
            scheduler = KayakScheduler.Factory.Create(new SchedulerDelegate());
            var server = KayakServer.Factory.CreateHttp(new RequestDelegate(), scheduler);

            Callback = callback.token;
            Http.ActiveHttp = this;

            using (server.Listen(new IPEndPoint(IPAddress.Any, port)))
            {
                //Begin the long wait...
                scheduler.Start();
            }
        }

        [ShiroMethod("httpStop", 0)]
        public string httpStop()
        {
            scheduler.Stop();
            return "";
        }

        #region Internal classes

        class RequestDelegate : IHttpRequestDelegate
        {
            public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
            {
                //Handler format:  fun([URI])  -or-  fun([path], [qs])
                try
                {
                    var args = Http.ActiveHttp.Parser.SymbolTable.GetFArgs(Callback);
                    string body = "";
                    Token rawBody;

                    if (args.Count == 0)
                        rawBody = Http.ActiveHttp.Parser.ParseFunctionCall(Callback, new List<Token>());
                    else if (args.Count == 1)
                    {
                        List<Token> argVals = new List<Token>();
                        argVals.Add(Token.FromString(request.Uri));

                        rawBody = Http.ActiveHttp.Parser.ParseFunctionCall(Callback, argVals);
                    }
                    else
                    {
                        List<Token> argVals = new List<Token>();
                        argVals.Add(Token.FromString(request.Path ?? "/"));
                        argVals.Add(Token.FromString(request.QueryString ?? ""));

                        rawBody = Http.ActiveHttp.Parser.ParseFunctionCall(Callback, argVals);
                    }

                    string status = "200 OK",
                           contentType = "text/plain";
                    

                    if (rawBody.IsObject && rawBody.baseClass == "ResponseDetails")
                    {
                        body = rawBody.GetObjectPropertyValue("Body").ToString();
                        status = rawBody.GetObjectPropertyValue("Status").ToString();
                        contentType = rawBody.GetObjectPropertyValue("ContentType").ToString();
                    } else
                        body = rawBody.ToString();

                    var headers = new HttpResponseHead()
                    {
                        Status = status,
                        Headers = new Dictionary<string, string>() 
                        {
                            { "Content-Type", contentType },
                            { "Content-Length", body.Length.ToString() },
                        }
                    };
                    response.OnResponse(headers, new BufferedProducer(body));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MerlinException: " + ex.Message);
                    throw;
                }
            }
        }

        class SchedulerDelegate : ISchedulerDelegate
        {
            public void OnException(IScheduler scheduler, Exception e)
            {
                e.DebugStackTrace();
            }

            public void OnStop(IScheduler scheduler)
            {

            }
        }
        #endregion

        #region Datastream classes

        class BufferedProducer : IDataProducer
        {
            ArraySegment<byte> data;

            public BufferedProducer(string data) : this(data, Encoding.UTF8) { }
            public BufferedProducer(string data, Encoding encoding) : this(encoding.GetBytes(data)) { }
            public BufferedProducer(byte[] data) : this(new ArraySegment<byte>(data)) { }
            public BufferedProducer(ArraySegment<byte> data)
            {
                this.data = data;
            }

            public IDisposable Connect(IDataConsumer channel)
            {
                // null continuation, consumer must swallow the data immediately.
                channel.OnData(data, null);
                channel.OnEnd();
                return null;
            }
        }

        class BufferedConsumer : IDataConsumer
        {
            List<ArraySegment<byte>> buffer = new List<ArraySegment<byte>>();
            Action<string> resultCallback;
            Action<Exception> errorCallback;

            public BufferedConsumer(Action<string> resultCallback, Action<Exception> errorCallback)
            {
                this.resultCallback = resultCallback;
                this.errorCallback = errorCallback;
            }
            public bool OnData(ArraySegment<byte> data, Action continuation)
            {
                // since we're just buffering, ignore the continuation. 
                // TODO: place an upper limit on the size of the buffer. 
                // don't want a client to take up all the RAM on our server! 
                buffer.Add(data);
                return false;
            }
            public void OnError(Exception error)
            {
                errorCallback(error);
            }

            public void OnEnd()
            {
                // turn the buffer into a string. 
                // 
                // (if this isn't what you want, you could skip 
                // this step and make the result callback accept 
                // List<ArraySegment<byte>> or whatever) 
                // 
                var str = buffer
                    .Select(b => Encoding.UTF8.GetString(b.Array, b.Offset, b.Count))
                    .Aggregate((result, next) => result + next);

                resultCallback(str);
            }
        }
        #endregion
    }
}
