<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SmartResearch.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="jumbotron">
        <h1>Smart Research</h1>
        <p class="lead">Просто. Мощно. Уникально.</p>
        <p><a class="btn btn-lg btn-success" href="application" role="button">Начать работу</a></p>
      </div>

      <div class="row marketing">
        <div class="col-lg-6">
          <h4>Что такое Smart Research?</h4>
          <p>Это система интеллектуального анализа текста на естественном языке с дальнейшим составлением и обработкой математических моделей.</p>

          <%--<h4>Subheading</h4>
          <p>Morbi leo risus, porta ac consectetur ac, vestibulum at eros. Cras mattis consectetur purus sit amet fermentum.</p>--%>
        </div>

        <%--<div class="col-lg-6">
          <h4>Subheading</h4>
          <p>Donec id elit non mi porta gravida at eget metus. Maecenas faucibus mollis interdum.</p>
        </div>--%>
      </div>
</asp:Content>
