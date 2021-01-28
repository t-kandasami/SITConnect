<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="CSS/Default.css" rel="stylesheet" type="text/css" />
    <link href="Content/Standard.css" rel="stylesheet" type="text/css" />
    <link href="CSS/Login.css" rel="stylesheet" type="text/css" />


    <div class="container-fluid" style="height: 100vh; width: 600px;">
        <%if (Page.Items["create_error"] != null)
            {%>
        <div style="z-index: 1; position: absolute; width; width: 100%;">
            <div class="alert alert-danger fade in">
                <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <b>ERROR! </b></br><p style="font-weight: normal;">Login is invalid</p>
            </div>
        </div>
        <%}%>
        <%if (Page.Items["create_success"] != null)
            {%>
        <div style="z-index: 1; position: absolute; width; width: 100%;">
            <div class="alert alert-success fade in">
                <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                <b>SUCCESS! </b></br><p style="font-weight: normal;">Account is Created</p>
            </div>
        </div>
        <%}%>

        <div class="row bg-text bg-container-1" style="position: relative; top: 20vh;">
            <div class="col-md-2"></div>
            <div class="col-md-8 form-group">
                <br />
                <h1 class="gallery-title" style="color: black; text-shadow: 2px 2px #ff0000;">LOGIN</h1>
                <br />
                <%-- EMAIL INPUT --%>
                <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="tb_email">Email address</asp:Label>
                <asp:TextBox runat="server" ID="tb_email" CssClass="text-input" placeholder="Enter email address" TextMode="Email" required="required"></asp:TextBox>
                <i class="fas fa-envelope text-icon"></i>
                <br />
                <br />
                <%-- PASSSWORD INPUT --%>
                <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="tb_password">Password</asp:Label>
                <asp:TextBox runat="server" ID="tb_password" CssClass="text-input" placeholder="Enter password" TextMode="Password" required="required" pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$"></asp:TextBox>
                <i class="fas fa-lock text-icon"></i>
                <%-- END OF INPUTS --%>
                <br />
                <br />
                <br />
                <%-- LOG IN BUTTON --%>
                <asp:Button ID="loginBtn" class="btn btn-long" runat="server" Text="Login">
                    
                </asp:Button>
                <br />
                <br />
                <asp:Label runat="server" ID="lblMsg"></asp:Label>
                <a href="/Registration.aspx" class="btn-signup">Not on SITConnect? Sign up here</a>
            </div>
            <div class="col-md-2"></div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('#<%=loginBtn.ClientID%>').prop('disabled', true)
        });

        $(document).keypress(function (event) {
            $('#<%=loginBtn.ClientID%>').prop('disabled', true)
            // boolean variable
            var isFilled = true;
            // map through all textboxes
            $(".text-input").map(function () {
                console.log($(this).val().length)
                // if length is 0, means empty, not all textboxes are filled
                if ($(this).val().length == 0) {
                    isFilled = false;
                }
                console.log(isFilled)
            })

            // if all textboxes are filled, enable the button
            if (isFilled) {
                $('#<%=loginBtn.ClientID%>').prop('disabled', false)
            }
        })
    </script>
</asp:Content>
