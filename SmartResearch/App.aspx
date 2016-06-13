<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster.master" AutoEventWireup="true" CodeBehind="App.aspx.cs" Inherits="SmartResearch.App" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AppHeader" runat="server">

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="AppPlaceholder" runat="server">

    <div class="row">
        <div class="col-sm-4">
            <asp:Label ID="Label1" runat="server" Text="Введите текст"></asp:Label>
            <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" Height="300px" Rows="10" TextMode="MultiLine" Width="210px"></asp:TextBox>
            <asp:Button ID="ButtonParseText" runat="server" Text="Распознать" OnClick="ButtonParseText_Click" CssClass="btn btn-default"/>
            <asp:Button ID="ButtonNextStep" runat="server" OnClick="ButtonNextStep_Click" Text="Продолжить" CssClass="btn btn-default"/>
        </div>
        <div class="col-sm-4">
            <asp:Label ID="Label2" runat="server" Text="Зависимости"></asp:Label>
            <asp:TextBox ID="OutputFromStanford" ReadOnly="true" runat="server" CssClass="form-control" Height="300px" Rows="10" TextMode="MultiLine" Width="555px"></asp:TextBox>
        </div>
    </div>

    <nav>
        <ul class="pager">
        <li class="previous disabled"><a href="#">Назад</a></li>
        <li class="next"><a href="ShowImagePage.aspx">Вперёд</a></li>
    </ul>
    </nav>
    
</asp:Content>