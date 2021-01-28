using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        
        public class MyObject
        {
            public string success { get; set; }
            public List<string> ErrorMessage { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserEmail"] == null)
            {
                if (Session["create_success"] != null)
                {
                    Page.Items["create_success"] = "Account Created";
                    Session["create_success"] = null;
                }

                if (Session["create_error"] != null)
                {
                    Page.Items["create_error"] = Session["create_error"].ToString() ;
                    Session["create_error"] = null;

                }
            }
            else
            {
                Response.Redirect("~/Profile", false);
            }
        }

        protected void loginBtn_Click(object sender, EventArgs e)
        {
            if (ValidateCaptcha())
            {
                string pwd = tb_password.Text.ToString().Trim();
                string email = tb_email.Text.ToString().Trim();

                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);

                List<String> errList = new List<string>();

                try
                {
                    System.Diagnostics.Debug.WriteLine("Test 1");
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Test 2");
                        string pwdWithSalt = pwd + dbSalt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                        string userHash = Convert.ToBase64String(hashWithSalt);
                        
                        if (AccountExistsUsingEmail(email))
                        {
                            System.Diagnostics.Debug.WriteLine("Test 3");
                            if (getAttempt(email) == "3")
                            {
                                System.Diagnostics.Debug.WriteLine("Test 4");
                                if (Convert.ToDateTime(getAttemptTime(email)) > DateTime.Now)
                                {
                                    System.Diagnostics.Debug.WriteLine("Test 5");
                                    // Account is locked
                                    System.Diagnostics.Debug.WriteLine("Error - Account is Locked");
                                    Page.Items["create_error"] = "Account is Locked. Please try again later";
                                    errList.Add("Account is locked");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Test 6");
                                    UpdateAccountFailedAttempts(email, 1);
                                    UpdateDateTime(email, DateTime.Now);
                                }
                            }

                            else if (userHash.Equals(dbHash))
                            {
                                System.Diagnostics.Debug.WriteLine("Test 7");
                                Session["UserEmail"] = email;
                                UpdateAccountFailedAttempts(email, 0);

                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                if (Convert.ToDateTime(getPasswordMaxAge(email)) < DateTime.Now)
                                {
                                    System.Diagnostics.Debug.WriteLine("Test 8");
                                    Response.Redirect("~/Settings", false);
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("Test 9");
                                    Response.Redirect("~/Profile", false);
                                }
                            }

                            else if (getAttempt(email) == "2")
                            {
                                System.Diagnostics.Debug.WriteLine("Test 10");
                                int attempt = Convert.ToInt32(getAttempt(email)) + 1;
                                UpdateAccountFailedAttempts(email, attempt);
                                UpdateDateTime(email, DateTime.Now.AddMinutes(1));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Test 11");
                                int attempt = Convert.ToInt32(getAttempt(email)) + 1;
                                UpdateAccountFailedAttempts(email, attempt);
                                Response.Redirect("~/Login", false);
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Test 12");
                        System.Diagnostics.Debug.WriteLine("Error - Login is invalid");
                        Page.Items["create_error"] = "Login is Invalid. Please try again";
                        errList.Add("Login is Invalid. Please try again");
                    }
                    System.Diagnostics.Debug.WriteLine("Error List: " + errList.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                finally {
                    System.Diagnostics.Debug.WriteLine("Error List: " + errList.ToString());
                    if (errList.Count > 0)
                    {
                        Session["create_error"] = String.Join(",", errList);
                        Page.Items["create_error"] = "Login is Invalid. Please try again";
                        System.Diagnostics.Debug.WriteLine("Login is Invalid. Please try again");
                        Response.Redirect("~/Login", false);
                    }
                    
                }
            }

        }

        

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=6LeZpuQZAAAAAJAEDF_iw7RPNnIe75QarGA23O0S &response=" + captchaResponse);
            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected string getDBHash(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_passwordHash FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_passwordHash"] != null)
                        {
                            if (reader["account_passwordHash"] != DBNull.Value)
                            {
                                h = reader["account_passwordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected string getDBSalt(string email)
        {
            string s = null;

            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_passwordSalt FROM ACCOUNT WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_passwordSalt"] != null)
                        {
                            if (reader["account_passwordSalt"] != DBNull.Value)
                            {
                                s = reader["account_passwordSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return s;
        }

        protected int UpdateAccountFailedAttempts(string email, int attempt)
        {
            try
            {
                SqlConnection myConn = new SqlConnection(MYDBConnectionString);

                string sqlStmt = "UPDATE Account SET account_invalidAttempts = @InvalidAttempts where account_email = @Email";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@Email", email);
                sqlCmd.Parameters.AddWithValue("@InvalidAttempts", attempt);

                myConn.Open();
                int result = sqlCmd.ExecuteNonQuery();

                myConn.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected int UpdateDateTime(string email, DateTime datetime)
        {
            try
            {
                SqlConnection myConn = new SqlConnection(MYDBConnectionString);

                string sqlStmt = "UPDATE Account SET account_lastInvalidAttemptTime = @account_lastInvalidAttemptTime where account_email = @Email";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@Email", email);
                sqlCmd.Parameters.AddWithValue("@account_lastInvalidAttemptTime", datetime);

                myConn.Open();
                int result = sqlCmd.ExecuteNonQuery();

                myConn.Close();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        protected bool AccountExistsUsingEmail(string email)
        {
            try
            {

                SqlConnection myConn = new SqlConnection(MYDBConnectionString);

                // Create a DataAdapter to retrieve data from the database table
                string sqlStmt = "Select * from Account where account_email = @paraEmail";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                // Create a DataSet to store the data to be retrieved
                DataSet ds = new DataSet();

                // Use the DataAdapter to fill the DataSet with data retrieved
                da.Fill(ds);


                int record = ds.Tables[0].Rows.Count;

                if (record == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected string getAttemptTime(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_lastInvalidAttemptTime FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_lastInvalidAttemptTime"] != null)
                        {
                            if (reader["account_lastInvalidAttemptTime"] != DBNull.Value)
                            {
                                h = reader["account_lastInvalidAttemptTime"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected string getPasswordMaxAge(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_passwordMaxAge FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_passwordMaxAge"] != null)
                        {
                            if (reader["account_passwordMaxAge"] != DBNull.Value)
                            {
                                h = reader["account_passwordMaxAge"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected string getAttempt(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_invalidAttempts FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_invalidAttempts"] != null)
                        {
                            if (reader["account_invalidAttempts"] != DBNull.Value)
                            {
                                h = reader["account_invalidAttempts"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }
    }
}