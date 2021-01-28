<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link href="CSS/Default.css" rel="stylesheet" type="text/css" />
    <link href="Content/Standard.css" rel="stylesheet" type="text/css" />
    <link href="CSS/Login.css" rel="stylesheet" type="text/css" />

    <script src="https://www.google.com/recaptcha/api.js?render=6LeZpuQZAAAAAEm22mJLRqqWV7bqm469FgMbyw03"></script>


    <div class="container-fluid" style="height: 100vh; width: 600px;">
        <div class="row bg-text bg-container-1" style="position: relative; top: 20vh;">
            <div class="col-md-2"></div>
            <div class="col-md-8 form-group">
                <%if (Page.Items["create_error"] != null)
                    {%>
                <div style="z-index: 1; position: absolute; width; width: 100%;">
                    <div class="alert alert-danger fade in" style="margin-right: 30px;">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>ERROR! </b><%:Page.Items["create_error"]%><span id="ce_text" style="font-weight: normal" onload="create_error('<%:Page.Items["create_error"]%>')"></span>
                    </div>
                </div>
                <%}%>
                <%if (Page.Items["create_success"] != null)
                    {%>
                <div style="z-index: 1; position: absolute; width; width: 100%;">
                    <div class="alert alert-success fade in" style="margin-right: 30px;">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>SUCCESS! </b><%:Page.Items["create_success"]%><span id="ce_text" style="font-weight: normal" onload="create_error('<%:Page.Items["create_success"]%>')"></span>
                    </div>
                </div>
                <%}%>
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
                <asp:TextBox runat="server" ID="tb_password" CssClass="text-input" placeholder="Enter password" TextMode="Password" required="required" ></asp:TextBox>
                <i class="fas fa-lock text-icon"></i>
                <%-- END OF INPUTS --%>
                <br />
                <br />
                <br />
                <%-- LOG IN BUTTON --%>
                <asp:Button ID="loginBtn" class="btn btn-long" runat="server" OnClick="loginBtn_Click" Text="Login"></asp:Button>
                <br />
                <br />
                <input type="hidden"id="g-recaptcha-response"name="g-recaptcha-response"/>
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

        function create_error(errList) {
            var list = errList.split(",");
            console.log(list.join("\n"));
            //$("#ce_text").text(list.join("\n"))
            for (var i = 0; i < list.length; i++) {
                console.log(list[i]);
                var s = document.createElement('p')
                s.innerHTML = list[i];
                document.getElementById("ce_text").appendChild(s);
            };
        }

        grecaptcha.ready(function () {
            grecaptcha.execute('6LeZpuQZAAAAAEm22mJLRqqWV7bqm469FgMbyw03', { action: 'Login' }).then(function (token) {
                document.getElementById("g-recaptcha-response").value = token;
            });
        });
    </script>
</asp:Content>
