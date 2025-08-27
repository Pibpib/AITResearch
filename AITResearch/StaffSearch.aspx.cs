using AITResearch.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AITResearch
{
    /// <summary>
    /// page to allow staff to search survey respondents based on multiple criteria
    /// </summary>
    public partial class StaffSearch : System.Web.UI.Page
    {
        private ChoiceRepo choiceRepo = new ChoiceRepo();
        private ViewRepo viewRepo = new ViewRepo();

        /// <summary>
        /// handles page load and initialises dropdows, checkboxes, data table 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    //load dropdowns, checkboxes, and initial data table
                    BindDropDowns();
                    BindCheckBoxLists();
                    BindPivotResults();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error in Page_Load while initializing page data: " + ex.Message;
                }
            }
        }

        /// <summary>
        /// bind dropdowns for gender, age  range and state
        /// </summary>
        private void BindDropDowns()
        {
            try
            {
                //gender dropdown (QuestionID=1)
                ddlGender.DataSource = choiceRepo.GetChoices(1);
                ddlGender.DataTextField = "ChoiceLabel";
                ddlGender.DataValueField = "ChoiceLabel";
                ddlGender.DataBind();
                ddlGender.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Gender", ""));

                //age range dropdown (QuestionID=2)
                ddlAgeRange.DataSource = choiceRepo.GetChoices(2);
                ddlAgeRange.DataTextField = "ChoiceLabel";
                ddlAgeRange.DataValueField = "ChoiceLabel";
                ddlAgeRange.DataBind();
                ddlAgeRange.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Age Range", ""));

                //state dropdown (QuestionID=3)
                ddlState.DataSource = choiceRepo.GetChoices(3);
                ddlState.DataTextField = "ChoiceLabel";
                ddlState.DataValueField = "ChoiceLabel";
                ddlState.DataBind();
                ddlState.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select State", ""));
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BindDropDowns: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// load data into checkbox lists
        /// </summary>
        private void BindCheckBoxLists()
        {
            try
            {
                // Banks Used checkbox list
                cblBanksUsed.DataSource = choiceRepo.GetChoices(7);
                cblBanksUsed.DataTextField = "ChoiceLabel";
                cblBanksUsed.DataValueField = "ChoiceLabel";
                cblBanksUsed.DataBind();

                //services per bank
                // Commbank Bank Service checkbox list
                var commChoices = choiceRepo.GetChoices(10)
                    .Select(c => new {
                        ChoiceLabel = c.ChoiceLabel,
                        ValueWithPrefix = "Commonwealth_" + c.ChoiceLabel
                    }).ToList();
                cblServiceComm.DataSource = commChoices;
                cblServiceComm.DataTextField = "ChoiceLabel";
                cblServiceComm.DataValueField = "ValueWithPrefix";
                cblServiceComm.DataBind();

                // Westpac Bank Service checkbox list
                var westChoices = choiceRepo.GetChoices(11)
                    .Select(c => new {
                        ChoiceLabel = c.ChoiceLabel,
                        ValueWithPrefix = "Westpac_" + c.ChoiceLabel
                    }).ToList();
                cblServiceWest.DataSource = westChoices;
                cblServiceWest.DataTextField = "ChoiceLabel";
                cblServiceWest.DataValueField = "ValueWithPrefix";
                cblServiceWest.DataBind();

                // ANZ Bank Service checkbox list
                var anzChoices = choiceRepo.GetChoices(12)
                    .Select(c => new {
                        ChoiceLabel = c.ChoiceLabel,
                        ValueWithPrefix = "ANZ_" + c.ChoiceLabel
                    }).ToList();
                cblServiceANZ.DataSource = anzChoices;
                cblServiceANZ.DataTextField = "ChoiceLabel";
                cblServiceANZ.DataValueField = "ValueWithPrefix";
                cblServiceANZ.DataBind();

                //newspapers Read checkbox list
                cblNewspapersRead.DataSource = choiceRepo.GetChoices(8);
                cblNewspapersRead.DataTextField = "ChoiceLabel";
                cblNewspapersRead.DataValueField = "ChoiceLabel";
                cblNewspapersRead.DataBind();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in BindCheckBoxLists: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// bind survey result pivot table to grid view
        /// </summary>
        protected void BindPivotResults()
        {
            try
            {
                DataTable dt = viewRepo.GetSurveyResultsPivot();

                gvResults.DataSource = dt;
                gvResults.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = $"BindPivotResults error: {ex.Message}";
            }
        }

        /// <summary>
        /// handle search button click event to filter survey result based on user selection
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //get selected values from dropdowns
                string gender = ddlGender.SelectedValue;
                string ageRange = ddlAgeRange.SelectedValue;
                string state = ddlState.SelectedValue;

                //get selected banks
                List<string> selectedBanks = cblBanksUsed.Items.Cast<ListItem>()
                                          .Where(i => i.Selected)
                                          .Select(i => i.Text)
                                          .ToList();

                //get selected newspaper
                List<string> selectedNewspapers = cblNewspapersRead.Items.Cast<ListItem>()
                                            .Where(i => i.Selected)
                                            .Select(i => i.Text)
                                            .ToList();

                //get selected services per bank
                List<string> selectedServiceComm = cblServiceComm.Items.Cast<ListItem>()
                                                .Where(i => i.Selected)
                                                .Select(i => i.Value)  // use Value, not Text
                                                .ToList();

                List<string> selectedServiceWest = cblServiceWest.Items.Cast<ListItem>()
                                                .Where(i => i.Selected)
                                                .Select(i => i.Value)
                                                .ToList();

                List<string> selectedServiceANZ = cblServiceANZ.Items.Cast<ListItem>()
                                                .Where(i => i.Selected)
                                                .Select(i => i.Value)
                                                .ToList();

                //get data table and filter
                DataTable dt = viewRepo.GetSurveyResultsPivot();
                DataTable filteredDt = FilterSurveyResults(dt, gender, ageRange, state,
                                           selectedBanks, selectedNewspapers,
                                           selectedServiceComm, selectedServiceWest, selectedServiceANZ);

                //display filtered data
                gvResults.DataSource = filteredDt;
                gvResults.DataBind();

                //show message if no result found
                if (filteredDt.Rows.Count == 0)
                {
                    lblError.Text = "No results found matching your criteria.";
                }
                else
                {
                    lblError.Text = string.Empty;
                }
            }
            catch (InvalidCastException icex)
            {
                lblError.Text = "Data type error during search: " + icex.Message;
            }
            catch (Exception ex)
            {
                lblError.Text = "Error performing search: " + ex.Message;
            }
        }

        /// <summary>
        /// reset all inputs and show all results again
        /// </summary>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                //reset dropdowns
                ddlGender.SelectedIndex = 0;
                ddlAgeRange.SelectedIndex = 0;
                ddlState.SelectedIndex = 0;

                //uncheck all checkboxes
                foreach (ListItem item in cblBanksUsed.Items) item.Selected = false;
                foreach (ListItem item in cblNewspapersRead.Items) item.Selected = false;
                foreach (ListItem item in cblServiceComm.Items) item.Selected = false;
                foreach (ListItem item in cblServiceWest.Items) item.Selected = false;
                foreach (ListItem item in cblServiceANZ.Items) item.Selected = false;

                //clear gridview and rebind data table
                gvResults.DataSource = null;
                gvResults.DataBind();
                BindPivotResults();

                lblError.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error when clicking clear button: {ex.Message}";
            }
        }

        /// <summary>
        /// filter survey based on selected criteria
        /// </summary>
        /// <param name="dt">Original DataTable of survey results</param>
        /// <param name="gender">Selected gender</param>
        /// <param name="ageRange">Selected age range></param>
        /// <param name="state">Selected state</param>
        /// <param name="selectedBanks">Selected banks</param>
        /// <param name="selectedNewspapers">>Selected newspapers</param>
        /// <param name="selectedServiceComm">Selected Commonwealth bank service</param>
        /// <param name="selectedServiceWest">Selected Westpac bank service</param>
        /// <param name="selectedServiceANZ">Selected ANZ bank service</param>
        /// <returns>Filtered DataTable matching the selected criteria</returns>
        private DataTable FilterSurveyResults(DataTable dt, string gender, string ageRange, string state,
                                     List<string> selectedBanks, List<string> selectedNewspapers,
                                     List<string> selectedServiceComm, List<string> selectedServiceWest,
                                     List<string> selectedServiceANZ)
        {
            try
            {
                var query = dt.AsEnumerable();

                //filter by gender
                if (!string.IsNullOrEmpty(gender))
                    query = query.Where(row => row.Field<string>("Gender") == gender);

                //filter by age range
                if (!string.IsNullOrEmpty(ageRange))
                    query = query.Where(row => row.Field<string>("AgeRange") == ageRange);

                //filter by state
                if (!string.IsNullOrEmpty(state))
                    query = query.Where(row => row.Field<string>("State") == state);

                //filter by selected banks
                if (selectedBanks != null && selectedBanks.Count > 0)
                {
                    foreach (string bank in selectedBanks)
                    {
                        if (dt.Columns.Contains(bank))
                            query = query.Where(row => row.Field<int?>(bank) == 1);
                    }
                }

                //filter by selected newspapers
                if (selectedNewspapers != null && selectedNewspapers.Count > 0)
                {
                    foreach (string news in selectedNewspapers)
                    {
                        if (dt.Columns.Contains(news))
                            query = query.Where(row => row.Field<int?>(news) == 1);
                    }
                }

                //filter by selected bank services for commbank, westpac, anz
                if (selectedServiceComm != null && selectedServiceComm.Count > 0)
                {
                    foreach (string colName in selectedServiceComm)
                    {
                        if (dt.Columns.Contains(colName))
                            query = query.Where(row => row.Field<int?>(colName) == 1);
                    }
                }

                if (selectedServiceWest != null && selectedServiceWest.Count > 0)
                {
                    foreach (string colName in selectedServiceWest)
                    {
                        if (dt.Columns.Contains(colName))
                            query = query.Where(row => row.Field<int?>(colName) == 1);
                    }
                }

                if (selectedServiceANZ != null && selectedServiceANZ.Count > 0)
                {
                    foreach (string colName in selectedServiceANZ)
                    {
                        if (dt.Columns.Contains(colName))
                            query = query.Where(row => row.Field<int?>(colName) == 1);
                    }
                }

                //return filtered results or empty table
                return query.Any() ? query.CopyToDataTable() : dt.Clone();
            }
            catch (Exception ex)
            {
                throw new Exception("Error filtering survey results: " + ex.Message, ex);
            }
        }
    }
}