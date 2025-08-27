<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StaffLogin.aspx.cs" Inherits="AITResearch.StaffLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Staff Login</title>
        <link href="StyleSheet1.css" rel="stylesheet"/>
    </head>
    <body>
        <!-- top navigation bar -->
        <div class="topnav">
            <a href="index.aspx">Home</a>
            <a href="StaffLogin.aspx" class="active">Staff Login</a>
        </div>

        <div style="width: 100%;flex: 1;display: flex;justify-content: center;align-items: center;x">
            <form runat="server" style="width: 100%;min-width: 750px;max-width: 900px;">
                <div class="container-account">
                    <!-- Staff Login -->
                    <div class="form-box">
                        <h1>Staff Login</h1>

                        <div class="form-group">
                            <asp:Label ID="lblUsername" runat="server" Text="Username"></asp:Label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="textbox"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="textbox"></asp:TextBox>
                        </div>

                        <div class="button-center">
                            <asp:Button ID="btnLogin" runat="server" CssClass="button" Text="Login" OnClick="btnLogin_Click" />
                        </div>

                        <asp:Label ID="lblError" runat="server" CssClass="error" ForeColor="Red"></asp:Label>
                    </div>

                    <!-- Create Staff Account -->
                    <div class="form-box">
                        <h1>Create Account</h1>

                        <div class="form-group">
                            <asp:Label ID="lblNewUsername" runat="server" Text="Username"></asp:Label>
                            <asp:TextBox ID="txtNewUsername" runat="server" CssClass="textbox"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lblNewPassword" runat="server" Text="Password"></asp:Label>
                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" CssClass="textbox"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lblConfirmPassword" runat="server" Text="Confirm Password"></asp:Label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="textbox"></asp:TextBox>
                        </div>

                        <div class="button-center">
                            <asp:Button ID="btnCreateAccount" runat="server" CssClass="button" Text="Create Account" OnClick="btnCreateAccount_Click"/>
                        </div>

                        <asp:Label ID="lblCreateError" runat="server" CssClass="error" ForeColor="Red"></asp:Label>
                    </div>
                </div>
            </form>
        </div>
    </body>
</html>
