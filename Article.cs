using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace webSiteEngineer
{
    /// <summary>
    /// defines an article or text page for storage in database and display on web page
    /// </summary>
    public class Article
    {
        private string _Article_Title, _Article_Description, _Article_Text, _Article_Page_Description, _Article_Page_Keywords, _Article_URL, _Author, _Cat_Name, _Site_Name;
        private bool _Publish, _Deleted, _Secure;
        private int _Sort, _id_Category, _id_Site, _id_Articles;
        private DateTime _Pub_Date, _Expire_Date, _Edit_Date, _Create_Date, _Approved_Date;
        private Guid _Edit_By, _Approved_By, _Create_By;

        //used in cleaning up the properties as they are get and set
        Utility utility = new Utility();

        /******************************
         * properties
         * ***************************/
        #region PublicArticleProperties
        public int id_Articles
        {
            get { return _id_Articles; }
            set { _id_Articles = value; }
        }
        /// <summary>
        /// used for page title, trimmed to 70 char because of suggestions from http://SEOMOZ.org/ for better SEO compliance
        /// </summary>
        public string Article_Title
        {
            get { return utility.trimLength(_Article_Title, 70); }
            set { _Article_Title = utility.trimLength(value, 70); }
        }
        /// <summary>
        /// internal notes on article/page
        /// </summary>
        public string Article_Description
        {
            get { return utility.safeHTML(_Article_Description, false); }
            set { _Article_Description = utility.safeHTML(value, true); }
        }
        /// <summary>
        /// used for body text on page
        /// </summary>
        public string Article_Text
        {
            get { return utility.safeHTML(_Article_Text, false); }
            set { _Article_Text = utility.safeHTML(value, true); }
        }
        /// <summary>
        /// used for meta data on page, trimmed to 155 char because of suggestions from http://SEOMOZ.org/ for better SEO compliance
        /// </summary>
        public string Article_Page_Description
        {
            get { return utility.safeHTML(_Article_Page_Description, false, 155); }
            set { _Article_Page_Description = utility.safeHTML(value, true, 155); }
        }
        /// <summary>
        /// used for meta data on page, trimmed to 155 char because of suggestions from http://SEOMOZ.org/ for better SEO compliance
        /// </summary>
        public string Article_Page_Keywords
        {
            get { return utility.safeHTML(_Article_Page_Keywords, false, 155); }
            set { _Article_Page_Keywords = utility.safeHTML(value, true, 155); }
        }
        /// <summary>
        /// used to mask actual data driven page with friendly URL
        /// </summary>
        public string Article_URL
        {
            get { return HttpUtility.UrlDecode(_Article_URL); }
            set { _Article_URL = HttpUtility.UrlEncode(value); }
        }
        public bool Secure
        {
            get { return _Secure; }
            set { _Secure = value; }
        }
        public bool Publish
        {
            get { return _Publish; }
            set { _Publish = value; }
        }
        public bool Deleted
        {
            get { return _Deleted; }
            set { _Deleted = value; }
        }
        public int Sort
        {
            get { return _Sort; }
            set { _Sort = value; }
        }
        public String Author
        {
            get { return _Author; }
            set { _Author = value; }
        }
        public Guid Approved_By
        {
            get { return _Approved_By; }
            set { _Approved_By = value; }
        }
        public DateTime Approved_Date
        {
            get { return _Approved_Date; }
            set { _Approved_Date = value; }
        }
        /// <summary>
        /// date when article can be shown
        /// </summary>
        public DateTime Pub_Date
        {
            get { return _Pub_Date; }
            set { _Pub_Date = value; }
        }
        /// <summary>
        /// date after which article cannot be shown
        /// </summary>
        public DateTime Expire_Date
        {
            get { return _Expire_Date; }
            set { _Expire_Date = value; }
        }
        public DateTime Edit_Date
        {
            get { return _Edit_Date; }
            set { _Edit_Date = value; }
        }
        public Guid Edit_By
        {
            get { return _Edit_By; }
            set { _Edit_By = value; }
        }
        public DateTime Create_Date
        {
            get { return _Create_Date; }
            set { _Create_Date = value; }
        }
        public Guid Create_By
        {
            get { return _Create_By; }
            set { _Create_By = value; }
        }
        public int id_Category
        {
            get { return _id_Category; }
            set { _id_Category = value; }
        }
        public string Cat_Name
        {
            get { return _Cat_Name; } //I trim the articles as those are expected to be in the page title and cat names are not, so no trim
            set { _Cat_Name = value; }
        }
        public int id_Site
        {
            get { return _id_Site; }
            set { _id_Site = value; }
        }
        public string Site_Name
        {
            get { return _Site_Name; }
            set { _Site_Name = value.ToLower(); }
        }

        #endregion

        /******************************
    * methods
    * ***************************/
        public Article()
        {
            //
            // TODO: Add constructor logic here
            //
            _Sort = 99;
        }

        #region PublicArticleMethods

        /// <summary>
        /// returns article with given ID
        /// </summary>
        /// <returns>article</returns>
        public Article GetArticleByID()
        {
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                //a given article
                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand("wse_ArticleByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Parameters.AddWithValue("@id_Article", this.id_Articles);

                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (DBNull.Value != reader["Approved_By"])
                        {
                            this.Approved_By = (Guid)reader["Approved_By"];
                            this.Approved_Date = (DateTime)reader["Approved_Date"];
                        }
                        this.Article_Description = reader["Article_Description"].ToString();
                        this.Article_Text = reader["Article_Text"].ToString();
                        this.Article_Page_Description = reader["Page_Description"].ToString();
                        this.Article_Page_Keywords = reader["Page_Keywords"].ToString();
                        this.Article_Title = reader["Article_Title"].ToString();
                        this.Article_URL = reader["Article_URL"].ToString();
                        this.Author = reader["Author"].ToString();
                        this.Cat_Name = reader["Cat_Name"].ToString();
                        this.Create_Date = (DateTime)reader["Create_Date"];
                        this.Create_By = (Guid)reader["Create_by"];
                        this.Deleted = (bool)reader["Deleted"];
                        this.Edit_Date = (DateTime)reader["Edit_Date"];
                        this.Expire_Date = (DateTime)reader["Expire_Date"];
                        this.Edit_By = (Guid)reader["Edit_by"];
                        //this.id_Articles = (Int32)reader["id_Articles"];
                        this.id_Category = (Int32)reader["id_Category"];
                        this.id_Site = (Int32)reader["id_Site"];
                        this.Pub_Date = (DateTime)reader["Pub_Date"];
                        this.Publish = (bool)reader["Publish"];
                        this.Secure = (bool)reader["Secure"];
                        if (DBNull.Value != reader["Site_Name"])
                        {
                            this.Site_Name = reader["Site_Name"].ToString();
                        }
                        this.Sort = (Int32)reader["Sort"];
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Article.cs", err.Source, "Reading the article an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the article was an issue: " + err.Message + " " + err.Source);
                }
                finally
                {
                    cn.Close();
                    cmd.Dispose();
                }
                return this;
            }
            throw new Exception("insufficient permissions to perform this action");
        }

        /// <summary>
        /// returns active/not deleted articles within a given published date range
        /// </summary>
        /// <returns>list of articles</returns>
        public List<Article> GetListArticleByDate(DateTime _Pub_Date, DateTime _Expire_Date)
        {
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                List<Article> returnArticleList = new List<Article>();

                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand("wse_ArticlesActiveListByDate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Parameters.AddWithValue("@Expire_Date", _Expire_Date);
                    cmd.Parameters.AddWithValue("@Pub_Date", _Pub_Date);

                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Article newArticle = new Article();
                        if (DBNull.Value != reader["Approved_By"])
                        {
                            newArticle.Approved_By = (Guid)reader["Approved_By"];
                            newArticle.Approved_Date = (DateTime)reader["Approved_Date"];
                        }
                        newArticle.Article_Text = reader["Article_Text"].ToString();
                        newArticle.Article_Description = reader["Article_Description"].ToString();
                        newArticle.Article_Page_Description = reader["Page_Description"].ToString();
                        newArticle.Article_Page_Keywords = reader["Page_Keywords"].ToString();
                        newArticle.Article_Title = reader["Article_Title"].ToString();
                        newArticle.Article_URL = reader["Article_URL"].ToString();
                        newArticle.Author = reader["Author"].ToString();
                        newArticle.Cat_Name = reader["Cat_Name"].ToString();
                        newArticle.Create_Date = (DateTime)reader["Create_Date"];
                        newArticle.Create_By = (Guid)reader["Create_by"];
                        newArticle.Deleted = (bool)reader["Deleted"];
                        newArticle.Edit_Date = (DateTime)reader["Edit_Date"];
                        newArticle.Expire_Date = (DateTime)reader["Expire_Date"];
                        newArticle.Edit_By = (Guid)reader["Edit_by"];
                        newArticle.id_Articles = (Int32)reader["id_Articles"];
                        newArticle.id_Category = (Int32)reader["id_Category"];
                        newArticle.id_Site = (Int32)reader["id_Site"];
                        newArticle.Pub_Date = (DateTime)reader["Pub_Date"];
                        newArticle.Publish = (bool)reader["Publish"];
                        newArticle.Secure = (bool)reader["Secure"];
                        newArticle.Site_Name = reader["Site_Name"].ToString();
                        newArticle.Sort = (Int32)reader["Sort"];

                        returnArticleList.Add(newArticle);
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Article.cs", err.Source, "Reading the article list by date an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the article list by date was an issue: " + err.Message + err.Source);
                }
                finally
                {
                    cn.Close();
                    cmd.Dispose();
                }
                return returnArticleList;
            }
            throw new Exception("insufficient permissions to perform this action");
        }

        /// <summary>
        /// returns a list of articles by category
        /// </summary>
        /// <param name="ActiveOnly">set to true if only active/not deleted categories are desired</param>
        /// <returns>list of articles</returns>
        public List<Article> GetListArticlesByCategory(bool ActiveOnly, int _id_Category)
        {
            //all articles for a given cat
            //overload with bool to use [sw_ArticlesActiveListByCategory] or not
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                List<Article> returnArticleList = new List<Article>();

                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                if (ActiveOnly)
                {
                    cmd.CommandText = "wse_ArticlesActiveListByCategory";
                }
                else
                {
                    cmd.CommandText = "wse_ArticlesListByCategory";
                }
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Parameters.AddWithValue("@id_Category", _id_Category);

                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Article newArticle = new Article();
                        if (DBNull.Value != reader["Approved_By"])
                        {
                            newArticle.Approved_By = (Guid)reader["Approved_By"];
                            newArticle.Approved_Date = (DateTime)reader["Approved_Date"];
                        }
                        newArticle.Article_Text = reader["Article_Text"].ToString();
                        newArticle.Article_Description = reader["Article_Description"].ToString();
                        newArticle.Article_Page_Description = reader["Page_Description"].ToString();
                        newArticle.Article_Page_Keywords = reader["Page_Keywords"].ToString();
                        newArticle.Article_Title = reader["Article_Title"].ToString();
                        newArticle.Article_URL = reader["Article_URL"].ToString();
                        newArticle.Author = reader["Author"].ToString();
                        newArticle.Cat_Name = reader["Cat_Name"].ToString();
                        newArticle.Create_Date = (DateTime)reader["Create_Date"];
                        newArticle.Create_By = (Guid)reader["Create_by"];
                        newArticle.Deleted = (bool)reader["Deleted"];
                        newArticle.Edit_Date = (DateTime)reader["Edit_Date"];
                        newArticle.Expire_Date = (DateTime)reader["Expire_Date"];
                        newArticle.Edit_By = (Guid)reader["Edit_by"];
                        newArticle.id_Articles = (Int32)reader["id_Articles"];
                        newArticle.id_Category = (Int32)reader["id_Category"];
                        newArticle.id_Site = (Int32)reader["id_Site"];
                        newArticle.Pub_Date = (DateTime)reader["Pub_Date"];
                        newArticle.Publish = (bool)reader["Publish"];
                        newArticle.Secure = (bool)reader["Secure"];
                        newArticle.Site_Name = reader["Site_Name"].ToString();
                        newArticle.Sort = (Int32)reader["Sort"];

                        returnArticleList.Add(newArticle);
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Article.cs", err.Source, "Reading the article list by category an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the article list by category was an issue: " + err.Message + err.Source);
                }
                finally
                {
                    cn.Close();
                    cn.Dispose();
                    cmd.Dispose();
                }
                return returnArticleList;
            }
            throw new Exception("insufficient permissions to perform this action");
        }

        /// <summary>
        /// gets a hashtable of URLs and Article IDs from the database for the given site
        /// no permissions check for speed considerations
        /// </summary>
        /// <param name="_siteID">the id of the site to search</param>
        /// <returns>hashtable of URL/Article ID</returns>
        public Hashtable GetURLlist(int _siteID)
        {
            //return a list of url and ID from the article table
            Hashtable returnTable = new Hashtable();

            //connect to DB and run proc to return list
            SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
            SqlCommand cmd = new SqlCommand("wse_ArticlesActiveURLBySite", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.Add(new SqlParameter("@id_Site", _siteID));
                cn.Open();
                SqlDataReader result = cmd.ExecuteReader();
                while (result.Read())
                {
                    returnTable.Add(result["pageURL"].ToString().ToLower(), Convert.ToInt32(result["pageID"]));
                }
            }
            catch (Exception err)
            {
                Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                Errors.LogError(UserID, "Article.cs", err.Source, "URL listing had an issue", err.Message, err.InnerException, err.StackTrace);
                //throw new Exception("URL listing had an issue: " + err.Message + err.Source);
            }
            finally
            {
                cn.Close();
                cmd.Dispose();
            }

            return returnTable;
        }
        #endregion
    }
}