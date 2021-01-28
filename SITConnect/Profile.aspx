<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Portal.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="SITConnect.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="profile-gallery">
        <img src="../Media/pencilHolder.jpg" />
    </div>

    <div class="profile-user" style="height: 740px;">
        <div class="user-image">
            <img id="profileImg" src="../Media/DefaultAvatar.png" />
        </div>
        <div class="user-extras">
            <a runat="server" class="btn btn-rect-icon" href="#">
                <i class="fas fa-sign-out-alt"></i><span>&nbsp;Sign Out</span>
            </a>
        </div>
        <div class="user-info">
            <h1 class="user-name">
                <span class="trippin"><asp:Label runat="server" ID="lbl_username">ABC</asp:Label></span>
            </h1>
        </div>
        <div class="user-stuff">
            <h1><span class="trippin">Basic Information</span></h1>
            <table>
                <tr>
                    <td class="row-title">First Name</td>
                    <td class="row-info">
                        <asp:Label runat="server" ID="lbl_fname"></asp:Label></td>
                </tr>
                <tr>
                    <td class="row-title">Last Name</td>
                    <td class="row-info">
                        <asp:Label runat="server" ID="lbl_lname"></asp:Label></td>
                </tr>
                <tr>
                    <td class="row-title">Date of Birth</td>
                    <td class="row-info">
                        <asp:Label runat="server" ID="lbl_dob"></asp:Label></td>
                </tr>
                <tr>
                    <td class="row-title">Email Address</td>
                    <td class="row-info">
                        <asp:Label runat="server" ID="lbl_email"></asp:Label></td>
                </tr>
                <tr>
                    <td class="row-title">Credit Card Number</td>
                    <td class="row-info">
                        <asp:Label runat="server" ID="lbl_ccard"></asp:Label></td>
                </tr>

            </table>
        </div>
    </div>

</asp:Content>
