<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffSearch.aspx.cs" Inherits="AITResearch.StaffSearch" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <title>Staff Search Responses</title>
    <link href="StyleSheet1.css" rel="stylesheet"/>
</head>
<body style="height : auto">
    <!-- top navigation bar -->
    <div class="topnav">
        <a href="index.aspx">Home</a>
        <a href="StaffSearch.aspx" class="active">Search Responses</a>
    </div>

    <div class="main-container2">
        <form id="form2" runat="server">
            <div class="search-container">
                <h1>Search Survey Responses</h1>

                <!-- pick gender section -->
                <div class="form-group">
                    <label>Gender </label>
                    <asp:DropDownList ID="ddlGender" runat="server" AppendDataBoundItems="True" />
                </div>

                <!-- pick age range section -->
                <div class="form-group">
                    <label>Age Range </label>
                    <asp:DropDownList ID="ddlAgeRange" runat="server" AppendDataBoundItems="True" />
                </div>

                <!-- pick state / territory section -->
                <div class="form-group">
                    <label>State / Territory </label>
                    <asp:DropDownList ID="ddlState" runat="server" AppendDataBoundItems="True" />
                </div>

                <!-- pick banks used section -->
                <div class="form-group">
                    <label>Banks Used</label>
                    <asp:CheckBoxList ID="cblBanksUsed" runat="server" RepeatDirection="Vertical" />
                </div>
                
                <!-- pick commbank service section -->
                <div class="form-group">
                    <label>Bank Service (Commonwealth)</label>
                    <asp:CheckBoxList ID="cblServiceComm" runat="server" RepeatDirection="Vertical" />
                </div>

                 <!-- pick westpac service section -->
                <div class="form-group">
                    <label>Bank Service (Westpac)</label>
                    <asp:CheckBoxList ID="cblServiceWest" runat="server" RepeatDirection="Vertical" />
                </div>

                 <!-- pick anz service section-->
                <div class="form-group">
                    <label>Bank Service (ANZ)</label>
                    <asp:CheckBoxList ID="cblServiceANZ" runat="server" RepeatDirection="Vertical" />
                </div>

                 <!-- pick newspaper name section -->
                <div class="form-group">
                    <label>Newspapers Read </label>
                    <asp:CheckBoxList ID="cblNewspapersRead" runat="server" RepeatDirection="Vertical" />
                </div>

                 <!-- clear and search button -->
                <div class="button-center">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button" OnClick="btnClear_Click" />
                </div>

                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
            </div>

             <!-- search result -->
            <div class="results-container">
                <h2>Search Results</h2>
                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="True" CssClass="table" />
            </div>
        </form>
    </div>
</body>
</html>
