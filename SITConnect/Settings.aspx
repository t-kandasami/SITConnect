<%@ Page Title="Settings" Language="C#" MasterPageFile="~/Portal.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="SITConnect.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/Standard.css" rel="stylesheet" type="text/css" />

    <%-- UPDATE PASSWORD MODAL --%>
    <div class="modal fade" id="modalChangePassword" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content" id="modal-content" style="margin-top: -40px;">
                <div class="modal-header">
                    <h5 class="modal-title">Change Password</h5>
                </div>
                <div class="modal-body">
                    <%if (Page.Items["create_error"] != null)
                        {%>
                    <div style="z-index: 1; position: absolute; width: 100%;">
                        <div class="alert alert-error fade in" style="margin-top: -80px; margin-right: 30px;">
                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                            <b>ERROR! </b></br><p style="font-weight: normal;">Login is invalid</p>
                        </div>
                    </div>
                    <%}%>
                    <%if (Page.Items["create_success"] != null)
                        {%>
                    <div style="z-index: 1; position: absolute; width; width: 100%;">
                        <div class="alert alert-success fade in" style="margin-top: -80px; margin-right: 30px;">
                            <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                            <b>SUCCESS! </b></br><p style="font-weight: normal;">Account is Created</p>
                        </div>
                    </div>
                    <%}%>
                    <div class="form-group">
                        <!--OLD PASSWORD-->
                        <asp:Label CssClass="label" runat="server">Old Password</asp:Label>
                        <i class="fas fa-lock form-icon"></i>
                        <asp:TextBox runat="server" ID="oldPwdTB" CssClass="form-input" TextMode="Password" placeholder="Enter old password"></asp:TextBox>
                    </div>
                    <br />
                    <div class="form-group">
                        <!--NEW PASSWORD-->
                        <asp:Label CssClass="label" runat="server">New Password</asp:Label>
                        <i class="fas fa-lock form-icon"></i>
                        <asp:TextBox runat="server" data-toggle="tooltip" data-html="true" data-placement="left" ID="newPwdTB" CssClass="form-input" TextMode="Password" placeholder="Enter new password" onkeyup="javascript:validatePwd()"></asp:TextBox>
                    </div>
                    <br />
                    <div class="form-group">
                        <!--CONFIRM PASSWORD-->
                        <asp:Label CssClass="label" runat="server">Confirm Password</asp:Label>
                        <i class="fas fa-lock form-icon"></i>
                        <asp:TextBox runat="server" data-toggle="tooltip" data-html="true" data-placement="left" ID="confirmPwdTB" CssClass="form-input" TextMode="Password" placeholder="Enter new password again" onkeyup="javascript:validateConPwd()"></asp:TextBox>
                    </div>
                    <p id="pwdMsg" style="color: red;"></p>
                </div>
            <div class="modal-footer">
                <asp:Button runat="server" CssClass="btn btn-start" ID="passwordSaveBtn" Text="Save Changes" />
            </div>
        </div>
    </div>
    </div>

    <div class="profile-gallery">
        <img src="../Media/pencilHolder.jpg" />
    </div>

    <div class="profile-user" style="height: 740px;">
        <div class="user-image">
            <img id="profileImg" src="../Media/DefaultAvatar.png" />
        </div>
        <div class="user-extras">
            <div class="form-group">
                <a class="btn btn-rect-icon" href="#" data-toggle="modal" data-target="#modalChangePassword"><i class="fas fa-lock"></i><span>&nbsp;Change Password</span></a>
            </div>
        </div>
        <div class="user-info">
            <h1 class="user-name"><span class="trippin">
                <asp:Label runat="server" ID="lbl_username"></asp:Label></span></h1>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('#<%=passwordSaveBtn.ClientID%>').prop('disabled', true)
        });



        $(document).keypress(function (event) {
            $('#<%=passwordSaveBtn.ClientID%>').prop('disabled', true)
            // boolean variable
            var isFilled = true;
            // map through all textboxes
            $(".form-input").map(function () {
                console.log($(this).val().length)
                // if length is 0, means empty, not all textboxes are filled
                if ($(this).val().length == 0) {
                    isFilled = false;
                }
                console.log(isFilled)
            })

            // if all textboxes are filled, enable the button
            if (isFilled) {
                $('#<%=passwordSaveBtn.ClientID%>').prop('disabled', false)
            }
        })

        function validatePwd() {
            var pwd = document.getElementById('<%=newPwdTB.ClientID%>').value;
            var message = "";

            if (pwd.length < 8) {
                document.getElementById('<%=newPwdTB.ClientID%>').setCustomValidity("Password length must be at least 8 Characters");
                message += "8 characters<br>";
            }
            if (pwd.search(/[0-9]/) == -1) {
                document.getElementById('<%=newPwdTB.ClientID%>').setCustomValidity("Password require at least 1 number");
                message += "1 number<br>";
            }
            if (pwd.search(/[a-z]/) == -1) {
                document.getElementById('<%=newPwdTB.ClientID%>').setCustomValidity("Password require at least 1 lowercase");
                message += "1 lowercase<br>";
            }
            if (pwd.search(/[A-Z]/) == -1) {
                document.getElementById('<%=newPwdTB.ClientID%>').setCustomValidity("Password require at least 1 uppercase");
                message += "1 uppercase<br>";
            }
            if (/^[a-zA-Z0-9-,_]*$/.test(pwd) == 1) {
                document.getElementById('<%=newPwdTB.ClientID%>').setCustomValidity("Password require at least 1 special character");
                message += "1 specialchar<br>";
            }

            if (message.length > 1) {
                $("#<%=newPwdTB.ClientID%>").attr('data-original-title', "<b>Password Must Have</b><br>" + message)
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                document.getElementById('<%=newPwdTB.ClientID%>').setCustomValidity("");
                $("#<%=newPwdTB.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }
            if (document.getElementById("<%=confirmPwdTB.ClientID%>").value.length > 0) {
                validateConPwd();
            }
        }

        function validateConPwd() {
            var pwd = document.getElementById('<%=newPwdTB.ClientID%>').value;
            var conPwd = document.getElementById('<%=confirmPwdTB.ClientID%>').value;
            var message = ""

            if (pwd != conPwd) {
                document.getElementById('<%=confirmPwdTB.ClientID%>').setCustomValidity("Passwords do not match.");
                message += "Passwords do not match!";
            }

            if (message.length > 1 && conPwd.length > 0) {
                $("#<%=confirmPwdTB.ClientID%>").attr('data-original-title', "<b>" + message + "<br>")
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                document.getElementById('<%=confirmPwdTB.ClientID%>').setCustomValidity("");
                $("#<%=confirmPwdTB.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }

        }
    </script>
</asp:Content>
