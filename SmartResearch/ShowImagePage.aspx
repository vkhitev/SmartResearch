<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster.master" AutoEventWireup="true" CodeBehind="ShowImagePage.aspx.cs" Inherits="SmartResearch.ShowImagePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AppPlaceholder" runat="server">
    <nav>
      <ul class="pager">
        <li class="previous"><a href="App.aspx">Назад</a></li>
        <li class="next"><a href="#">Вперёд</a></li>
      </ul>
        <asp:Label ID="Label1" runat="server" Text="Семантическая сеть"></asp:Label>
        <br />
        <asp:Image ID="Image1" runat="server" ImageUrl="image2.png" Width="1000px" BorderStyle="Groove" ImageAlign="Middle" />

    </nav>
</asp:Content>
