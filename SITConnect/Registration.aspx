<%@ Page Title="Registration" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" ValidateRequest="false"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Standard.css" rel="stylesheet" type="text/css" />
    <link href="CSS/Login.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript" src="https://cdn.jsdelivr.net/momentjs/latest/moment.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css" />

    <div class="container-fluid" style="height: 100vh; width: 600px;">
        <div class="row bg-text bg-container-1" style="position: relative; top: 8vh; margin-left: -100px; margin-right: -100px; padding-bottom: 2px;">

            <div class="col-md-1">
            </div>
            <div class="col-md-10 form-group" style="">
                <%if (Page.Items["create_error"] != null)
                    {%>
                <div style="z-index: 1; position: absolute; width; width: 100%;">
                    <div class="alert alert-danger fade in" style=" margin-right: 30px;">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>ERROR! </b><span style="font-weight: normal"><%:Page.Items["create_error"]%></span><span id="ce_text" style="font-weight:normal" onload="create_error('<%:Page.Items["create_error"]%>')"></span>
                    </div>
                </div>
                <%}%>
                <%if (Page.Items["create_success"] != null)
                    {%>
                <div style="z-index: 1; position: absolute; width; width: 100%;">
                    <div class="alert alert-success fade in" style=" margin-right: 30px;">
                        <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        <b>SUCCESS! </b><span style="font-weight: normal"><%:Page.Items["create_success"]%></span><span id="ce_text" style="font-weight:normal" onload="create_error('<%:Page.Items["create_success"]%>')"></span>
                    </div>
                </div>
                <%}%>
                <br />
                <h1 class="gallery-title" style="color: black; text-shadow: 2px 2px #ff0000;">REGISTER</h1>
                <br />

                <div class="col-md-6" style="padding-left: 0px;">
                    <%-- FIRSTNAME INPUT --%>
                    <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="fnameTB">First Name</asp:Label>
                    <asp:TextBox runat="server" ID="fnameTB" CssClass="text-input" placeholder="Enter firstname" TextMode="SingleLine" required="required"> </asp:TextBox>
                    <i style="margin-left: -13px;" class="fas fa-user text-icon"></i>
                    <br />
                    <br />
                </div>
                <div class="col-md-6" style="padding-right: 0px;">
                    <%-- LASTNAME INPUT --%>
                    <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="lnameTB">Last Name</asp:Label>
                    <asp:TextBox runat="server" ID="lnameTB" CssClass="text-input" placeholder="Enter lastname" TextMode="SingleLine" required="required"></asp:TextBox>
                    <i class="fas fa-envelope text-icon"></i>
                    <br />
                    <br />
                </div>
                <asp:Label runat="server" class="text-label text-label-half" AssociatedControlID="dobTB">Date of Birth</asp:Label>
                <asp:TextBox runat="server" Cssclass="text-input" type="text" ID="dobTB" placeholder="Enter birthdate" required="required"></asp:TextBox>
                <i class="fas fa-calendar text-icon"></i>
                <br />
                <br />
                <%-- EMAIL INPUT --%>
                <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="emailTB">Email Address</asp:Label>
                <asp:TextBox runat="server" ID="emailTB" CssClass="text-input" placeholder="Enter email address" TextMode="Email" pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$"></asp:TextBox>
                <i class="fas fa-envelope text-icon"></i>
                <br />
                <br />
                <%-- PASSSWORD INPUT --%>
                <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="pwdTB">Password</asp:Label>
                <asp:TextBox runat="server" data-toggle="tooltip" data-html="true" data-placement="bottom" ID="pwdTB" CssClass="text-input" placeholder="Enter password" TextMode="Password" onkeyup="javascript:validatePwd()"></asp:TextBox>
                <i class="fas fa-lock text-icon"></i>
                <br />
                <br />
                <%-- CONFIRM PASSSWORD INPUT --%>
                <asp:Label CssClass="text-label black" runat="server" AssociatedControlID="conpwdTB">Confirm Password</asp:Label>
                <asp:TextBox runat="server" data-toggle="tooltip" data-html="true" data-placement="bottom" ID="conpwdTB" CssClass="text-input" placeholder="Enter password again" TextMode="Password" onkeyup="javascript:validateConPwd()"></asp:TextBox>
                <i class="fas fa-lock text-icon"></i>
                <br />
                <br />
                <%-- CREDITCARD INPUT --%>
                <asp:Label runat="server" class="text-label" AssociatedControlID="ccardTB">Card Number (VISA Only)</asp:Label><br />
                <asp:TextBox runat="server" CssClass="text-input" data-toggle="tooltip" data-html="true" data-placement="bottom" type="text" ID="ccardTB" placeholder="Enter creditcard number" MaxLength="19" pattern="[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}" onkeyup="javascript:validateCCard()" required="required" />
                <i class="fas fa-credit-card text-icon"></i>

                <%-- END OF INPUTS --%>
                <br />
                <br />
                <br />
                <%-- REGISTER BUTTON --%>
                <asp:Button ID="regBtn" class="btn btn-long" runat="server" OnClick="regBtn_Click" Text="Register"></asp:Button>
                <asp:Label runat="server" ID="lblMsg"></asp:Label>
                <br />
                <br />
                <a href="/Login.aspx" class="btn-signup">Already have an account? Sign in</a>
            </div>
            <div class="col-md-1"></div>
        </div>
    </div>



    <script>
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('#<%=regBtn.ClientID%>').prop('disabled', true)

            $('input[id="<%=dobTB.ClientID%>"]').daterangepicker({
                opens: 'right',
                drops: 'down',
                autoApply: true,
                singleDatePicker: true,
                showDropdowns: true,
                maxDate: new Date(),
                locale: {
                    format: 'DD/MM/YYYY'
                }
            }, function (start, end, label) {
            });
            window.onscroll = function () {
                $('input[id="<%=dobTB.ClientID%>"]').data('daterangepicker').hide();
                document.getElementById("<%=dobTB.ClientID%>").blur();
            }
        });

        $(document).keypress(function (event) {
            $('#<%=regBtn.ClientID%>').prop('disabled', true)
            var isFilled = true;
            
            $(".text-input").map(function () {
                console.log($(this).val().length)
                if ($(this).val().length == 0) {
                    isFilled = false;
                }
                console.log(isFilled)
            })

            if (isFilled) {
                $('#<%=regBtn.ClientID%>').prop('disabled', false)
            }
        })

        function create_error(errList) {
            var list = errList.split(",");
            console.log(list.join("\n"));
            for (var i = 0; i < list.length; i++) {
                console.log(list[i]);
                var s = document.createElement('p')
                s.innerHTML = list[i];
                document.getElementById("ce_text").appendChild(s);
            };
        }

        function validatePwd() {
            var pwd = document.getElementById('<%=pwdTB.ClientID%>').value;
            var message = "";

            if (pwd.length < 8) {
                document.getElementById('<%=pwdTB.ClientID%>').setCustomValidity("Password length must be at least 8 Characters");
                message += "8 characters<br>";
            }
            if (pwd.search(/[0-9]/) == -1) {
                document.getElementById('<%=pwdTB.ClientID%>').setCustomValidity("Password require at least 1 number");
                message += "1 number<br>";
            }
            if (pwd.search(/[a-z]/) == -1) {
                document.getElementById('<%=pwdTB.ClientID%>').setCustomValidity("Password require at least 1 lowercase");
                message += "1 lowercase<br>";
            }
            if (pwd.search(/[A-Z]/) == -1) {
                document.getElementById('<%=pwdTB.ClientID%>').setCustomValidity("Password require at least 1 uppercase");
                message += "1 uppercase<br>";
            }
            if (/^[a-zA-Z0-9-,_]*$/.test(pwd) == 1) {
                document.getElementById('<%=pwdTB.ClientID%>').setCustomValidity("Password require at least 1 special character");
                message += "1 specialchar<br>";
            }

            if (message.length > 1) {
                $("#<%=pwdTB.ClientID%>").attr('data-original-title', "<b>Password Must Have</b><br>" + message)
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                document.getElementById('<%=pwdTB.ClientID%>').setCustomValidity("");
                $("#<%=pwdTB.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }
            if (document.getElementById("<%=conpwdTB.ClientID%>").value.length > 0) {
                validateConPwd();
            }
        }

        function validateConPwd() {
            var pwd = document.getElementById('<%=pwdTB.ClientID%>').value;
            var conPwd = document.getElementById('<%=conpwdTB.ClientID%>').value;
            var message = ""

            if (pwd != conPwd) {
                document.getElementById('<%=conpwdTB.ClientID%>').setCustomValidity("Passwords do not match.");
                message += "Passwords do not match!";
            }

            if (message.length > 1 && conPwd.length > 0) {
                $("#<%=conpwdTB.ClientID%>").attr('data-original-title', "<b>" + message + "<br>")
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                document.getElementById('<%=conpwdTB.ClientID%>').setCustomValidity("");
                $("#<%=conpwdTB.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }

        }

        $('#<%=ccardTB.ClientID%>').keypress(function () {
            var rawNos = $(this).val().replace(/-/g, '');
            var lengthccard = rawNos.length;
            if (lengthccard !== 0 && lengthccard <= 12 && lengthccard % 4 == 0) {
                $(this).val($(this).val() + '-');
            }
            var str = $(this).val().replace(/-/g, "");
            console.log(str);

            validateCCard();
        });

        function validateCCard() {
            var ccard = document.getElementById('<%=ccardTB%>').value;
            var message = ""

            if (ccard.search(/^4[0-9]{12}(?:[0-9]{3})?$/) == -1) {
                document.getElementById('<%=ccardTB.ClientID%>').setCustomValidity("VISA Credit Card is not valid.");
                message += "VISA Credit Card is not valid.";
            }

            if (message.length > 1) {
                $("#<%=ccardTB.ClientID%>").attr('data-original-title', "<b>" + message + "<br>")
                    .tooltip('fixTitle')
                    .tooltip('show')
                    .unbind();
            }
            else {
                document.getElementById('<%=ccardTB.ClientID%>').setCustomValidity("");
                $("#<%=ccardTB.ClientID%>").attr('data-original-title', "")
                    .tooltip('fixTitle')
                    .tooltip('hide');
            }
        }
    </script>

</asp:Content>
