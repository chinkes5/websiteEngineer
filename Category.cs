using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace webSiteEngineer
{
    /// <summary>
    /// defines a Category as a navigational structure, a means for organizing articles, image galleries, and something else.
    /// </summary>
    public class Category
    {
        private string _Cat_Name, _Cat_Description, /*_Cat_Text,*/ _Cat_ImagePath, _Cat_Page_Description, _Cat_Page_Keywords, _Cat_URL, _Site_Name; 
        private bool _Active, _Deleted, _Secure;
        private int _Sort, _id_Parent, _id_Site, _id_Categories;
        private DateTime _Edit_Date, _Create_Date, _Approved_Date;
        private Guid _Approved_By, _Edit_by, _Created_By;

        //used in cleaning up the properties as they are get and set
        Utility utility = new Utility();
        private string _Parent_Name;

        /******************************
         * properties
         * ***************************/
        #region PublicCategoryProperties
        // * are suggestions from http://SEOMOZ.org/ for better SEO compliance (need to check front end validation on these!)
        public int id_Categories
        {
            get { return _id_Categories; }
            set { _id_Categories = value; }
        }
        public string Cat_Name
        {
            get { return _Cat_Name; } //I trim the articles as those are expected to be in the page title and cat names are not, so no trim
            set { _Cat_Name = value; }
        }

        /// <summary>
        /// internal use only
        /// </summary>
        public string Cat_Description
        {
            get { return utility.safeHTML(_Cat_Description, true); }
            set { _Cat_Description = utility.safeHTML(value, false); }
        }

        /*
        /// <summary>
        /// text to display when showing the category
        /// </summary>
        public string Cat_Text
        {
            get { return utility.safeHTML(_Cat_Text, true); }
            set { _Cat_Text = utility.safeHTML(value, false); }
        }
        */

        /// <summary>
        /// path to image to display when showing the category (thumbs should be automatically handled)
        /// </summary>
        public string Cat_ImagePath
        {
            get { return _Cat_ImagePath; }
            set { _Cat_ImagePath = value; }
        }
        public string Cat_Page_Description
        {
            get { return utility.safeHTML(_Cat_Page_Description, true, 155); } //*
            set {_Cat_Page_Description = utility.safeHTML(value, false, 155); } //*
        }
        public string Cat_Page_Keywords
        {
            get { return utility.safeHTML(_Cat_Page_Keywords, true, 155); }
            set { _Cat_Page_Keywords = utility.safeHTML(value, false, 155); }
        }

        /// <summary>
        /// friendly URL when using categories for gallery or as placeholder for actual file
        /// </summary>
        public string Cat_URL
        {
            get
            {
                if (!string.IsNullOrEmpty(_Cat_URL))
                    return utility.safeURL(_Cat_URL, false);
                else
                    return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _Cat_URL = utility.safeURL(value, false);
                //for URL simplicity and safety
            }
        }
        public bool Secure
        {
            get { return _Secure; }
            set { _Secure = value; }
        }
        public bool Active
        {
            get { return _Active; }
            set { _Active = value; }
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

        /// <summary>
        /// used to set hierarchy of categories (beyond sort value)
        /// </summary>
        public int id_Parent
        {
            get { return _id_Parent; }
            set { _id_Parent = value; }
        }
        public int id_Site
        {
            get { return _id_Site; }
            set { _id_Site = value; }
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
        public DateTime Edit_Date
        {
            get { return _Edit_Date; }
            set { _Edit_Date = value; }
        }
        public Guid Edit_by
        {
            get { return _Edit_by; }
            set { _Edit_by = value; }
        }
        public DateTime Create_Date
        {
            get { return _Create_Date; }
            set { _Create_Date = value; }
        }
        public Guid Created_By
        {
            get { return _Created_By; }
            set { _Created_By = value; }
        }
        public string Site_Name
        {
            get { return _Site_Name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _Site_Name = value.ToLower();
                }
            }
        }
        /// <summary>
        /// Name of parent category
        /// </summary>
        public string ParentName
        {
            get { return _Parent_Name; }
            set { _Parent_Name = value; }
        }
        #endregion

        /******************************
     * methods
     * ***************************/
        public Category()
        {
            //default values for this class

            //_id_Categories *no default!
            //_Cat_Name *required field!
            //_Cat_Description;
            //_Cat_Text = "";
            //_Cat_ImagePath = "";
            //_Cat_Page_Description = "";
            //_Cat_Page_Keywords = "";
            //_Cat_URL = "";
            //_Secure = false *bool default is false
            //_Active = false;
            //_Deleted = false;
            _Sort = 99; //*int default is 0
            //_Approved_By *really want null, but not available for this type
            //_Edit_by;
            //_Edit_Date *want the actual time and date
            //_Created_By
            //_Create_Date;
            //_id_Parent *only home page is null, the rest have _some_ value
            //_id_Site *I don't know the site I am working on, necessarily...
        }
        #region PublicCategoryMethods

        /// <summary>
        /// returns category info from current category ID
        /// </summary>
        /// <returns>requested category</returns>
        public Category GetCategoryByID()
        {
            //get the cat by it's id
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
                SqlCommand cmd = new SqlCommand("wse_CategoryListByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Parameters.AddWithValue("@id_Category", this.id_Categories);

                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        this.Active = (bool)reader["Active"];
                        if (DBNull.Value != reader["Approved_By"])
                        {
                            this.Approved_By = (Guid)reader["Approved_By"];
                            this.Approved_Date = (DateTime)reader["Approved_Date"];
                        }
                        this.Cat_Description = reader["Cat_Description"].ToString();
                        this.Cat_ImagePath = reader["Cat_ImagePath"].ToString();
                        this.Cat_Name = reader["Cat_Name"].ToString();
                        this.Cat_Page_Description = reader["Cat_Page_Description"].ToString();
                        this.Cat_Page_Keywords = reader["Cat_Page_Keywords"].ToString();
                        //this.Cat_Text = reader["Cat_Text"].ToString();
                        this.Cat_URL = reader["Cat_URL"].ToString();
                        this.Create_Date = (DateTime)reader["Create_Date"];
                        this.Created_By = (Guid)reader["Create_by"];
                        this.Deleted = (bool)reader["Deleted"];
                        this.Edit_Date = (DateTime)reader["Edit_Date"];
                        this.Edit_by = (Guid)reader["Edit_by"];
                        //this.id_Categories = (Int32)reader["id_Categories"];
                        if (DBNull.Value != reader["id_Parent"])
                        {
                            this.id_Parent = (Int32)reader["id_Parent"];
                            this.ParentName = reader["ParentName"].ToString();
                        }
                        this.id_Site = (Int32)reader["id_Site"];
                        this.Secure = (bool)reader["Secure"];
                        //this.Site_Name = reader["Site_Name"].ToString();
                        this.Sort = (Int32)reader["Sort"];
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Category.cs", err.Source, "Reading category was an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the category was an issue: " + err.Message + err.Source);
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
        /// returns list of all children categories for a given parent ID
        /// </summary>
        /// <param name="ActiveOnly">set to true if only active/not deleted categories are desired</param>
        /// <param name="_id_Parent">parent ID to work with</param>
        /// <returns>list of categories</returns>
        public List<Category> GetListCategoriesByParentID(bool ActiveOnly, int _id_Parent)
        {
            //gets all children
            //overload with bool to use [sw_CategoriesActiveListByParentID] or not
            if (utility.isRoleActionPermissible("read", this.id_Site))
            {
                List<Category> returnCategoryList = new List<Category>();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection.ConnectionString = Utility.dbConnectionString;
                if (ActiveOnly)
                {
                    cmd.CommandText = "wse_CategoriesActiveListByParentID";
                }
                else
                {
                    cmd.CommandText = "wse_CategoriesListByParentID";
                }
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cmd.Parameters.AddWithValue("@id_Parent", _id_Parent);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Category newCategory = new Category();
                        newCategory.Active = (bool)reader["Active"];
                        if (DBNull.Value != reader["Approved_By"])
                        {
                            newCategory.Approved_By = (Guid)reader["Approved_By"];
                            newCategory.Approved_Date = (DateTime)reader["Approved_Date"];
                        }
                        newCategory.Cat_Description = reader["Cat_Description"].ToString();
                        newCategory.Cat_ImagePath = reader["Cat_ImagePath"].ToString();
                        newCategory.Cat_Name = reader["Cat_Name"].ToString();
                        newCategory.Cat_Page_Description = reader["Cat_Page_Description"].ToString();
                        newCategory.Cat_Page_Keywords = reader["Cat_Page_Keywords"].ToString();
                        //newCategory.Cat_Text = reader["Cat_Text"].ToString();
                        newCategory.Cat_URL = reader["Cat_URL"].ToString();
                        newCategory.Create_Date = (DateTime)reader["Create_Date"];
                        newCategory.Created_By = (Guid)reader["Create_by"];
                        newCategory.Deleted = (bool)reader["Deleted"];
                        newCategory.Edit_Date = (DateTime)reader["Edit_Date"];
                        newCategory.Edit_by = (Guid)reader["Edit_by"];
                        newCategory.id_Categories = (Int32)reader["id_Categories"];
                        if (DBNull.Value != reader["id_Parent"])
                        {
                            newCategory.id_Parent = (Int32)reader["id_Parent"];
                            newCategory.ParentName = reader["ParentName"].ToString();
                        }
                        newCategory.id_Site = (Int32)reader["id_Site"];
                        newCategory.Secure = (bool)reader["Secure"];
                        newCategory.Site_Name = reader["Site_Name"].ToString();
                        newCategory.Sort = (Int32)reader["Sort"];

                        returnCategoryList.Add(newCategory);
                    }
                }
                catch (Exception err)
                {
                    Guid UserID = new Guid(HttpContext.Current.Session["UserID"].ToString());
                    Errors.LogError(UserID, "Category.cs", err.Source, "Reading category list by parent ID was an issue", err.Message, err.InnerException, err.StackTrace);
                    //throw new Exception("Reading the category list by parent ID was an issue: " + err.Message + err.Source);
                }
                finally
                {
                    cmd.Dispose();
                }
                return returnCategoryList;
            }
            throw new Exception("insufficient permissions to perform this action");
        }


        /// <summary>
        /// gets a hashtable of URLs and Category IDs from the database for the given site
        /// no permissions check for speed purposes
        /// </summary>
        /// <param name="_siteID">the id of the site to search</param>
        /// <returns>hashtable of URL/Category ID</returns>
        public Hashtable GetCatURLlist(int _siteID)
        {
            //return a list of url and ID from the article table
            Hashtable returnTable = new Hashtable();

            //connect to DB and run proc to return list
            SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
            SqlCommand cmd = new SqlCommand("wse_CategoriesActiveURLBySite", cn);
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
                Errors.LogError(UserID, "Category.cs", err.Source, "Category URL listing had an issue", err.Message, err.InnerException, err.StackTrace);
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