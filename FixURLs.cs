using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Web.Configuration;
using webSiteEngineer;

/// <summary>
/// Summary description for FixURLs
/// *********************************
/// This will mask the true file name and querystrings from the user 
/// and display something simpler to the client; a 'friendly URL'
/// *********************************
/// </summary>
public class FixURLs : IHttpModule
{
	public FixURLs()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    #region IHttpModule Members

    public void Dispose()
    {
        // do nothing
    }

    public void Init(HttpApplication context)
    {
        context.BeginRequest += new EventHandler(context_BeginRequest);
    }

    #endregion

    void context_BeginRequest(object sender, EventArgs e)
    {
        //new hash table to hold the translation pairs
		//find a way to pull this out of the DB!
		Hashtable mapURL = new Hashtable();
        Article urlList = new Article();
        int siteID = Convert.ToInt32(WebConfigurationManager.AppSettings.GetValues("siteID")[0]);

		HttpApplication app = (HttpApplication)sender;
		
        string defaultPage = "default.aspx";
        string defaultQueryString = "pageID=";
		string CorrectHost = WebConfigurationManager.AppSettings.GetValues("siteURL")[0];
		int segment = app.Request.Url.Segments.GetUpperBound(0);
		string searchURLpath = app.Request.Url.Segments[segment].ToLower();

        try
        {
            //switch to secure if appropriate
            if (!app.Request.IsSecureConnection)
            {
                //now what?
            }

            //SEO Style- redirect to canonical URL if not there
            string hostString = app.Request.Url.Host.ToLower();
            //better to test if exact match...
            if (hostString != CorrectHost)
            {
                //build URL to redirect to
                string redirectPath;
                if (searchURLpath != "")
                {
                    redirectPath = "http://" + CorrectHost + "/" + searchURLpath + app.Request.Url.Query;
                }
                else
                {
                    redirectPath = "http://" + CorrectHost;
                }

                //redirect to correct host
                app.Response.Status = "301 Moved Permanently";
                app.Response.AddHeader("Location", redirectPath);
            }

            mapURL = urlList.GetURLlist(siteID);
            foreach (DictionaryEntry URLpath in mapURL)
            {
                //check if we are even looking at the page (and not some sub-aspect)
                if (searchURLpath.Contains(".aspx"))
                {
                    if (searchURLpath.Contains(URLpath.Key.ToString().ToLower()))
                    {
                        app.Context.RewritePath(defaultPage, "", defaultQueryString + URLpath.Value.ToString());
                        break;
                    }
                    else
                    {
                        //we can't find a match so we let it pass thru
                    }
                }
            }
        }
        catch (Exception err)
        {
            Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
            Errors.LogError(UserID, "FixURLs.cs", err.Source, "Error parsing address from database", err.Message, err.InnerException, err.StackTrace);
            //throw new Exception("Error parsing address from database: " + err.Message, err.InnerException);
        }
	}
}
