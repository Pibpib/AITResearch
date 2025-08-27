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
    //page to handle respondent registration after completing survey
    public partial class Register : System.Web.UI.Page
    {
        private readonly RespondentProfileRepo profileRepo = new RespondentProfileRepo();
        private readonly RespondentRepo respondentRepo = new RespondentRepo();
        private readonly ChoiceRepo choiceRepo = new ChoiceRepo();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// submit button click event to register respondent profile
        /// </summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                //check if session respondent id doen't exists
                if (Session["RespondentID"] == null)
                {
                    lblMessage.Text = "Session expired. Please start the survey again.";
                    return;
                }

                //get respondent ID from session
                int respondentId;
                if (!int.TryParse(Session["RespondentID"].ToString(), out respondentId))
                {
                    lblMessage.Text = "Invalid session data.";
                    return;
                }

                //get input values
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string phone = txtPhone.Text.Trim();

                //validate required fields
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    lblMessage.Text = "First and last name are required.";
                    return;
                }

                //validate DOB input
                if (!DateTime.TryParse(txtDOB.Text, out DateTime dob))
                {
                    lblMessage.Text = "Please enter a valid date of birth.";
                    return;
                }

                // Validate DOB matches selected age range in survey age range 
                int? ageChoiceId;
                try
                {
                    ageChoiceId = profileRepo.GetRespondentAgeRangeChoice(respondentId);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in GetRespondentAgeRangeChoice (RespondentID={respondentId}): {ex.Message}", ex);
                }

                if (ageChoiceId == null)
                {
                    lblMessage.Text = "Cannot verify age range from your survey response.";
                    return;
                }

                if (!IsDobMatchingAgeRange(dob, ageChoiceId.Value))
                {
                    lblMessage.Text = "Your date of birth does not match the age range you selected in the survey.";
                    return;
                }

                //change respondent to not anonymous
                try
                {
                    if (!respondentRepo.UpdateRespondentToNotAnonymous(respondentId))
                    {
                        lblMessage.Text = "Failed to update respondent status. Please contact support.";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in UpdateRespondentToNotAnonymous (RespondentID={respondentId}): {ex.Message}", ex);
                }

                //update profile
                var profile = new RespondentProfile
                {
                    RespondentID = respondentId,
                    FirstName = firstName,
                    LastName = lastName,
                    DOB = dob,
                    PhoneNumber = phone
                };

                try
                {
                    if (profileRepo.UpdateRespondentProfile(profile))
                    {
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Registration complete! Thank you.";
                        btnSubmit.Enabled = false;

                        // Redirect to Index page after successful registration
                        Response.Redirect("Index.aspx");
                    }
                    else
                    {
                        lblMessage.Text = "No profile was updated. Please check your details.";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error in UpdateRespondentProfile (RespondentID={respondentId}): {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An unexpected error occurred. Please try again later.";
            }
        }

        /// <summary>
        /// checks if the DOB matches the age range choice selected in the survey.
        /// </summary>
        /// <param name="dob">date of birth of the respondent</param>
        /// <param name="choiceId">choice ID representing the age range in the survey</param>
        /// <returns>true if DOB matches the age range or false if failed</returns>
        private bool IsDobMatchingAgeRange(DateTime dob, int choiceId)
        {
            try
            {
                //calculate age
                int age = DateTime.Now.Year - dob.Year;
                if (dob > DateTime.Now.AddYears(-age)) age--; // adjust if birthday hasn't occurred yet

                //get age range from choice
                var range = choiceRepo.GetAgeRangeByChoice(choiceId);
                if (range == null) return false;

                //compage age with min/max range
                if (range.Value.MaxAge.HasValue)
                    return age >= range.Value.MinAge && age <= range.Value.MaxAge.Value;
                else
                    return age >= range.Value.MinAge;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in IsDobMatchingAgeRange (ChoiceID={choiceId}, DOB={dob.ToShortDateString()}): {ex.Message}", ex);
            }
        }
    }
}