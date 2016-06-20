<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/AppMaster.master" AutoEventWireup="true" CodeBehind="AnalysisResults.aspx.cs" Inherits="SmartResearch.AnalysisResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="AppHeader" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="AppPlaceholder" runat="server">
    <div class="container">
        <div class="btn-group">
            <asp:Button ID="ButtonShowGraph" runat="server" Text="Просмотреть граф семантической сети" CssClass="btn btn-primary" OnClick="ButtonShowGraph_Click"/>
            <asp:Button ID="ButtonFindDefinition" runat="server" Text="Найти определение слова" CssClass="btn btn-primary disabled" OnClick="ButtonFindDefinition_Click"/>
            <asp:Button ID="ButtonChangeDependencies" runat="server" Text="Исправить зависимости" CssClass="btn btn-primary disabled" OnClick="ButtonChangeDependencies_Click"/>

            <div>
                <asp:PlaceHolder ID="ResultsPlaceholder" runat="server">
                </asp:PlaceHolder>
            </div>
			<asp:GridView ID="GridView1" runat="server" DataSourceID="SqlDataSource1" AutoGenerateColumns="False" CssClass="table table-bordered" DataKeyNames="ID">
				<Columns>
					<%--<asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" SortExpression="ID" />--%>
					<asp:BoundField DataField="Expresion" HeaderText="Expresion" SortExpression="Expresion" />
					<asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
				</Columns>
			</asp:GridView>

			<asp:SqlDataSource ID="SqlDataSource1"
				runat="server"
				ConnectionString="<%$ ConnectionStrings:KnowledgeBaseConnectionString %>"
				OldValuesParameterFormatString="original_{0}"
				SelectCommand="SELECT * FROM [Axioms]">
			</asp:SqlDataSource>
		</div>
    </div>
</asp:Content>
