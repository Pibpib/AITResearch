using AITResearch.Model;
using AITResearch.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITResearch
{
    /// <summary>
    /// page to handle staff login and account creation
    /// </summary>
    public partial class StaffLogin : System.Web.UI.Page
    {

        private readonly StaffRepo staffRepo = new StaffRepo();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// handle staff login
        /// validate credentials and log in the staff member
        /// </summary>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            //get user input value
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            //check staff credentials in repository
            Staff staff = staffRepo.GetStaffByUsernameAndPassword(username, password);

            if (staff != null)
            {
                //success: save to session and redirect
                Session["StaffID"] = staff.StaffID;
                Session["Username"] = staff.Username;
                Session["Role"] = staff.Role;

                Response.Redirect("StaffSearch.aspx");
            }
            else
            {
                //invalid
                lblError.Text = "Invalid username or password.";
            }
        }

        /// <summary>
        /// handles creation of new staff account
        /// </summary>
        protected void btnCreateAccount_Click(object sender, EventArgs e)
        {
            string newUsername = txtNewUsername.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            //field validation
            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                lblCreateError.Text = "All fields are required.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                lblCreateError.Text = "Passwords do not match.";
                return;
            }

            //check if there's same username
            if (staffRepo.IsUsernameTaken(newUsername))
            {
                lblCreateError.Text = "Username is already taken. Please choose another.";
                return;
            }

            //insert new staff into database
            bool success = staffRepo.InsertStaff(newUsername, newPassword);
            if (success)
            {
                lblCreateError.ForeColor = System.Drawing.Color.Green;
                lblCreateError.Text = "Account created successfully.";
                txtNewUsername.Text = "";
                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            else
            {
                lblCreateError.ForeColor = System.Drawing.Color.Red;
                lblCreateError.Text = "Failed to create account. Please try again later.";
            }
        }
    }
}