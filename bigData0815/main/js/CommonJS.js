/**
 * Created with JetBrains WebStorm.
 * User: asus
 * Date: 13-9-18
 * Time: 下午7:32
 * To change this template use File | Settings | File Templates.
 */

//获取Object对象的属性数目
function getPropertyCount(obj){
    var n, count = 0;
    for(n in obj){
        if(obj.hasOwnProperty(n)){
            count++;
        }
    }
    return count;
}
//获取Node的整个名称
function getNodeFullName(node){
    var tempName = node.data.text;
   while(node.getDepth()>1){
       node = node.parentNode;
        tempName = node.data.text+';'+tempName;
    }
    return tempName;
}
//获取Node的整个名称
function getNodeFullName(node,splitChar){
    var tempName = node.data.text;
    if(splitChar==undefined)
    splitChar=';';
    while(node.getDepth()>1){
        node = node.parentNode;
        tempName = node.data.text+splitChar+tempName;
    }
    return tempName;
}
//获取Object对象的属性
function getObjectAttirbuteNames(obj){
    var result = new Array();
    if(obj!=undefined){
        for(i in obj)
            result.push(i);
    }
        return result;
}

//获取Object对象的第index个属性值
function getObjectValue(obj,index){
    if(obj!=undefined){
        var j=0;
        for(var i in obj) {
            if(j==index)
                return obj[i];
            j++;
        }
    }
}

//转换Ext.data.Model或Object对象的属性及相应属性值为字符串内容
function convertObjectInfo(model,fieldsName){
    var len = fieldsName.length;
    if(len>0){
        var result = '';
        for(var i=0;i<len;i++)
            result+=fieldsName[i]+':'+model.get(fieldsName[i])+'    ';
        return result;
    }
}
//转换Ext.data.Model或Object对象的属性及相应属性值为字符串内容
function convertObjectInfo2(model,fieldsName,size,splitChar){
    var len = fieldsName.length;
    if(len>0){
        len = size>len?len:size;
        var result = '';
        for(var i=0;i<len;i++)
            result+=fieldsName[i]+':'+model.get(fieldsName[i])+splitChar;
        return result;
    }
}

//从对象数组中根据属性名称提取数组1
function extractArrayInObject(objArray,propName){
    var result = [];
    if(objArray!=undefined){
        for(var i= 0,len = objArray.length;i<len;i++){
            var tempObj = objArray[i];
            result.push(tempObj[propName]);
        }
    }
    return result;
}
//从对象数组中根据属性名称提取数组2
function extractArrayInObjectTimeData(objArray,propName){
    var result = [];
    if(objArray!=undefined){
        for(var i= 0,len = objArray.length;i<len;i++){
            var tempObj = objArray[i].data;
            if(tempObj[propName] != undefined)
            {
                result.push(tempObj[propName]);
            }
        }
    }
    return result;
}

//从对象数组中根据属性名称提取数组3
function extractArrayInObjectData(objArray,propName){
    var result = [];
    if(objArray!=undefined){
        for(var i= 0,len = objArray.length;i<len-1;i++){
            var tempObj = objArray[i].data;
            if(tempObj[propName] != undefined)
            {
                result.push(tempObj[propName]);
            }else{
                result.push(null);
            }
        }
    }
    return result;
}

//复制Store数据
function deepCloneStore (source) {
    //alert('1');
    var target = Ext.create ('Ext.data.Store', {
        model: source.model
    });
    //alert('2');
    Ext.each (source.getRange (), function (record) {
        var newRecordData = Ext.clone (record.copy().data);
        var model = new source.model (newRecordData, newRecordData.id);

        target.add (model);
    });
    //alert('3');
    return target;
}

//对特殊字符进行转换
function SpecialStrEncode(str) {
    //利用正则表达式把所有的“$”替换为“#”。“$”为特殊字符，所以前面要加“\”。
//    var regS = new RegExp("\\#","g");
//    alert(str.replace(regS,"%23"));
//    字符 特殊字符的含义 URL编码
//    # 用来标志特定的文档位置 %23
//    % 对特殊字符进行编码 %25
//    & 分隔不同的变量值对 %26
//    + 在变量值中表示空格 %2B
//    \ 表示目录路径 %2F
//    = 用来连接键和值 %3D
//    ? 表示查询字符串的开始 %3F

    str = str.replace(/%/g,"%25");
    str = str.replace(/#/g,"%23");
    str = str.replace(/&/g,"%26");
    str = str.replace(/\+/g,"%2B");
    str = str.replace(/\\/g,"%2F");
    str = str.replace(/=/g,"%3D");
    str = str.replace(/\?/g,"%3F");
    return str;
}
function GetTopologyStoreData(){    //获取梯级结构
    var tempStoreId = 'myTopologyStore';
    var TopologyStore = Ext.getStore(tempStoreId);
    if(TopologyStore==undefined){     //如果数据没有定义
         
        try{
            RunWebService(false,"getAllTopoInfo","/pms/Base","/doreaddata/getdata",null,null,
            function(data){
                    var jqueryObj = $(data);
                    // 获取message节点
                    var message = jqueryObj.children();
                    var text = message.text();
                    text = StringDeCompress(text);
                    eval(text);
                    // 获取文本内容
                    //获取元素个数
                    if (ret.getAllTopoInfo.length > 0 ) {
                         TopologyStore = new Ext.data.ArrayStore({
                                    id:"myTopologyStore",
                                    fields: ['label', 'data'],
                                    autoLoad: false
                                });
			            bgbasin=[];
                        var Stations=ret.getAllTopoInfo;
			            topologies=[];//如何处理
			            var len=Stations.length;
			            var TopoCode="";
			            for (var i=0; i < len; i++)
			            {
				            TopoCode=Stations[i].TopoCode;
				            if (TopoCode == "")
				            {
					            TopoCode="0";
				            }
				            var TopoInfo=topologies[TopoCode];
				            if (TopoInfo == null)
				            {
					            TopoInfo=new Object();
					            TopoInfo.code=TopoCode;
					            TopoInfo.name=Stations[i].TopoName;
					            TopoInfo.Desc=Stations[i].TopDesc;
					            topologies[TopoCode]=TopoInfo;
					            bgbasin.push({label: TopoInfo.name, data: TopoInfo});
				            }
				            var sPTCode=Stations[i].PTCode.toString();
				            if (TopoInfo.stations == null)
					            TopoInfo.stations=new Object();
				            var stationInfo=TopoInfo.stations[sPTCode];
				            if (stationInfo == null)
				            {
					            stationInfo=new Object();
					            stationInfo.stcdt=sPTCode;
					            stationInfo.name=Stations[i].PTName;
					            stationInfo.LagTime=Stations[i].LagTime;
					            stationInfo.Serial=Stations[i].Serial;
					            TopoInfo.stations[sPTCode]=stationInfo;
				            }
			            }
                        TopologyStore.loadData(bgbasin);
                     }
                    else {
                        //Ext.Msg.alert("操作失败,请检查输入数据！");
                        layer.msg('操作失败,请检查输入数据！', {icon: 1});
                    }
                    //myMask.hide(); //隐藏提示对象
                    return TopologyStore;
                });
        } 
        catch(err)
        {
            //myMask.hide();//隐藏提示对象
            //Ext.Msg.alert('提示1',err);
            layer.msg(err.message, {icon: 1});
        }
    }
    else {
         return TopologyStore;
    }

}
function getplantStore() 
{
    var PTCodeStoreId = 'myPTCodeStore';
    var PTCodeStore = Ext.getStore(PTCodeStoreId);
    if(PTCodeStore==undefined){     //如果数据没有定义
 	    //填充所有电站列表
        return RunWebService(false,"GetPTCode","/pms/Base","/doreaddata/getdata",null,null,GetPTCodeResult);
	   }
    else
    {
      return PTCodeStore;
    }
   
}
function getlocaleStore() 
{
    var LocaleCodeStoreId = 'myLocaleCodeStore';
    var LocaleCodeStore = Ext.getStore(LocaleCodeStoreId);
    if(LocaleCodeStore==undefined){     //如果数据没有定义
 	    //填充所有电站列表
	  return  RunWebService(false,"GetLocaleCode","/pms/Base","/doreaddata/getdata",null,null,GetLocaleCodeResult);
    }
    else
    {
      return LocaleCodeStore;
    }
   
}
function getPrecipitationStore() 
{
    var PrecipitationCodeStoreId = 'myPrecipitationCodeStore';
    var PrecipitationCodeStore = Ext.getStore(PrecipitationCodeStoreId);
    if(PrecipitationCodeStore==undefined){     //如果数据没有定义
 	    //填充所有电站列表
	 return RunWebService(false,"GetPrecipitationCode","/pms/Base","/doreaddata/getdata",null,null,GetPrecipitationCodeResult);
    }
    else
    {
      return PrecipitationCodeStore;
    }
}
function RunWebService(async,keys,action,dispatch,paramNames,paramValues,func)
{
   try{
			//加密
            var keyHex = CryptoJS.enc.Utf8.parse(myKey);
            var keyIV=keyHex;
            var names = [];
            var values = [];
            var names=[];
            var values=[];

            names[0]="action";
		    values[0]=action;
		    //dispatch
		    names[1]="dispatch";
		    values[1]=dispatch;
		
		    names[2]="keys";
		    values[2]=keys;
		    if (paramNames!=null)
		    {
				names[3]="paramNames";
				values[3]=paramNames;
		
				names[4]="paramValues";
				values[4]=paramValues;
		    }
		    $.ajax({
                type: "post",
                async: async, //同步
                url: myURL + "/ActionContrllerWebService.asmx/Run",
                data: { language: "js", "names": CryptoArray(names), "values": CryptoArray(values)},
                contentType: "application/x-www-form-urlencoded;charset=utf-8", //"application/json",
                traditional: true,
                dataType: "xml",
                success: func,
                error: function (XMLHttpRequest, textStatus, errorThrown)
                    {
                        //Ext.Msg.alert('提示2', textStatus);
                        layer.msg(textStatus, {icon: 1});
                    }
                });
            }
	    catch(err)
            {
                 //Ext.Msg.alert('提示1',err);
                 layer.msg(err.message, {icon: 1});
            }
 }
 function RunWebServiceJSON(async, keys, action, dispatch, paramNames, paramValues, func) {
    try{
         var names = [];
         var values = [];
         var names=[];
         var values=[];
  
          names[0]="action";
              values[0]=action;
              //dispatch
              names[1]="dispatch";
              values[1]=dispatch;
          
              names[2]="keys";
              values[2]=keys;
              if (paramNames!=null)
              {
                  names[3]="paramNames";
                  values[3]=paramNames;
          
                  names[4]="paramValues";
                  values[4]=paramValues;
              }
              $.ajax({
                  type: "post",
                  async: async, //同步
                  url: myURL + "/ActionContrllerWebService.asmx/Run",
                  data: { language: "json", "names": names, "values": values},
                  contentType: "application/x-www-form-urlencoded;charset=utf-8", //"application/json",
                  traditional: true,
                  dataType: "xml",
                  success: func,
                  error: function (XMLHttpRequest, textStatus, errorThrown)
                      {
                          //Ext.Msg.alert('提示2', textStatus);
                          layer.msg(textStatus, {icon: 1});
                      }
                  });
              }
          catch(err)
              {
                   //Ext.Msg.alert('提示1',err);
                   layer.msg(textStatus, {icon: 1});
              }
          }
function GetPTCodeResult(xml)
{ 
    // 接收服务器端返回的数据
    // 需要将data这个dom对象中的数据解析出来
    // 首先需要将dom的对象转换成JQuery的对象
    var jqueryObj = $(xml);
    // 获取message节点
    var message = jqueryObj.children();
    var text = message.text();
    text = StringDeCompress(text);
    eval(text);
    // 获取文本内容
    //获取元素个数
    if (ret.GetPTCode.length > 0 ) {
            var storePTCode = new Ext.data.ArrayStore({
            id:"myPTCodeStore",
            fields: ['PTCode', 'PTName'],
            autoLoad: false
        });
        storePTCode.loadData(ret.GetPTCode);
         return storePTCode;
        }
}
function GetLocaleCodeResult(xml)
{ 

     // 接收服务器端返回的数据
    // 需要将data这个dom对象中的数据解析出来
    // 首先需要将dom的对象转换成JQuery的对象
    var jqueryObj = $(xml);
    // 获取message节点
    var message = jqueryObj.children();
    var text = message.text();
    text = StringDeCompress(text);
    eval(text);
    // 获取文本内容
    //获取元素个数
    if (ret.GetLocaleCode.length > 0 ) {
            var storeLocaleCode = new Ext.data.ArrayStore({
            id:"myLocaleCodeStore",
            fields: ['PTCode','STCDT', 'Sub_NAME'],
            autoLoad: false
        });
        storeLocaleCode.loadData(ret.GetLocaleCode);
        return storeLocaleCode;    
        } 	

}
function sleep(numberMillis) { 
    var now = new Date();
    var exitTime = now.getTime() + numberMillis;
    while (true) {
    now = new Date(); 
    if (now.getTime() > exitTime)
    return; 
    }
};
function GetPrecipitationCodeResult(xml)
{ 
    // 接收服务器端返回的数据
    // 需要将data这个dom对象中的数据解析出来
    // 首先需要将dom的对象转换成JQuery的对象
    var jqueryObj = $(xml);
    // 获取message节点
    var message = jqueryObj.children();
    var text = message.text();
    text = StringDeCompress(text);
    eval(text);
    // 获取文本内容
    //获取元素个数
    if (ret.GetPrecipitationCode.length > 0 ) {
            storePrecipitationCode = new Ext.data.ArrayStore({
            id:"myPrecipitationCodeStore",
            fields: ['STCDT', 'Sub_NAME'],
            autoLoad: false
        });
        storePrecipitationCode.loadData(ret.GetPrecipitationCode);
        return storePrecipitationCode;
        } 	  
 }
 //字符串解压缩
function StringDeCompress(strCompressed)
{
    strCompressed = strCompressed.replace(/[^a-z0-9+/=]/gi, '')
    /*var words  = CryptoJS.enc.Base64.parse(strCompressed);
    var textReader=words.toString(CryptoJS.enc.Latin1)
    var dec = zip_inflate(textReader);*/
    var words  = CryptoJS.enc.Base64.parse(strCompressed);
    //解密
    var keyHex = CryptoJS.enc.Utf8.parse(myKey);
    var keyIV=keyHex;
    var decrypted = CryptoJS.DES.decrypt({ciphertext: words}, keyHex,{iv: keyIV});
    var textReader=decrypted.toString(CryptoJS.enc.Latin1);
    //解压
    var dec=zip_inflate(textReader);
    return dec;
}

//获取Cookie数据
function getCookie(name)//取cookies函数
{
    var arr = document.cookie.match(new RegExp("(^| )"+name+"=([^;]*)(;|$)"));
    if(arr != null) return unescape(arr[2]); return null;
}

//找出数组中的index
function FindValueInArray(objArray,value){
    for(var i= 0,len=objArray.length;i<len;i++)
        if(objArray[i]==value)
            return i;
    return -1;
}

//获取对象属性的长度
function getObjectAttirbuteLength(obj){
    var result = 0;
    if(obj!=undefined){
        for(i in obj)
            result++;
    }
    return result;
}

// 这个函数用来把字符串转换为日期格式
function parseDate(str)
{
    return new Date(Date.parse(str.replace(/-/g,"/")));
}

function addCookie(objName,objValue)
{      //添加cookie
    var str = objName + "=" + escape(objValue);
    document.cookie = str;
}

 //加密数组
function  CryptoArray(values)
{
    var keyHex = CryptoJS.enc.Utf8.parse(myKey);
    var keyIV=keyHex;
    for(var i = 0; i<values.length;i++)
    {
        var queryStr = CryptoJS.DES.encrypt(values[i], keyHex, { iv: keyIV });
        queryStr = queryStr.ciphertext.toString(CryptoJS.enc.Base64);
        values[i]=queryStr;
    }
    return values;
}
function GetTreeStoreData(storeParas){    //功能名称，人工或自动化，原始库、技术库或整编库
    //alert('storeParas:'+storeParas);
    var tempParas = storeParas.split(";");//用分号隔开
    if(tempParas.length<=0) //如果没有解析参数就直接返回
        return;
    var tempStoreId = 'myTreeStore';
    var functionName = tempParas[0];
    var storeName = tempParas[1];
    if(functionName=="监测信息") {
        if(storeName=='人工')
            tempStoreId = 'myDeviceTreeStore01';
        else  if(storeName=='自动化')
            tempStoreId = 'myDeviceTreeStore02';
    }
	else if(functionName=="文档查询") {
        if(storeName=='管养技术资料')
            tempStoreId = 'myDocInfoQuery01';
        else  if(storeName=='图表整编')
            tempStoreId = 'myDocInfoQuery02';
			else  if(storeName=='工程资料')
            tempStoreId = 'myDocInfoQuery03';
			else  if(storeName=='报告整编')
            tempStoreId = 'myDocInfoQuery04';
			else  if(storeName=='安全分析成果')
            tempStoreId = 'myDocInfoQuery05';
    }
    else 
        tempStoreId+=storeParas;

    var tempStore =Ext.getStore(tempStoreId);
    if(tempStore==undefined){     //如果数据没有定义
        //alert('新建');
        

        try{
            var tempUrl = myURL + '/ActionContrllerWebService.asmx/GetJsonTree';
			//加密
            var keyHex = CryptoJS.enc.Utf8.parse(myKey);
            var keyIV=keyHex;
            var IDStr=CryptoJS.DES.encrypt(storeParas, keyHex,{iv: keyIV});
            IDStr= IDStr.ciphertext.toString(CryptoJS.enc.Base64);
            var tempData = {JsontreeID:IDStr};

            $.ajax({
                type: "post",
                url:tempUrl,
                async: false, //同步
                data:tempData, //{instrumentType:'人工' },
                dataType:"text",
                success: function(result){
                    //alert('result:'+result);
                    var jsonData;
                    eval('jsonData='+result);

					if(jsonData.success=="true")  //如果返回数据正确
                    {
                        tempStore = Ext.create('Ext.data.TreeStore', {
                            id:tempStoreId,
                            root: { expanded: true },
                            autoLoad: true
                        });
                        myMask.hide();//隐藏提示对象
	                    //console.debug(jsonData.data);
						//console.debug(StringDeCompress(jsonData.data));
                        tempStore.setRootNode(Ext.JSON.decode(StringDeCompress(jsonData.data)));
                        //var resultStore = Ext.create('Ext.data.TreeStore', {
                        //    root: { expanded: false },
                        //    autoLoad: true
                        //});
                        //resultStore.setRootNode(Ext.JSON.decode(StringDeCompress(jsonData.data)));
                        //复制TreeStore的数据
                        //直接定位于某一个树状目录菜单？
                        //copyTreeStore(tempStore,resultStore);
                        //resultStore.setRootNode(jsonData.data);
                        
                        //return resultStore;
                        
                    }
                    else{ //返回数据不成功，输出消息msg
                        //myMask.hide();//隐藏提示对象
                        //Ext.Msg.alert('提示', jsonData.msg);
                        layer.msg(jsonData.msg, {icon: 1});
                    }
                },
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                error: function (XMLHttpRequest, textStatus, errorThrown)
                {
                    //myMask.hide();//隐藏提示对象
                    //Ext.Msg.alert('提示2', textStatus);
                    layer.msg(textStatus, {icon: 1});
                }
            });
        } catch(err)
        {
            //myMask.hide();//隐藏提示对象
           //Ext.Msg.alert('提示1',err);
           layer.msg(err.message, {icon: 1});
        }
        return tempStore;
    }
    else {
        //alert('已有');
        var resultStore = Ext.create('Ext.data.TreeStore', {
            root: { expanded: false },
            autoLoad: true
        });
        //复制TreeStore的数据
        //copyTreeStore(tempStore,resultStore);
        //直接定位于某一个树状目录菜单？
        //return resultStore;
        return tempStore
    }

}
//复制TreeStore中的数据
function copyTreeStore(store1,store2){
    var clone = function(node) {
        var result = node.copy(),
            len = node.childNodes ? node.childNodes.length : 0, i;
        // Move child nodes across to the copy if required
        for (i = 0; i < len; i++) {
            var tempNode = clone(node.childNodes[i]);
            tempNode.collapse();//收缩
            result.appendChild(tempNode);
        }
        return result;
    };
    var oldRoot = store1.getRootNode(),
        newRoot = clone(oldRoot);
    store2.setRootNode(newRoot);
}
function watermark(settings) {

    //默认设置
    var defaultSettings={
        watermark_txt:"text",
        watermark_x:20,//水印起始位置x轴坐标
        watermark_y:200,//水印起始位置Y轴坐标
        watermark_rows:20,//水印行数
        watermark_cols:20,//水印列数
        watermark_x_space:100,//水印x轴间隔
        watermark_y_space:50,//水印y轴间隔
        watermark_color:'#000000',//水印字体颜色
        watermark_alpha:0.15,//水印透明度
        watermark_fontsize:'18px',//水印字体大小
        watermark_font:'微软雅黑',//水印字体
        watermark_width:120,//水印宽度
        watermark_height:80,//水印长度
        watermark_angle:15//水印倾斜度数
    };
    //采用配置项替换默认值，作用类似jquery.extend
    if(arguments.length===1&&typeof arguments[0] ==="object" )
    {
        var src=arguments[0]||{};
        for(key in src)
        {
            if(src[key]&&defaultSettings[key]&&src[key]===defaultSettings[key])
                continue;
            else if(src[key])
                defaultSettings[key]=src[key];
        }
    }

    var oTemp = document.createDocumentFragment();

    //获取页面最大宽度
    var page_width = 1960;//Math.max(document.body.clientWidth,document.body.clientWidth);
    //获取页面最大长度
    var page_height =400;// Math.max(document.body.clientHeight,document.body.clientHeight);

    //如果将水印列数设置为0，或水印列数设置过大，超过页面最大宽度，则重新计算水印列数和水印x轴间隔
    if (defaultSettings.watermark_cols == 0 ||
 (parseInt(defaultSettings.watermark_x 
+ defaultSettings.watermark_width *defaultSettings.watermark_cols 
+ defaultSettings.watermark_x_space * (defaultSettings.watermark_cols - 1)) 
> page_width)) {
        defaultSettings.watermark_cols = 
parseInt((page_width
-defaultSettings.watermark_x
+defaultSettings.watermark_x_space) 
/ (defaultSettings.watermark_width 
+ defaultSettings.watermark_x_space));
        defaultSettings.watermark_x_space = 
parseInt((page_width 
- defaultSettings.watermark_x 
- defaultSettings.watermark_width 
* defaultSettings.watermark_cols) 
/ (defaultSettings.watermark_cols - 1));
    }
    //如果将水印行数设置为0，或水印行数设置过大，超过页面最大长度，则重新计算水印行数和水印y轴间隔
    if (defaultSettings.watermark_rows == 0 ||
 (parseInt(defaultSettings.watermark_y 
+ defaultSettings.watermark_height * defaultSettings.watermark_rows 
+ defaultSettings.watermark_y_space * (defaultSettings.watermark_rows - 1)) 
> page_height)) {
        defaultSettings.watermark_rows = 
parseInt((defaultSettings.watermark_y_space 
+ page_height - defaultSettings.watermark_y) 
/ (defaultSettings.watermark_height + defaultSettings.watermark_y_space));
        defaultSettings.watermark_y_space = 
parseInt((page_height 
- defaultSettings.watermark_y 
- defaultSettings.watermark_height 
* defaultSettings.watermark_rows) 
/ (defaultSettings.watermark_rows - 1));
    }
    var x;
    var y;
    for (var i = 0; i < defaultSettings.watermark_rows; i++) {
        y = defaultSettings.watermark_y + (defaultSettings.watermark_y_space + defaultSettings.watermark_height) * i;
        for (var j = 0; j < defaultSettings.watermark_cols; j++) {
            x = defaultSettings.watermark_x + (defaultSettings.watermark_width + defaultSettings.watermark_x_space) * j;

            var mask_div = document.createElement('div');
            mask_div.id = 'mask_div' + i + j;
            mask_div.appendChild(document.createTextNode(defaultSettings.watermark_txt));
            //设置水印div倾斜显示
            mask_div.style.webkitTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.MozTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.msTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.OTransform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.transform = "rotate(-" + defaultSettings.watermark_angle + "deg)";
            mask_div.style.visibility = "";
            mask_div.style.position = "absolute";
            mask_div.style.left = x + 'px';
            mask_div.style.top = y + 'px';
            mask_div.style.overflow = "hidden";
            mask_div.style.zIndex = "9999";
            //mask_div.style.border="solid #eee 1px";
            mask_div.style.opacity = defaultSettings.watermark_alpha;
            mask_div.style.fontSize = defaultSettings.watermark_fontsize;
            mask_div.style.fontFamily = defaultSettings.watermark_font;
            mask_div.style.color = defaultSettings.watermark_color;
            mask_div.style.textAlign = "center";
            mask_div.style.width = defaultSettings.watermark_width + 'px';
            mask_div.style.height = defaultSettings.watermark_height + 'px';
            mask_div.style.display = "block";
            oTemp.appendChild(mask_div);
        };
    };
    if (document.body!=null)
      document.body.appendChild(oTemp);
}


// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
    if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

