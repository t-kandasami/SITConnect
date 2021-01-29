<%@ Page Title="Page Error 500" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenericError.aspx.cs" Inherits="SITConnect.CustomError.Http500" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="CSS/Default.css" rel="stylesheet" type="text/css" />
    <link href="Content/Standard.css" rel="stylesheet" type="text/css" />

    <div class="container-fluid" style="height: 100vh;">
        <div class="row bg-text bg-container-1" style="position: relative; top: 8vh; padding-bottom: 2px;">
            <span class="gallery-title trippin">Webservice currently unavailable!</span>

            <h3 class="gallery-subtitle" style="text-align: left;">We can't seem to find the page you're looking for!</h3>
            <br />
            <p style="padding-left: 0px; font-family: 'JetBrains Mono', monospace;" class="navbar-brand"><b>Error Code:</b> <span class="trippin">500</span></p>
            <br />
            <br />
            <br />
            <p style="font-family: 'JetBrains Mono', monospace; font-style: oblique; font-size: medium;">We apologise for the inconvenience. An unexpected condition was encountered.<br>Our service team has been dispatched to bring it back online.</p>
            <br />
            <a href="../Default" class="plan-btn"><span>Back to Home</span></a>
        </div>
    </div>

</asp:Content>
