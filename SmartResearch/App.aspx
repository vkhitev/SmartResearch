<%@ Page Title="" Language="C#" MasterPageFile="~/AppMaster.master" AutoEventWireup="true" CodeBehind="App.aspx.cs" Inherits="SmartResearch.App" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AppHeader" runat="server">

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="AppPlaceholder" runat="server">

    <div class="row panel panel-default">
        <div class="text-center panel-body" style="padding-top:0;">
            <h3>Загрузите файл</h3>
            <h5 style="color:grey">Поддерживаемые форматы: txt, doc, docx</h5>
            <asp:FileUpload ID="FileUpload" runat="server" CssClass="center-block"/>
            <asp:Button ID="ButtonUpload" runat="server" Text="Загрузить" CssClass="btn btn-default upload_btn" OnClick="ButtonUpload_Click"/>
            <br />
            <asp:Label ID="LabelUpload" runat="server" Text=""></asp:Label>
            <%--<asp:Button ID = "ButtonFileContinue" runat = "server" Text = "Продолжить" CssClass = "btn btn-default upload_btn" OnClick = "ButtonFileContinue_Click" />--%>
        </div>
    </div>

    <div class="row" style="margin-top:-20px;">
        <div class="text-center">
            <h3>Или введите текст вручную</h3>
            <h5 style="color:grey">Текст не должен содержать прямую речь и диалоги</h5>
        </div>
        <div>
            <%--<asp:Label ID="Label1" runat="server" Text="Поле для ввода текста"></asp:Label>--%>
            <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" Height="300px" Rows="10" TextMode="MultiLine"></asp:TextBox>
            <%--<asp:Button ID="ButtonParseText" runat="server" Text="Распознать" OnClick="ButtonParseText_Click" CssClass="btn btn-default"/>--%>
            <asp:Button ID="ButtonNextStep" runat="server" OnClick="ButtonNextStep_Click" Text="Продолжить" CssClass="btn btn-default"/>
            <asp:PlaceHolder runat="server" ID="EmptyLabel"></asp:PlaceHolder>
        </div>
        <%--<div class="col-sm-4">
            <asp:Label ID="Label2" runat="server" Text="Зависимости между словами"></asp:Label>
            <asp:TextBox ID="OutputFromStanford" ReadOnly="true" runat="server" CssClass="form-control" Height="300px" Rows="10" TextMode="MultiLine" Width="555px"></asp:TextBox>
        </div>--%>
    </div>

    <%--<div class="row">
        <nav class="text-center">
            <ul class="pagination" >
                <li class="previous disabled"><a href="#">Назад</a></li>
                <li class="next disabled"><a href="ShowImagePage.aspx">Вперёд</a></li>
            </ul>
        </nav>
    </div>--%>
    
</asp:Content>