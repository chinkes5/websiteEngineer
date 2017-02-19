using System.Data.SqlClient;
using System.Data;
using System;
using webSiteEngineer;

public class Errors
{
    public static void LogError(Guid _userID, string _pageID, string _source, string _wseMessage, string _errorMessage, Exception _innerException, string _stackTrace)
    {
        Utility utility = new Utility();
        SqlConnection cn = new SqlConnection(Utility.dbConnectionString);
        SqlCommand cmd = new SqlCommand("wse_ErrorAdd", cn);
        cmd.CommandType = CommandType.StoredProcedure;

        try
        {

            cmd.Parameters.AddWithValue("@userID", _userID);
            cmd.Parameters.AddWithValue("@pageID", utility.trimLength(_pageID, 256));
            cmd.Parameters.AddWithValue("@source", utility.trimLength(_source, 256));
            cmd.Parameters.AddWithValue("@wseMessage", utility.trimLength(_wseMessage, 256));
            cmd.Parameters.AddWithValue("@errorMessage", utility.trimLength(_errorMessage, 256));
            if (_innerException != null)
            {
                cmd.Parameters.AddWithValue("@innerException", utility.trimLength(_innerException.Message, 256));
            }
            cmd.Parameters.AddWithValue("@stackTrace", utility.trimLength(_stackTrace, 256));

            cn.Open();
            if (cmd.ExecuteNonQuery() > 1)
            {
                //not good, only one record should be saved at a time!
                throw new Exception("Error logging may not have saved error; ");
            }
        }
        catch (Exception err)
        {
            throw new Exception("Error logging had an issue: " + err.Message + " " + err.Source);
        }
        finally
        {
            cn.Close();
            cmd.Dispose();
        }
    }
}
