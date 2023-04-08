<%@ Page Language="C#" validateRequest="false" %>
<%
    string method = Request.QueryString["method"];
    string name = Request.QueryString["name"];

    byte[] data = Request.BinaryRead(Request.TotalBytes);

    Response.ContentType = "application/pdf;charset=UTF-8";
    Response.AddHeader("Content-Length", data.Length.ToString());
    Response.AddHeader("Content-disposition", method + "; filename=" + name);
    Response.BinaryWrite(data);
    Response.End();
%>