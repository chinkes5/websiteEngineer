using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Configuration;

namespace webSiteEngineer
{
    /// <summary>
    /// Summary description for utility
    /// </summary>
    public class Utility
    {
        public Utility()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //public static string dbConnectionString = ConfigurationSettings.AppSettings["WebSiteEngineerConnectionString"].ToString() //for testing
        public static string dbConnectionString = "Data Source=192.168.1.100;Initial Catalog=websiteEngineer;Persist Security Info=True;User ID=chinkeswebuser;Password=eV3rN@me";

        /// <summary>
        /// utility for sending email, accepts TO:, FROM:, SUBJECT:, and a message
        /// </summary>
        /// <param name="txtFrom">TO:</param>
        /// <param name="txtTo">FROM:</param>
        /// <param name="txtSubject">SUBJECT:</param>
        /// <param name="txtMessage">the message, in HTML or plain text</param>
        public bool SendEmail(string txtFrom, string txtTo, string txtSubject, string txtMessage)
        {
            try
            {
                SmtpClient mailSvr = new SmtpClient();

                MailMessage msg = new MailMessage(txtFrom, txtTo, txtSubject, txtMessage);
                msg.IsBodyHtml = true;
                mailSvr.Send(msg);

                //everything ok!
                return true;
            }
            catch (SmtpException Smtp_ex)
            {
                //error in emailing
                throw new SystemException("There was an SMTP issue sending your message", Smtp_ex);
            }
            catch (Exception ex)
            {
                //error in transmission
                throw new SystemException("There was an unknown issue sending your message", ex);
            }
        }

        /// <summary>
        /// a regular expression checker, returns true if matches given expression
        /// </summary>
        /// <param name="_checkString">the value to compare against</param>
        /// <param name="_expression">the regular expression to use</param>
        public bool checkRexEx(string _checkString, string _expression)
        {
            //reference- http://msdn.microsoft.com/en-us/library/ms972966.aspx
            //email- @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
            //URL- @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"
            //apply a pass/fail test on a given string against the given regular expression
            bool returnValue = false;
            if (Regex.IsMatch(_checkString.Trim(), _expression))
            {
                returnValue = true;
            }
            return returnValue;
        }

        /// <param name="_checkString">the value to compare against</param>
        /// <param name="_isEmail">true for email regular expression or false for URL regular expression</param>
        public bool checkRexEx(string _checkString, bool _isEmail)
        {
            //reference- http://msdn.microsoft.com/en-us/library/ms972966.aspx
            //apply a pass/fail test on a given string against the given regular expression
            string _expression;
            if (_isEmail)
            {
                _expression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            }
            else
            {
                _expression = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
            }

            bool returnValue = false;
            if (Regex.IsMatch(_checkString.Trim(), _expression))
            {
                returnValue = true;
            }
            return returnValue;
        }

        /// <summary>
        /// cuts off the end of any string if longer than the given length
        /// </summary>
        /// <param name="_checkString">a string to check</param>
        /// <param name="_length">how long it can be</param>
        public string trimLength(string _checkString, int _length)
        {
            if (!string.IsNullOrEmpty(_checkString))
            {
                //if the string is longer than the given value, chop it off, otherwise return it whole
                int _checkStringLength = _checkString.Length;
                string _returnString;
                if (_checkStringLength > _length)
                {
                    //too long
                    _returnString = _checkString.Substring(0, _checkStringLength);
                }
                else
                {
                    //OK length
                    _returnString = _checkString;
                }
                return _returnString;
            }
            return null;
        }

        /// <summary>
        /// decodes or encodes a string to protect against JavaScript or SQL injection attacks
        /// </summary>
        /// <param name="_HTMLstring">the string to change</param>
        /// <param name="_decode">true if you wish to decode or false if you wish to encode</param>
        /// <returns>string</returns>
        public string safeHTML(string _HTMLstring, bool _decode)
        {
            //one place to keep track of for which is decoding and encoding
            string _returnValue;

            if (_decode)
            {
                //inbound to the DB, right?
                _returnValue = HttpUtility.HtmlEncode(miniHTML(_HTMLstring));
            }
            else
            {
                //outbound from the DB
                _returnValue = HttpUtility.HtmlDecode(_HTMLstring);
            }
            return _returnValue;
        }
        /// <param name="_HTMLstring">the string to change</param>
        /// <param name="_decode">true if you wish to decode or false if you wish to encode</param>
        /// <param name="_length">the maximum length of the string to be changed, will not check if valid HTML remains!</param>
        public string safeHTML(string _HTMLstring, bool _decode, int _length)
        {
            //one place to keep track of for which is decoding and encoding
            string _returnValue, _tempValue;

            if (string.IsNullOrEmpty(_HTMLstring))
            {
                _returnValue = null;
            }
            else
            {
                if (_decode)
                {
                    //inbound to the DB, right?
                    _tempValue = HttpUtility.HtmlEncode(miniHTML(_HTMLstring));
                }
                else
                {
                    //outbound from the DB
                    _tempValue = HttpUtility.HtmlDecode(_HTMLstring);
                }

                //trim to size too
                int _returnValueLength = _tempValue.Length;
                if (_returnValueLength > _length)
                {
                    _returnValue = _tempValue.Substring(0, _length);
                }
                else
                {
                    _returnValue = _tempValue;
                }
            }
            return _returnValue;
        }

        /// <summary>
        /// an attempt to strip out extra stuff so the HTML is as small as possible
        /// </summary>
        /// <param name="_HTMLstring">html to be 'compressed'</param>
        /// <returns>html with less fluff</returns>
        public string miniHTML(string _HTMLstring)
        {
            //might want to look at implementing this-
            //http://htmlagilitypack.codeplex.com/ 
            //later this year, when he has updated the source code

            if (string.IsNullOrEmpty(_HTMLstring))
            {
                return null;
            }
            else
            {
                //remove extra leading or trailing spaces
                string _newString = _HTMLstring.Trim().Replace("  ", " ").Replace('\r', ' ').Replace('\n', ' ');
                string returnString = _newString;

                return returnString;
            }
        }

        /// <summary>
        /// function to return a URL with escaped characters
        /// </summary>
        /// <param name="_URLstring">URL to encode</param>
        /// <param name="_decode">true to encode, false to decode</param>
        /// <returns></returns>
        public string safeURL(string _URLstring, bool _decode)
        {
            //one place to keep track of for which is decoding and encoding
            string _returnValue;

            if (_decode)
            {
                //inbound to the DB, right?
                _returnValue = HttpUtility.UrlEncode(_URLstring.Trim().ToLower());
            }
            else
            {
                //outbound from the DB
                _returnValue = HttpUtility.UrlDecode(_URLstring);
            }
            return _returnValue;
        }

        /// <summary>
        /// evaluate if given action is ok (true) for current user role
        /// this is where all my business logic around permissions is at
        /// known roles are author, editor, siteadmin, uberadmin
        /// </summary>
        /// <param name="_action">acceptable actions: read, update, insert, publish, approve, delete</param>
        /// <param name="_siteID">int with the current site ID</param>
        /// <returns>true/false</returns>
        public bool isRoleActionPermissible(string _action, int _siteID)
        {
            /* http://weblogs.asp.net/scottgu/archive/2006/10/04/Tip_2F00_Trick_3A00_-Adding-Authorization-Rules-to-Business-and-Data-Layers-using-PrincipalPermissionAttributes.aspx
            read this to see if this implementation is better */
            Page newPage = new Page();

            if (newPage.User.IsInRole("uberadmin"))
            {
                //uber admin is for every site
                return true;
            }

            //check if the list of sites the user has assigned to them match the current site
            //need to make table with many to many relation of UserID and SiteID!
            /*int[] siteList = { 1, 2, 3 };
            foreach (int siteID in siteList)
            {
                if (_siteID == siteID)
                {
                    //may be part of a loop to get thru all the assigned sites
                    break;
                }
                return false;
            }*/

            //check the action against the permissions for each role
            switch (_action.ToLower())
            {
                case "read":
                    {
                        //anyone can read if you have permission to the site
                        return true;
                    }
                case "update":
                    {
                        if (newPage.User.IsInRole("siteadmin") || newPage.User.IsInRole("editor"))
                        {
                            return true;
                        }
                        if (newPage.User.IsInRole("author"))
                        {
                            //if working on own article; editby = userID or something...
                            return true;
                        }
                        return false;
                    }
                case "insert":
                    {
                        if (newPage.User.IsInRole("siteadmin") || newPage.User.IsInRole("editor") || newPage.User.IsInRole("author"))
                        {
                            return true;
                        }
                        return false;
                    }
                case "insertcat":
                    {
                        if (newPage.User.IsInRole("siteadmin") || newPage.User.IsInRole("editor"))
                        {
                            return true;
                        }
                        return false;
                    }
                case "publish":
                    {
                        if (newPage.User.IsInRole("siteadmin") || newPage.User.IsInRole("editor"))
                        {
                            return true;
                        }
                        return false;
                    }
                case "approve":
                    {
                        if (newPage.User.IsInRole("siteadmin"))
                        {
                            return true;
                        }
                        return false;
                    }
                case "delete":
                    {
                        if (newPage.User.IsInRole("siteadmin"))
                        {
                            return true;
                        }
                        return false;
                    }
                default:
                    return false;
            }
        }

        public string SaveErrorLog()
        {
            //save to db!
            return "OK, saved!";
        }

        /*public string GetUserName(Guid _userID)
        {
            MembershipUser newUser = Membership.GetUser(_userID);
            return newUser.UserName;
        }*/
    }
}