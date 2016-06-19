<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="Contacts.aspx.cs" Inherits="SmartResearch.Contacts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h4>Контакты:</h4>
    
    <strong>Хитёв Владислав</strong>
    <ul>
        <li>Тел. (067) 882-47-71</li>
        <li>email: vkhitev@gmail.com</li>
    </ul>

    <strong>Евгений Сердюк</strong>
    <ul>
        <li>Тел. (063) 476-91-67</li>
        <li>email: zheka2797@gmail.com</li>
    </ul>

    <br />
    <strong>Обратная связь</strong>
    <br />

    <%--Message to: <asp:TextBox ID="txtTo" CssClass="form-control" runat="server" Width="300px" /><br>--%>
    Отправитель: <asp:TextBox ID="txtFrom" CssClass="form-control" runat="server" Width="300px" /><br>
    Тема: <asp:TextBox ID="txtSubject" CssClass="form-control" runat="server" Width="300px" /><br>
    Тело Сообщения:<br/>
    <asp:TextBox ID="txtBody" CssClass="form-control" runat="server" Height="171px" TextMode="MultiLine"  Width="300px" /><br>
    <asp:Button ID="SendMail" runat="server" Text="Отправить" CssClass="btn btn-default" OnClick="SendMail_Click" />

</asp:Content>
