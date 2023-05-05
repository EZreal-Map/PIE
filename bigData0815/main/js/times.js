function showLocale(objD){
	var str,colorhead,colorfoot;
	var yy = objD.getYear();
	if(yy<1900) yy = yy+1900;
	var MM = objD.getMonth()+1;
	if(MM<10) MM = '0' + MM;
	var dd = objD.getDate();
	if(dd<10) dd = '0' + dd;
	var hh = objD.getHours();
	if(hh<10) hh = '0' + hh;
	var mm = objD.getMinutes();
	if(mm<10) mm = '0' + mm;
	var ss = objD.getSeconds();
	if(ss<10) ss = '0' + ss;
	var ww = objD.getDay();
	if  ( ww==0 )  colorhead="<font color=\"#2FEDA5\">";
	if  ( ww > 0 && ww < 6 )  colorhead="<font color=\"#2FEDA5\">";
	if  ( ww==6 )  colorhead="<font color=\"#2FEDA5\">";
	if  (ww==0)  ww="星期日";
	if  (ww==1)  ww="星期一";
	if  (ww==2)  ww="星期二";
	if  (ww==3)  ww="星期三";
	if  (ww==4)  ww=" 星期四";
	if  (ww==5)  ww=" 星期五";
	if  (ww==6)  ww=" 星期六";
	colorfoot="</font>"
	str = colorhead + yy + "-" + MM + "-" + dd + " <br/>&nbsp;" + ww + colorfoot;
	//str = colorhead + hh + ":" + mm + ":" + ss + "  " + colorfoot;
	return(str);
}

function showLocale2(objD){
	var str,colorhead,colorfoot;
	var yy = objD.getYear();
	if(yy<1900) yy = yy+1900;
	var MM = objD.getMonth()+1;
	if(MM<10) MM = '0' + MM;
	var dd = objD.getDate();
	if(dd<10) dd = '0' + dd;
	var hh = objD.getHours();
	if(hh<10) hh = '0' + hh;
	var mm = objD.getMinutes();
	if(mm<10) mm = '0' + mm;
	var ss = objD.getSeconds();
	if(ss<10) ss = '0' + ss;
	colorhead="<font color=\"#2FEDA5\">";

	colorfoot="</font>"
	str = colorhead + hh + ":" + mm + ":" + ss + "  " + colorfoot;
	return(str);
}

function tick(){
var today;
today = new Date();
// today = new Date(new Date().getTime() + 24 * 60 * 60 * 1000 * 3);
// today.setFullYear('2019');
document.getElementById("localtime").innerHTML = showLocale(today);
document.getElementById("localtime2").innerHTML = showLocale2(today);
window.setTimeout("tick()", 1000);
}

tick();