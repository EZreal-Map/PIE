<%@ Page Language="C#" validateRequest="false"%>
<%
    string filename = Request.Form["filename"];
    Response.AddHeader("Content-disposition", "attachment; filename="+filename+".xls");
    Response.Write(Request.Form["data"]);
%>

