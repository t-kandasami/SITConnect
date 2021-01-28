using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Profile : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        byte[] ccard = null;
        string email = null;

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
                    if (Convert.ToDateTime(getPasswordMaxAge(email)) < DateTime.Now)
                    {
                        Response.Redirect("~/Settings", false);
                    }
                    else
                    {
                        displayUserProfile(email);
                    }
                }
            }
            else
            {
                Response.Redirect("~/Login", false);
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the desrypted bytes from decrypting stream
                            // and places them in a string 
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
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
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
            return h;
        }

        protected void displayUserProfile(string email)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "SELECT * FROM Account WHERE account_email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["account_fname"] != DBNull.Value)
                        {
                            lbl_fname.Text = reader["account_fname"].ToString();
                        }
                        if (reader["account_lname"] != DBNull.Value)
                        {
                            lbl_lname.Text = reader["account_lname"].ToString();
                        }
                        if (reader["account_ccard"] != DBNull.Value)
                        {
                            ccard = Convert.FromBase64String(reader["account_ccard"].ToString());
                        }
                        if (reader["account_email"] != DBNull.Value)
                        {
                            lbl_email.Text = reader["account_email"].ToString();
                        }
                        if (reader["account_iv"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["account_iv"].ToString());
                        }
                        if (reader["account_key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["account_key"].ToString());
                        }
                        if (reader["account_dob"] != DBNull.Value)
                        {
                            lbl_dob.Text = reader["account_dob"].ToString();
                        }
                    }
                    lbl_ccard.Text = decryptData(ccard);
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
        }

        protected void changePwd_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Settings", false);
        }

        protected void logoutBtn_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);
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
    }
}