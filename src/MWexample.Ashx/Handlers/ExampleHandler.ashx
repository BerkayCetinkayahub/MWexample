<%@ WebHandler Language="C#" Class="ExampleHandler" %>

using System;
using System.Web;

public class ExampleHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Write("Hello from ExampleHandler!");
    }

    public bool IsReusable
    {
        get { return false; }
    }
}