<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StartSurvey.aspx.cs" Inherits="AITResearch.StartSurvey" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AIT Research Survey</title>
    <link href="StyleSheet1.css" rel="stylesheet"/>
    <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" />
</head>
<body>
    <!--top nav-->
    <div class="topnav">
        <a href="Index.aspx">Home</a>
        <a href="StartSurvey.aspx" class="active">Start Survey</a>
    </div>

    <div class="main-container">
        <form id="form1" runat="server">
            <div>
                <!--question form-->
                <asp:Label ID="lblQuestion" runat="server"></asp:Label>
                <br /><br />

                <asp:PlaceHolder ID="phInput" runat="server"></asp:PlaceHolder>
                <br /><br />

                <asp:Label ID="lblError" runat="server" Text="" ForeColor="Red"></asp:Label> <br />

                <div class="button-center">
                    <asp:LinkButton ID="btnPrev" CssClass="material-symbols-outlined button" runat="server" OnClick="btnPrev_Click">arrow_back</asp:LinkButton>
                    &nbsp;
                    <asp:LinkButton ID="btnNext" CssClass="material-symbols-outlined button" runat="server" OnClick="btnNext_Click">arrow_forward</asp:LinkButton>
                </div>

                <!--register panel shown at the last question-->
                <asp:Panel ID="pnlRegister" runat="server" Visible="false">
                    <asp:Label ID="lblRegisterQuestion" runat="server" Text="Would you like to register?"></asp:Label><br />
                    <asp:RadioButtonList ID="rblRegister" runat="server">
                        <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                        <asp:ListItem Text="No" Value="no"></asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Button ID="btnFinish" CssClass="button button-center" runat="server" Text="Finish" OnClick="btnFinish_Click" />
                </asp:Panel>
            </div>
        </form>
    </div>
</body>
</html>
