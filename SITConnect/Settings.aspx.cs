using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Settings : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string email = null;
        static string finalHash;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserEmail"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("~/Login", false);
                }
                else
                {

                    email = (string)Session["UserEmail"];
                    if (Convert.ToDateTime(getPasswordMinAge(email)) < DateTime.Now)
                    {
                        if (Convert.ToDateTime(getPasswordMaxAge(email)) < DateTime.Now)
                        {
                            Page.Items["showModal"] = 1;
                            Page.Items["openModal"] = 1;

                            if (Session["create_success"] != null)
                            {
                                Page.Items["create_success"] = Session["create_success"].ToString();
                                Session["create_success"] = null;
                            }

                            if (Session["create_error"] != null)
                            {
                                Page.Items["create_error"] = Session["create_error"].ToString();
                                Session["create_error"] = null;
                            }
                        }
                        else
                        {
                            Page.Items["showModal"] = 1;

                            if (Session["create_success"] != null)
                            {
                                Page.Items["create_success"] = Session["create_success"].ToString();
                                Session["create_success"] = null;
                            }

                            if (Session["create_error"] != null)
                            {
                                Page.Items["create_error"] = Session["create_error"].ToString();
                                Session["create_error"] = null;
                            }
                        }
                    }
                    else
                    {
                        Session["create_error"] = "Password cannot be changed yet";
                        Response.Redirect("~/Profile", false);
                    }
                }
            }
            else
            {
                Response.Redirect("~/Login", false);
            }
        }

        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("~/Login", false);
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = String.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = String.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        protected void passwordSaveBtn_Click(object sender, EventArgs e)
        {
            string pwd = confirmPwdTB.Text.ToString().Trim();
            string oldPwd = oldPwdTB.Text.ToString().Trim();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);

            if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
            {
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + dbSalt;

                string oldpwdwithSalt = oldPwd + dbSalt;

                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                byte[] oldplainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(oldPwd));
                byte[] oldhashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(oldpwdwithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);
                var oldFinalHash = Convert.ToBase64String(oldhashWithSalt);

                if (AccountExistsUsingEmail(email))
                {
                    if (oldFinalHash == dbHash)
                    {
                        if (finalHash != dbHash)
                        {
                            if (finalHash != getDBHashOld1(email) && finalHash != getDBHashOld2(email))
                            {
                                if (String.IsNullOrEmpty(getDBHashOld1(email)))
                                {
                                    string oldHash = getDBHash(email);
                                    if (String.IsNullOrEmpty(getDBHashOld2(email)))
                                    {
                                        updatePasswordOld2(email, oldHash);
                                    }
                                    else
                                    {
                                        updatePasswordOld1(email, oldHash);
                                    }
                                }
                                else
                                {
                                    string old1Hash = getDBHashOld1(email);
                                    updatePasswordOld2(email, old1Hash);
                                    updatePasswordOld1(email, getDBHash(email));

                                }

                                updatePassword(email, finalHash);
                                logoutBtn_Click(sender, e);
                            }
                            else
                            {
                                Session["create_error"] = "Cannot reuse passwords. Create a new one.";
                                Response.Redirect("~/Settings", false);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Test");
                            Session["create_error"] = "Cannot reuse password. Create a new one.";
                            Response.Redirect("~/Settings", false);
                        }
                    }
                    else
                    {
                        Session["create_error"] = "Old Password is invalid. Try Again";
                        Response.Redirect("~/Settings", false);
                    }
                }
            }
        }

        protected string getPasswordMinAge(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_passwordMinAge FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_passwordMinAge"] != null)
                        {
                            if (reader["account_passwordMinAge"] != DBNull.Value)
                            {
                                h = reader["account_passwordMinAge"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                //throw new Exception(ex.ToString());
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
                Response.Redirect("~/CustomError/Http500.aspx", true);
                //throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected bool AccountExistsUsingEmail(string email)
        {
            try
            {

                SqlConnection myConn = new SqlConnection(MYDBConnectionString);

                string sqlStmt = "Select * from Account where account_email = @paraEmail";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                DataSet ds = new DataSet();

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
                Response.Redirect("~/CustomError/Http500.aspx", true);
                return false;
                //throw new Exception(ex.ToString());
            }
        }

        protected int updatePassword(string email, string finalHash)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "UPDATE Account SET account_passwordHash = @account_passwordHash, account_passwordMinAge = @PasswordMinAge, account_passwordMaxAge = @PasswordMaxAge where account_email = @Email";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@Email", email);
                sqlCmd.Parameters.AddWithValue("@account_passwordHash", finalHash);
                sqlCmd.Parameters.AddWithValue("@PasswordMinAge", DateTime.Now.AddMinutes(5));
                sqlCmd.Parameters.AddWithValue("@PasswordMaxAge", DateTime.Now.AddMinutes(15));
                myConn.Open();

                int result = sqlCmd.ExecuteNonQuery();

                myConn.Close();

                return result;
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                return 0;
                //throw new Exception(ex.ToString());
            }
        }

        protected int updatePasswordOld1(string email, string finalHash)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "UPDATE Account SET account_passwordHashOld1 = @account_passwordHash, account_passwordMinAge = @PasswordMinAge, account_passwordMaxAge = @PasswordMaxAge where account_email = @Email";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@Email", email);
                sqlCmd.Parameters.AddWithValue("@account_passwordHash", finalHash);
                sqlCmd.Parameters.AddWithValue("@PasswordMinAge", DateTime.Now.AddMinutes(5));
                sqlCmd.Parameters.AddWithValue("@PasswordMaxAge", DateTime.Now.AddMinutes(15));
                myConn.Open();

                int result = sqlCmd.ExecuteNonQuery();

                myConn.Close();

                return result;
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                return 0;
                //throw new Exception(ex.ToString());
            }
        }

        protected int updatePasswordOld2(string email, string finalHash)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "UPDATE Account SET account_passwordHashOld2 = @account_passwordHash, account_passwordMinAge = @PasswordMinAge, account_passwordMaxAge = @PasswordMaxAge where account_email = @Email";

                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);

                sqlCmd.Parameters.AddWithValue("@Email", email);
                sqlCmd.Parameters.AddWithValue("@account_passwordHash", finalHash);
                sqlCmd.Parameters.AddWithValue("@PasswordMinAge", DateTime.Now.AddMinutes(5));
                sqlCmd.Parameters.AddWithValue("@PasswordMaxAge", DateTime.Now.AddMinutes(15));
                myConn.Open();

                int result = sqlCmd.ExecuteNonQuery();

                myConn.Close();

                return result;
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                return 0;
                //throw new Exception(ex.ToString());
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
                Response.Redirect("~/CustomError/Http500.aspx", true);
                //throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected string getDBHashOld1(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_passwordHashOld1 FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_passwordHashOld1"] != null)
                        {
                            if (reader["account_passwordHashOld1"] != DBNull.Value)
                            {
                                h = reader["account_passwordHashOld1"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                //throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected string getDBHashOld2(string email)
        {

            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select account_passwordHashOld2 FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_passwordHashOld2"] != null)
                        {
                            if (reader["account_passwordHashOld2"] != DBNull.Value)
                            {
                                h = reader["account_passwordHashOld2"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                //throw new Exception(ex.ToString());
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
                Response.Redirect("~/CustomError/Http500.aspx", true);
                //throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return s;
        }
    }
}