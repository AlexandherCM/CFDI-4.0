<%@ Page Async="true"  Language="C#" AutoEventWireup="true" CodeBehind="PageOne.aspx.cs" Inherits="Facturalo.Views.PageOne" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnGeneratePem" OnClick="btnGeneratePem_Click" runat="server" Text="Timbrar" />
        </div>
        <br />
        <div>
            <asp:Button ID="btnDeserializeXML" OnClick="btnDeserializeXML_Click" runat="server" Text="Deserialize" />
        </div>
        <br />
        <div>
            <asp:Button ID="btnPDF" OnClick="btnPDF_Click" runat="server" Text="Crear Factura" />
        </div>
       <%-- <div>
            <asp:TextBox ID="txtRFC" runat="server" placeholder="RFC"></asp:TextBox>
            <br>
            <asp:Button ID="btnSearchRFC" OnClick="btnSearchRFC_Click" runat="server" Text="Buscar" />
        </div>--%>
    </form>
</body>
</html>
