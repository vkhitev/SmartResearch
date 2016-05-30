<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster.master" AutoEventWireup="true" CodeBehind="App.aspx.cs" Inherits="SmartResearch.App" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AppPlaceholder" runat="server">

    <nav>
      <ul class="pager">
        <li class="previous disabled"><a href="#">Назад</a></li>
        <li class="next"><a href="ShowImagePage.aspx">Вперёд</a></li>
      </ul>
        <asp:Label ID="Label1" runat="server" Text="Введите информацию"></asp:Label>
        <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" Height="300px" Rows="10" TextMode="MultiLine" Width="200px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Продолжить" />
    </nav>
</asp:Content>