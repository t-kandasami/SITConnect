using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserEmail"] == null)
            {
                if (Session["create_error"] != null)
                {
                    Page.Items["create_error"] = Session["create_error"].ToString();
                    Session["create_error"] = null;
                }
            }
            else
            {
                Response.Redirect("~/Profile", false);
            }
        }

        protected void regBtn_Click(object sender, EventArgs e)
        {
            bool validInput = ValidateInput();

            List<String> errList = new List<string>();

            if (validInput)
            {
                string pwd = pwdTB.Text.ToString().Trim();
                 
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));

                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;

                createAccount();

                Session["create_success"] = "Account is Created Successfully";
                Response.Redirect("~/Login", false);
                
            }
            else
            {
                Response.Redirect("~/Registration", false);
            }
        }

        protected int createAccount()
        {
            try
            {   
                SqlConnection myConn = new SqlConnection(MYDBConnectionString);

                string sqlStmt = "INSERT INTO Account VALUES(@Fname, @Lname,@Ccard,@Email,@PasswordHash,@PasswordHashOld1,@PasswordHashOld2,@PasswordSalt,@Dob,@IV,@Key,@InvalidAttempts,@LastInvalidTime,@PasswordMinAge,@PasswordMaxAge)";
                SqlCommand cmd = new SqlCommand(sqlStmt, myConn);

                cmd.Parameters.AddWithValue("@Fname", HttpUtility.HtmlEncode(fnameTB.Text.Trim()));
                cmd.Parameters.AddWithValue("@Lname", HttpUtility.HtmlEncode(lnameTB.Text.Trim()));
                cmd.Parameters.AddWithValue("@Ccard", Convert.ToBase64String(encryptData(HttpUtility.HtmlEncode(ccardTB.Text.Trim()))));
                cmd.Parameters.AddWithValue("@Email", HttpUtility.HtmlEncode(emailTB.Text.Trim()));
                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                cmd.Parameters.AddWithValue("@PasswordHashOld1", "");
                cmd.Parameters.AddWithValue("@PasswordHashOld2", "");
                cmd.Parameters.AddWithValue("@Dob", HttpUtility.HtmlEncode(dobTB.Text.Trim()));
                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
                cmd.Parameters.AddWithValue("@InvalidAttempts", HttpUtility.HtmlEncode(0));
                cmd.Parameters.AddWithValue("@LastInvalidTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@PasswordMinAge", DateTime.Now.AddMinutes(5));
                cmd.Parameters.AddWithValue("@PasswordMaxAge", DateTime.Now.AddMinutes(15));

                myConn.Open();
                int result = cmd.ExecuteNonQuery();
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

        protected bool AccountExistsUsingEmail(string email)
        {
            try
            {
                SqlConnection myConn = new SqlConnection(MYDBConnectionString);
                string sqlStmt = "Select * from Account where account_email = @Email";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@Email", email);
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

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                Response.Redirect("~/CustomError/Http500.aspx", true);
                return null;
                //throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        private bool ValidateInput()
        {
            List<String> errList = new List<string>();

            int scores = checkPwd(pwdTB.Text);

            if (AccountExistsUsingEmail(emailTB.Text) == true)
            {
                errList.Add("Registration is Invalid. Account Exists");
            }
            if (String.IsNullOrEmpty(fnameTB.Text))
            {
                errList.Add("First Name is required");
            }
            if (String.IsNullOrEmpty(lnameTB.Text))
            {
                errList.Add("Last Name is required");
            }
            if (String.IsNullOrEmpty(ccardTB.Text))
            {
                errList.Add("Credit Card Number is required");
            }
            if (String.IsNullOrEmpty(emailTB.Text))
            {
                errList.Add("Email is required");
            }
            if (String.IsNullOrEmpty(pwdTB.Text))
            {
                errList.Add("Password is required");
            }
            if (String.IsNullOrEmpty(conpwdTB.Text))
            {
                errList.Add("Retype your password");
            }
            if (String.IsNullOrEmpty(dobTB.Text))
            {
                errList.Add("Date of Birth is required");
            }
            if (scores < 4)
            {
                errList.Add("Password is not accepted");
            }
            if (pwdTB.Text != conpwdTB.Text)
            {
                errList.Add("Passwords do not match");
            }

            if (errList.Count > 0)
            {
                Session["create_error"] = String.Join(",", errList);
                return false;
            }
            else
            {
                return true;
            }

        }

        private int checkPwd(string pwd)
        {
            int score = 0;
            if (pwd.Length < 8)
            {
                return 0;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(pwd, "[a-z]"))
            {
                score++;
            }
            if (Regex.IsMatch(pwd, "[A-Z]"))
            {
                score++;
            }
            if (Regex.IsMatch(pwd, "[0-9]"))
            {
                score++;
            }
            if (Regex.IsMatch(pwd, "[^A-Za-z0-9]"))
            {
                score++;
            }
            return score;
        }
    }
}