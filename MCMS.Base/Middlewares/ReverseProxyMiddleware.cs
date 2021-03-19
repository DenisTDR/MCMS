using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MCMS.Base.Helpers;
using Microsoft.AspNetCore.Http;

namespace MCMS.Base.Middlewares
{
    // got from: 
    // https://auth0.com/blog/building-a-reverse-proxy-in-dot-net-core/
    // https://github.com/andychiare/netcore2-reverse-proxy
    // and adjusted by TDR

    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReverseProxyMiddleware
    {
        private static readonly HttpClient HttpClient = new();
        private readonly RequestDelegate _nextMiddleware;
        private readonly ReverseProxyMiddlewareOptions _options;

        public ReverseProxyMiddleware(RequestDelegate nextMiddleware, ReverseProxyMiddlewareOptions options)
        {
            _nextMiddleware = nextMiddleware;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!ShouldProxy(context.Request))
            {
                await _nextMiddleware(context);
            }

            var targetUri = BuildTargetUri(context.Request);
            if (targetUri != null)
            {
                var targetRequestMessage = CreateTargetMessage(context, targetUri);

                using var responseMessage = await HttpClient.SendAsync(targetRequestMessage,
                    HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
                context.Response.StatusCode = (int) responseMessage.StatusCode;

                CopyFromTargetResponseHeaders(context, responseMessage);

                await ProcessResponseContent(context, responseMessage);
            }
        }

        private async Task ProcessResponseContent(HttpContext context, HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsByteArrayAsync();
            await context.Response.Body.WriteAsync(content);
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var requestMessage = new HttpRequestMessage();
            CopyFromOriginalRequestContentAndHeaders(context, requestMessage);

            requestMessage.RequestUri = targetUri;
            requestMessage.Headers.Host = targetUri.Host;
            requestMessage.Method = GetMethod(context.Request.Method);

            return requestMessage;
        }

        private void CopyFromOriginalRequestContentAndHeaders(HttpContext context, HttpRequestMessage requestMessage)
        {
            var requestMethod = context.Request.Method;

            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        private void CopyFromTargetResponseHeaders(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove("transfer-encoding");
        }

        private static HttpMethod GetMethod(string method)
        {
            if (HttpMethods.IsDelete(method)) return HttpMethod.Delete;
            if (HttpMethods.IsGet(method)) return HttpMethod.Get;
            if (HttpMethods.IsHead(method)) return HttpMethod.Head;
            if (HttpMethods.IsOptions(method)) return HttpMethod.Options;
            if (HttpMethods.IsPost(method)) return HttpMethod.Post;
            if (HttpMethods.IsPut(method)) return HttpMethod.Put;
            if (HttpMethods.IsTrace(method)) return HttpMethod.Trace;
            return new HttpMethod(method);
        }

        private Uri BuildTargetUri(HttpRequest request)
        {
            Uri targetUri = null;

            foreach (var (key, value) in _options.ProxyRules)
            {
                if (request.Path.StartsWithSegments(key, out var remainingPath))
                {
                    targetUri = new Uri(value + remainingPath);
                }
            }

            return targetUri;
        }

        private bool ShouldProxy(HttpRequest request)
        {
            return _options.ProxyRules.Keys.Any(key => request.Path.StartsWithSegments(key));
        }
    }
}