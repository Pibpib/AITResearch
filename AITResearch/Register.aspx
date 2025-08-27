<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="AITResearch.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register</title>
    <link href="StyleSheet1.css" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <!-- page heading -->
        <h2>Register as Member</h2>

        <!-- given names input field -->
        <div class="form-group">
            <label for="txtFirstName">Given Names:</label>
            <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
            <!-- input field validator -->
            <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                ControlToValidate="txtFirstName"
                ErrorMessage="First name is required."
                ForeColor="Red" Display="Dynamic" />
        </div>

        <!-- last names input field -->
        <div class="form-group">
            <label for="txtLastName">Last Name:</label>
            <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
            <!-- input field validator -->
            <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                ControlToValidate="txtLastName"
                ErrorMessage="Last name is required."
                ForeColor="Red" Display="Dynamic" />
        </div>

        <!-- date of birth input field -->
        <div class="form-group">
            <label for="txtDOB">Date of Birth:</label>
            <asp:TextBox ID="txtDOB" runat="server" TextMode="Date"></asp:TextBox>
            <!-- input field validator -->
            <asp:RequiredFieldValidator ID="rfvDOB" runat="server"
                ControlToValidate="txtDOB"
                ErrorMessage="Date of Birth is required."
                ForeColor="Red" Display="Dynamic" />
        </div>

         <!--phone number input field -->
        <div class="form-group">
            <label for="txtPhone">Contact Phone Number:</label>
            <asp:TextBox ID="txtPhone" runat="server" TextMode="Phone"></asp:TextBox>
            <!-- input field validator -->
            <asp:RequiredFieldValidator ID="rfvPhone" runat="server"
                ControlToValidate="txtPhone"
                ErrorMessage="Phone number is required."
                ForeColor="Red" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revPhone" runat="server"
                ControlToValidate="txtPhone"
                ValidationExpression="^\+?[0-9]{7,15}$"
                ErrorMessage="Enter a valid phone number (numbers only)."
                ForeColor="Red" Display="Dynamic" />
        </div>

        <!--submit button -->
        <div class="btn">
            <asp:Button ID="btnSubmit" CssClass="button" runat="server" Text="Register" OnClick="btnSubmit_Click"/>
        </div>

        <!-- error label if there's any error -->
        <div class="form-group">
            <asp:Label ID="lblMessage" runat="server" CssClass="error"></asp:Label>
        </div>
    </form>
</body>
</html>
