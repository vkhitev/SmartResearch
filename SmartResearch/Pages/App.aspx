<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/AppMaster.master" AutoEventWireup="true" CodeBehind="App.aspx.cs" Inherits="SmartResearch.App" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AppHeader" runat="server">

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="AppPlaceholder" runat="server">

    <div class="row panel panel-default">
        <div class="text-center panel-body" style="padding-top:0;">
            <h3>Загрузите файл</h3>
            <h5 style="color:grey">Поддерживаемые форматы: .txt</h5>
            <asp:FileUpload ID="FileUpload" runat="server" CssClass="center-block"/>
            <asp:Button ID="ButtonUpload" runat="server" Text="Загрузить" CssClass="btn btn-default upload_btn" OnClick="ButtonUpload_Click"/>
            <br />
            <asp:Label ID="LabelUpload" runat="server" Text=""></asp:Label>
        </div>
    </div>

    <div class="row" style="margin-top:-20px;">
        <div class="text-center">
            <h3>Или введите текст вручную</h3>
			<h5 style="color: grey">Текст должен быть только на английском языке</h5>
			<h5 style="color: grey">Текст не должен содержать прямую речь и диалоги</h5>
        </div>
        <div>
            <asp:TextBox ID="TextBox" runat="server" CssClass="form-control" Height="300px" Rows="10" TextMode="MultiLine"></asp:TextBox>
            <asp:Button ID="ButtonNextStep" runat="server" OnClick="ButtonNextStep_Click" Text="Продолжить" CssClass="btn btn-default"/>
            <asp:PlaceHolder runat="server" ID="EmptyLabel"></asp:PlaceHolder>
        </div>
		<div class="example-buttons">
			<asp:Button ID="Example1" runat="server" Text="Пример 1" CssClass="btn btn-success" OnClick="Example1_Click" /> <br />
			<asp:Button ID="Example2" runat="server" Text="Пример 2" CssClass="btn btn-success" OnClick="Example2_Click" /> <br />
			<asp:Button ID="Example3" runat="server" Text="Пример 3" CssClass="btn btn-success" OnClick="Example3_Click" />
		</div>
    </div>
    
</asp:Content>