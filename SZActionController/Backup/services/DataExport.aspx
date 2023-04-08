<%@ Page Language="C#" validateRequest="false"%>
<%
Response.AddHeader("Content-disposition", "attachment; filename=Plan.PBF");
Response.Write(Request.Form["htmltable"]);
%>

