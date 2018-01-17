using System;
using System.Web;

public class ReplacerModule : IHttpModule
{
    public void Init(HttpApplication context)
    {
        context.PostRequestHandlerExecute += this.OnPostRequestHandlerExecute;
    }

    public void Dispose()
    {
    }

    private void OnPostRequestHandlerExecute(object sender, EventArgs eventArgs)
    {
        var context = ((HttpApplication)sender).Context;

        if (context.Response.ContentType.Equals("text/html"))
        {
            context.Response.Filter = new ReplacerStream(context.Response.Filter, context.Response.ContentEncoding);
        }
    }
}