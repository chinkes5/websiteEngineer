using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace webSiteEngineer
{
    /// <summary>
    /// defines a site in the Site Works context
    /// </summary>
    public class Site
    {
        /******************************
         * properties
         * ***************************/
        private string _Site_Name, _URL, _Secure_URL, _SiteOwner, _SiteOwnerEmail;
        private int _id_Site;
        private DateTime _StartDate, _Create_Date, _Edit_Date;
        private Guid _Create_By, _Edit_By; //_Approved_By, 

        //used in cleaning up the properties as they are get and set
        Utility utility = new Utility();

        #region PublicSiteProperties
        public int id_Site
        {
            get { return _id_Site; }
            set { _id_Site = value; }
        }
        public string Site_Name
        {
            get { return utility.safeHTML(_Site_Name, true); }
            set { _Site_Name = utility.safeHTML(value.ToLower(), false); }
        }
        public string URL
        {
            get { return utility.safeURL(_URL, false); }
            set
            {
                if (utility.checkRexEx(value, false))
                {
                    //how would I use this?  So what if entered 'wrong'?
                    _URL = utility.safeURL(value, true);
                }
                else
                {
                    _URL = "none";
                }
            }
        }
        public string Secure_URL
        {
            get { return utility.safeURL(_Secure_URL, false); }
            set
            {
                if (utility.checkRexEx(value, false))
                {
                    _Secure_URL = utility.safeURL(value, true);
                }
                else
                {
                    _Secure_URL = "none";
                }
            }
        }
        public string SiteOwner
        {
            get { return utility.safeHTML(_SiteOwner, true); }
            set { _SiteOwner = utility.safeHTML(value.ToLower(), false); }
        }
        public string SiteOwnerEmail
        {
            get { return utility.safeHTML(_SiteOwnerEmail, true); }
            set
            {
                string returnValue;
                if (utility.checkRexEx(value, true))
                {
                    returnValue = utility.safeHTML(value, false);
                }
                else
                {
                    //gosh, what do I do if not a valid email address?
                    returnValue = "none";
                }
                _SiteOwnerEmail = returnValue;
            }
        }
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        /*public Guid Approved_By
        {
            get { return _Approved_By; }
            set { _Approved_By = value; }
        }*/
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
        #endregion

        /******************************
     * methods
     * ***************************/
        public Site()
        {
            //TODO: Add constructor logic here
            // 
            //seems grumpy without some date in date fields...
            StartDate = DateTime.Now;
            Create_Date = DateTime.Now;
            Edit_Date = DateTime.Now;
        }

        #region PublicSiteMethods
        /// <summary>
        /// saves the new current site to the database
        /// no check of permissions for this action as it requires a site ID which may not work
        /// </summary>
        /// <returns>pass/fail</returns>
        public bool AddSite()
        {
            SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
            SqlCommand cmd = new SqlCommand("wse_SiteAdd", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cmd.Parameters.AddWithValue("@Site_Name", this.Site_Name);
                cmd.Parameters.AddWithValue("@URL", this.URL);
                cmd.Parameters.AddWithValue("@Secure_URL", this.Secure_URL);
                cmd.Parameters.AddWithValue("@SiteOwner", this.SiteOwner);
                cmd.Parameters.AddWithValue("@SiteOwnerEmail", this.SiteOwnerEmail);
                cmd.Parameters.AddWithValue("@StartDate", this.StartDate);
                //cmd.Parameters.AddWithValue("@Approved_By", this.Approved_By);
                cmd.Parameters.AddWithValue("@Create_By", this.Create_By);
                cmd.Parameters.AddWithValue("@Create_Date", DateTime.Now);
                cmd.Parameters.AddWithValue("@Edit_By", this.Create_By);
                cmd.Parameters.AddWithValue("@Edit_Date", DateTime.Now);

                cn.Open();
                //new site id
                this._id_Site = Convert.ToInt32(cmd.ExecuteScalar());

                //with each new site we also add a primary category and home page article
                CategoryManagement newCat = new CategoryManagement();
                newCat.Active = true;
                newCat.Cat_Name = "Home Category";
                newCat.Created_By = this.Create_By;
                newCat.Cat_Description = "Home category for " + this.Site_Name;
                newCat.Edit_by = this.Create_By;
                //newCat.id_Parent left blank as this is home category
                newCat.id_Site = this.id_Site;
                newCat.AddCategory();

                //get the username
                Page newPage = new Page();
                //now the article
                ArticleManagement newArt = new ArticleManagement();
                newArt.Article_Description = "Home page for " + this.Site_Name;
                newArt.Article_Text = "Home page for " + this.Site_Name + "&lt;br /&gt; Under contruction... &lt;br /&gt; Please add pages to this site.";
                newArt.Article_Title = "Home Page";
                newArt.Author = newPage.User.Identity.Name;
                newArt.Create_By = this.Create_By;
                newArt.Edit_By = this.Create_By;
                newArt.Expire_Date = DateTime.Now.AddMonths(3);
                newArt.id_Category = newCat.id_Categories;
                newArt.id_Site = this.id_Site;
                newArt.Pub_Date = DateTime.Now;
                newArt.AddArticle();
                return true;
            }
            catch (Exception err)
            {
                Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                Errors.LogError(UserID, "Site.cs", err.Source, "Site Add had an issue", err.Message, err.InnerException, err.StackTrace);
                //throw new Exception("SiteAdd had an issue: " + err.Message + err.Source);
                return false;
            }
            finally
            {
                cn.Close();
                cmd.Dispose();
            }
        }
        /// <summary>
        /// saves the existing current site to the database
        /// </summary>
        /// <returns>pass/fail</returns>
        public bool UpdateSite()
        {
            if (utility.isRoleActionPermissible("update", this.id_Site))
            {
                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand("wse_SiteUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //add the current time
                this.Edit_Date = DateTime.Now;

                try
                {
                    cmd.Parameters.AddWithValue("@Site_Name", this._Site_Name);
                    cmd.Parameters.AddWithValue("@URL", this.URL);
                    cmd.Parameters.AddWithValue("@Secure_URL", this.Secure_URL);
                    cmd.Parameters.AddWithValue("@SiteOwner", this.SiteOwner);
                    cmd.Parameters.AddWithValue("@SiteOwnerEmail", this.SiteOwnerEmail);
                    cmd.Parameters.AddWithValue("@StartDate", this.StartDate);
                    //cmd.Parameters.AddWithValue("@Approved_By", this.Approved_By);
                    cmd.Parameters.AddWithValue("@Edit_By", this.Edit_By);
                    cmd.Parameters.AddWithValue("@Edit_Date", this.Edit_Date);
                    cmd.Parameters.AddWithValue("@Original_id_Site", this.id_Site);

                    cn.Open();
                    int i = cmd.ExecuteNonQuery();
                    if (i == 1)
                    {
                        return true;
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Site.cs", err.Source, "Site Update had an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("SiteUpdate had an issue: " + err.Message + err.Source);
                }
                finally
                {
                    cn.Close();
                    cmd.Dispose();
                }
                return false;
            }
            throw new Exception("insufficent permissions to perform this action");
        }
        /// <summary>
        /// returns a list of sites in the database
        /// </summary>
        /// <returns>list of sites</returns>
        public List<Site> GetSitesList()
        {
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                List<Site> returnSiteList = new List<Site>();

                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand("wse_SitesList", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Site newSite = new Site();
                        newSite.id_Site = (Int32)reader["id_Site"];
                        newSite.Site_Name = reader["Site_Name"].ToString();
                        newSite.URL = reader["URL"].ToString();
                        newSite.Secure_URL = reader["Secure_URL"].ToString();
                        newSite.SiteOwner = reader["SiteOwner"].ToString();
                        newSite.SiteOwnerEmail = reader["SiteOwnerEmail"].ToString();
                        newSite.StartDate = (DateTime)reader["StartDate"];
                        //newSite.Approved_By = (Guid)reader["Approved_By"];
                        newSite.Create_Date = (DateTime)reader["Create_Date"];
                        newSite.Create_By = (Guid)reader["Create_By"];
                        newSite.Edit_Date = (DateTime)reader["Edit_Date"];
                        newSite.Edit_By = (Guid)reader["Edit_By"];

                        returnSiteList.Add(newSite);
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Site.cs", err.Source, "Reading the site list was an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the site list was an issue: " + err.Message + err.Source);
                }
                finally
                {
                    cn.Close();
                    cmd.Dispose();
                }
                return returnSiteList;
            }
            throw new Exception("insufficent permissions to perform this action");
        }
        /// <summary>
        /// retreives a site from the database with the current site ID
        /// </summary>
        /// <returns>the complete site</returns>
        public Site GetSiteByID()
        {
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand("wse_SiteListByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Parameters.AddWithValue("@id_Site", this.id_Site);
                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        //this.id_Site = (Int32)reader["id_Site"];
                        this.Site_Name = reader["Site_Name"].ToString();
                        this.URL = reader["URL"].ToString();
                        this.Secure_URL = reader["Secure_URL"].ToString();
                        this.SiteOwner = reader["SiteOwner"].ToString();
                        this.SiteOwnerEmail = reader["SiteOwnerEmail"].ToString();
                        this.StartDate = (DateTime)reader["StartDate"];
                        //this.Approved_By = (Guid)reader["Approved_By"];
                        this.Create_Date = (DateTime)reader["Create_Date"];
                        this.Create_By = (Guid)reader["Create_By"];
                        this.Edit_Date = (DateTime)reader["Edit_Date"];
                        this.Edit_By = (Guid)reader["Edit_By"];
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Site.cs", err.Source, "Reading the site was an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the site was an issue: " + err.Message + err.Source);
                }
                finally
                {
                    cn.Close();
                    cmd.Dispose();
                }
                return this;
            }
            throw new Exception("insufficent permissions to perform this action");
        }
        #endregion
    }
}