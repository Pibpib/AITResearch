using AITResearch.Model;
using AITResearch.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace AITResearch
{
    /// <summary>
    /// handles survey start page
    /// create respondent, question navigation, answer storage, survey complletion
    /// </summary>
    public partial class StartSurvey : System.Web.UI.Page
    {
        private readonly RespondentRepo respondentRepo = new RespondentRepo();
        private readonly QuestionRepo questionRepo = new QuestionRepo();
        private readonly ChoiceRepo choiceRepo = new ChoiceRepo();
        private readonly ResponseRepo responseRepo = new ResponseRepo();
        private readonly AttendanceRepo attendanceRepo = new AttendanceRepo();

        /// <summary>
        /// initialise respondent session, attendance, load first question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //create new respondent & attendance in session
                    if (Session["RespondentID"] == null)
                    {
                        int newRespondentId = respondentRepo.CreateRespondent();
                        Session["RespondentID"] = newRespondentId;

                        string hn = Dns.GetHostName();
                        string ipAddress = Dns.GetHostByName(hn).AddressList[0].ToString();
                        attendanceRepo.CreateAttendance(newRespondentId, ipAddress);

                        Session["TempResponses"] = new List<Response>();
                    }

                    //load first parent question
                    var firstQuestion = questionRepo.GetFirstTopLevelQuestion();
                    if (firstQuestion != null)
                    {
                        var flow = new List<int> { firstQuestion.QuestionID };
                        ViewState["QuestionFlow"] = flow;
                        ViewState["CurrentIndex"] = 0;
                        ViewState["LastTopLevelDisplayOrder"] = firstQuestion.DisplayOrder;

                        LoadQuestion(firstQuestion.QuestionID);
                    }
                }
                else
                {
                    //load current question after postback
                    var flow = (List<int>)ViewState["QuestionFlow"];
                    int currentIndex = (int)ViewState["CurrentIndex"];
                    LoadQuestion(flow[currentIndex]);
                }
            }
            catch (SqlException ex) { lblError.Text = "Page load database error: " + ex.Message; }
            catch (Exception ex) { lblError.Text = "Page load unexpected error: " + ex.Message; }
        }

        /// <summary>
        /// load question, its input type, fill saved answer (if exists)
        /// </summary>
        /// <param name="questionId"> ID of the question to load</param>
        private void LoadQuestion(int questionId)
        {
            try
            {
                var question = questionRepo.GetQuestion(questionId);
                if (question == null) return;

                lblQuestion.Text = question.QuestionText;
                phInput.Controls.Clear();

                //load answer if exists 
                var tempResponses = Session["TempResponses"] as List<Response>;
                var previousAnswer = tempResponses?.FirstOrDefault(r => r.QuestionID == questionId)?.AnswerValue;

                //render input control based on question input type
                switch (question.InputType)
                {
                    case "text":
                    case "date":
                        var tb = new TextBox { ID = "txtAnswer" };
                        if (question.InputType == "date") tb.TextMode = TextBoxMode.Date;
                        if (!string.IsNullOrEmpty(previousAnswer)) tb.Text = previousAnswer;
                        phInput.Controls.Add(tb);
                        break;

                    case "dropdown":
                        var ddl = new DropDownList { ID = "ddlAnswer" };
                        foreach (var choice in choiceRepo.GetChoices(questionId))
                            ddl.Items.Add(new ListItem(choice.ChoiceLabel, choice.ChoiceID.ToString()));
                        if (!string.IsNullOrEmpty(previousAnswer)) ddl.SelectedValue = previousAnswer;
                        phInput.Controls.Add(ddl);
                        break;

                    case "radio":
                        var rbl = new RadioButtonList { ID = "rblAnswer" };
                        foreach (var choice in choiceRepo.GetChoices(questionId))
                            rbl.Items.Add(new ListItem(choice.ChoiceLabel, choice.ChoiceID.ToString()));
                        if (!string.IsNullOrEmpty(previousAnswer)) rbl.SelectedValue = previousAnswer;
                        phInput.Controls.Add(rbl);
                        break;

                    case "checkbox":
                        var selectedIds = !string.IsNullOrEmpty(previousAnswer)
                            ? previousAnswer.Split(',') : new string[0];
                        foreach (var choice in choiceRepo.GetChoices(questionId))
                        {
                            var cb = new CheckBox
                            {
                                ID = "chk_" + choice.ChoiceID,
                                Text = choice.ChoiceLabel,
                                Checked = selectedIds.Contains(choice.ChoiceID.ToString())
                            };
                            phInput.Controls.Add(cb);
                            phInput.Controls.Add(new LiteralControl("<br/>"));
                        }
                        break;
                }
            }
            catch (InvalidCastException ex)
            {
                lblError.Text = "Load question data type issue: " + ex.Message;
            }
            catch (SqlException ex)
            {
                lblError.Text = "Load question database error: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblError.Text = "Load question failed to load question: " + ex.Message;
            }
        }

        /// <summary>
        /// handle next button click, saving current answer, determine next question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                var flow = (List<int>)ViewState["QuestionFlow"];
                int currentIndex = (int)ViewState["CurrentIndex"];
                int currentQuestionId = flow[currentIndex];
                var currentQuestion = questionRepo.GetQuestion(currentQuestionId);

                var answers = GetAnswersFromUI(currentQuestion);

                // validate answer
                if (!ValidateAnswer(currentQuestion, answers, out string error))
                {
                    lblError.Text = error;
                    return;
                }

                lblError.Text = "";

                //save cuurent answer
                SaveAnswer(currentQuestionId, answers);

                //store display order of last top-levvel question
                if (currentQuestion.ParentQuestionID == null)
                    ViewState["LastTopLevelDisplayOrder"] = currentQuestion.DisplayOrder;

                //convert answer to list of choice IDs for branching logic
                List<string> answerChoiceIds = (currentQuestion.InputType == "checkbox" ||
                                               currentQuestion.InputType == "dropdown" ||
                                               currentQuestion.InputType == "radio") ? answers : new List<string>();

                
                //remove child questions from the flow that no longer meet condition
                var tempResponses = Session["TempResponses"] as List<Response>;
                for (int i = flow.Count - 1; i > currentIndex; i--)
                {
                    var q = questionRepo.GetQuestion(flow[i]);
                    //check if there's any existing child items linked to current parent and remove them if they no longer meet a certain conditions"
                    if (IsDescendant(q, currentQuestionId) && !answerChoiceIds.Contains(q.ConditionValue))
                    {
                        //remove child question from flow
                        flow.RemoveAt(i);

                        //remove old saved answer
                        if (tempResponses != null)
                            tempResponses.RemoveAll(r => r.QuestionID == q.QuestionID);
                    }
                }

                //get new children based on current answer
                var childQuestions = questionRepo.GetChildQuestions(currentQuestionId, answerChoiceIds);
               
                //find child question if theres any
                if (childQuestions.Any())
                {
                    //insert child question after current question
                    flow.InsertRange(currentIndex + 1, childQuestions.Select(q => q.QuestionID));
                    ViewState["QuestionFlow"] = flow;
                    ViewState["CurrentIndex"] = currentIndex + 1;
                    LoadQuestion(childQuestions[0].QuestionID);
                }
                else
                {
                    if (currentIndex + 1 < flow.Count)
                    {
                        //go to the next question
                        ViewState["CurrentIndex"] = currentIndex + 1;
                        LoadQuestion(flow[currentIndex + 1]);
                    }
                    else
                    {
                        //next question or finish
                        int lastTopOrder = (int)ViewState["LastTopLevelDisplayOrder"];
                        var nextTop = questionRepo.GetNextTopLevelQuestion(lastTopOrder);
                        if (nextTop != null)
                        {
                            flow.Add(nextTop.QuestionID);
                            ViewState["QuestionFlow"] = flow;
                            ViewState["CurrentIndex"] = currentIndex + 1;
                            ViewState["LastTopLevelDisplayOrder"] = nextTop.DisplayOrder;
                            LoadQuestion(nextTop.QuestionID);
                        }
                        else
                        {
                            //save answers to database
                            if (tempResponses != null && tempResponses.Any())
                                foreach (var r in tempResponses)
                                    responseRepo.SaveResponse(r);
                            Session.Remove("TempResponses");

                            lblQuestion.Text = "";
                            phInput.Controls.Clear();
                            btnNext.Visible = false;
                            btnPrev.Visible = false;
                            pnlRegister.Visible = true;
                        }
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                lblError.Text = "Button next click data type issue: " + ex.Message;
            }
            catch (SqlException ex)
            {
                lblError.Text = "Button next click database error: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblError.Text = "Button next click unexpected error: " + ex.Message;
            }
        }

        /// <summary>
        /// handle previous button click
        /// save current answer and load previous question
        /// </summary>
        protected void btnPrev_Click(object sender, EventArgs e)
        {
            try
            {
                var flow = (List<int>)ViewState["QuestionFlow"];
                int currentIndex = (int)ViewState["CurrentIndex"];

                //save current answer before moving back
                var question = questionRepo.GetQuestion(flow[currentIndex]);
                SaveAnswer(flow[currentIndex], GetAnswersFromUI(question));

                //go back if possible
                if (currentIndex > 0)
                {
                    currentIndex--;
                    ViewState["CurrentIndex"] = currentIndex;
                    LoadQuestion(flow[currentIndex]);
                }
                lblError.Text = "";
            }
            catch (InvalidCastException ex)
            {
                lblError.Text = "Button prev click data type error: " + ex.Message;
            }
            catch (NullReferenceException ex)
            {
                lblError.Text = "Button prev click missing data: " + ex.Message;
            }
            catch (SqlException ex)
            {
                lblError.Text = "Button prev click database error: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblError.Text = "Button prev click unexpected error: " + ex.Message;
            }
        }

        /// <summary>
        /// get answers from ui controls for the given question
        /// </summary>
        /// <returns>list of answer</returns>
        private List<string> GetAnswersFromUI(Question question)
        {
            var answers = new List<string>();

            switch (question.InputType)
            {
                case "text":
                case "date":
                    var tb = phInput.FindControl("txtAnswer") as TextBox;
                    if (tb != null && !string.IsNullOrWhiteSpace(tb.Text))
                        answers.Add(tb.Text.Trim());
                    break;

                case "dropdown":
                    var ddl = phInput.FindControl("ddlAnswer") as DropDownList;
                    if (ddl != null && !string.IsNullOrEmpty(ddl.SelectedValue))
                        answers.Add(ddl.SelectedValue);
                    break;

                case "radio":
                    var rbl = phInput.FindControl("rblAnswer") as RadioButtonList;
                    if (rbl != null && !string.IsNullOrEmpty(rbl.SelectedValue))
                        answers.Add(rbl.SelectedValue);
                    break;

                case "checkbox":
                    foreach (var control in phInput.Controls)
                    {
                        if (control is CheckBox cb && cb.Checked)
                        {
                            answers.Add(cb.ID.Substring(4));
                        }
                    }
                    break;
            }
            return answers;
        }

        /// <summary>
        /// save answer for specified question in the session's responses list 
        /// </summary>
        private void SaveAnswer(int questionId, List<string> answers)
        {
            var answerValue = string.Join(",", answers);

            //make sure temp responses list exists in session
            var tempResponses = Session["TempResponses"] as List<Response> ?? new List<Response>();
            Session["TempResponses"] = tempResponses;

            int respondentId = Convert.ToInt32(Session["RespondentID"]);
            var existing = tempResponses.FirstOrDefault(r => r.QuestionID == questionId);

            if (existing != null)
                existing.AnswerValue = answerValue;
            else
                tempResponses.Add(new Response
                {
                    RespondentID = respondentId,
                    QuestionID = questionId,
                    AnswerValue = answerValue
                });
        }

        /// <summary>
        /// check if question is a descendant of a given parent question id
        /// </summary>
        /// <returns>true if it is a descendent, false if its not</returns>
        private bool IsDescendant(Question q, int parentId)
        {
            while (q.ParentQuestionID != null)
            {
                if (q.ParentQuestionID == parentId)
                    return true;
                q = questionRepo.GetQuestion(q.ParentQuestionID.Value);
            }
            return false;
        }

        /// <summary>
        /// handles finish button click
        /// display completion message and redirect to registration (if user wants to)
        /// </summary>
        protected void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(rblRegister.SelectedValue))
                {
                    lblError.Text = "Please select Yes or No.";
                    return;
                }

                pnlRegister.Visible = false;

                if (rblRegister.SelectedValue == "yes")
                    Response.Redirect("~/Register.aspx");
                else
                {
                    lblQuestion.Text = "Survey complete! Thank you!";
                    phInput.Controls.Clear();
                    phInput.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to finish: " + ex.Message;
            }
        }

        /// <summary>
        /// validates given answer against question requirement (required, min/max choice, validation pattern)
        /// </summary>
        /// <param name="question">question to validate</param>
        /// <param name="answers">list of answers provided by the user</param>
        public bool ValidateAnswer(Question question, List<string> answers, out string errorMsg)
        {
            errorMsg = null;

            //required field check
            if (question.IsRequired && (answers == null || !answers.Any() || answers.All(string.IsNullOrWhiteSpace)))
            {
                errorMsg = "This question is required.";
                return false;
            }

            //checkbox min/max answer check
            if (question.InputType == "checkbox")
            {
                if (question.MaxAnswer.HasValue && answers.Count > question.MaxAnswer.Value)
                {
                    errorMsg = $"You can select at most {question.MaxAnswer.Value} options.";
                    return false;
                }

                if (question.MinAnswer.HasValue && answers.Count < question.MinAnswer.Value)
                {
                    errorMsg = $"You must select at least {question.MinAnswer.Value} options.";
                    return false;
                }
            }

            //pattern validation for text input
            if (question.InputType == "text" && !string.IsNullOrWhiteSpace(question.ValidationPattern))
            {
                var answer = answers.FirstOrDefault() ?? "";
                if (!Regex.IsMatch(answer, question.ValidationPattern))
                {
                    errorMsg = "Your answer is not in the correct format.";
                    return false;
                }
            }

            return true;
        }
    }
}
