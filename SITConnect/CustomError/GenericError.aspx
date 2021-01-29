﻿<%@ Page Title="Page Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenericError.aspx.cs" Inherits="SITConnect.CustomError.GenericError" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="CSS/Default.css" rel="stylesheet" type="text/css" />
    <link href="Content/Standard.css" rel="stylesheet" type="text/css" />

    <div class="container-fluid" style="height: 100vh;">
        <div class="row bg-text bg-container-1" style="position: relative; top: 8vh; padding-bottom: 2px;">
            <span class="gallery-title trippin">This is Embarassing!</span>

            <h3 class="gallery-subtitle" style="text-align: left;">We can't seem to find the page you're looking for!</h3>
            <br />
            <p style="padding-left: 0px; font-family: 'JetBrains Mono', monospace;" class="navbar-brand"><b>Error Code:</b> <span class="trippin">Generic Error</span></p>
            <br />
            <br />
            <br />
            <p style="font-family: 'JetBrains Mono', monospace; font-style: oblique; font-size: medium;">We apologise for the inconvenience. It seems like you're trying to access a page that has either been removed or never existed in the first place.</p>
            <br />
            <a href="../Default" class="plan-btn"><span>Back to Home</span></a>
        </div>
    </div>

</asp:Content>