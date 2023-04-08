var mapView, M_map;

var CurrentMapMouseType = 0;

//站点图层
var StationLayerVectorSource = new ol.source.Vector({});
var StationLayerStyles = {};
var StationLayer = null;

var popups_container = document.getElementById('popup');
var popups_content = document.getElementById('popup-content');
var popups_closer = document.getElementById('popup-closer');

var popups = new ol.Overlay( /** @type {olx.OverlayOptions} */ ({
	element: popups_container,
	autoPan: true,
	autoPanAnimation: {
		duration: 250
	}
}));

//关闭提示
function popups_hide() {
	popups.setPosition(undefined);
	popups_closer.blur();
}

popups_closer.onclick = function() {
	popups_hide();
	return false;
}

function initialMap() {
	mapView = new ol.View({
		maxZoom: 20,
		center: ol.proj.fromLonLat([114.175097031, 22.598981100]),
		rotation: -Math.PI / 9,
		zoom: 15
	});
	M_map = new ol.Map({
		target: 'distribution_map',
		attributionControl: false,
		overlays: [popups],
		layers: [
			//StationLayer
		],
		view: mapView,
		interactions: ol.interaction.defaults({
                    doubleClickZoom: true, //
                    mouseWheelZoom: false,
                    shiftDragZoom: false,
                    pinchZoom:false
                })
	});
	var MyBaseLayer = new ol.layer.Tile({
		name: '我的地图',
		visible: true,
		//opacity:0.3,
		source: new ol.source.XYZ({
			url: "tiles/basemap/L{z}/{y}-{x}.png"
		})
	});
	M_map.addLayer(MyBaseLayer);
	$(".ol-zoom, .ol-zoomslider").remove();

	//站点图层
	StationLayer = new ol.layer.Vector({
		name: "站点图层",
		source: StationLayerVectorSource,
		projection: 'EPSG:3857',
		style: function(feature) {
			var ftype = feature.get('ftype');
			var tmpStationType = feature.get('stationtype');
			var tmpStyle = StationLayerStyles[tmpStationType];
			if(!tmpStyle) {
				var tmpImgs = ['dm.png','SK.png','PSK.png','TQ.png'];
				tmpStyle = new ol.style.Style({
					image: new ol.style.Icon({
						opacity: 1,
						src: 'images/'+tmpStationType+'.png',
						scale: 0.7
					}),
					text: new ol.style.Text({
						font: 'Bold 14px Arial,sans-serif',
						exceedLength: true,
						offsetY: 30,
						fill: new ol.style.Fill({
							color: '#00f6ff'
						})
					})
				});
				StationLayerStyles[tmpStationType] = tmpStyle;
			}
			if(mapView.getResolution()<1)
				tmpStyle.getText().setText(feature.get('nickname'));
			else
				tmpStyle.getText().setText("");
			return tmpStyle;
		}
	});
	M_map.addLayer(StationLayer);

	var mapselect = new ol.interaction.Select({
		layers: [riverSectionLayer, StationLayer]
	});
	M_map.addInteraction(mapselect);
	// 在选中>显示/隐藏弹出窗口
	mapselect.getFeatures().on(['add'], function(e) {
		var feature = e.element;
		var content = "";

		if(CurrentMapMouseType == 1) //测量
		{
			popups_hide();
			return;
		} else if(CurrentMapMouseType == 0) {
			var tmpFeatureType = feature.get("ftype");
			if(tmpFeatureType != undefined) {
				if(tmpFeatureType == "gpslayer") //GPS人员定位图层中人员图标
				{
					var tmpMsg = feature.get("msg");
					content = tmpMsg.replace(/<n>/g, "<br/>");
					popups_content.innerHTML = content;
					popups.setPosition(feature.getGeometry().getCoordinates());
					return;
				} else if(tmpFeatureType == "StationLayer") //监测站
				{
					var tmpMsg = feature.get("msg");
					var tmpContent = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + feature.get("nickname") + "】</font></label><br/></div>" +
						tmpMsg;
					popups_content.innerHTML = tmpContent;
					popups.setPosition(feature.getGeometry().getCoordinates());
					return;
				} else if(tmpFeatureType == "river") {
					var monitorvalue = feature.get("monitorvalue");
					var name = feature.get("name");
					if(monitorvalue != "" || monitorvalue != undefined) {
						layer.msg(name + "<br/>降雨量:" + monitorvalue);
					}
				} else if(tmpFeatureType == "usergpslayer") //usergpslayer
				{
					var tmpMsg = feature.get("msg");
					content = tmpMsg.replace(/<n>/g, "<br/>");
					popups_content.innerHTML = content;
					popups.setPosition(feature.getGeometry().getCoordinates());
					return;
				}
			} else if(tmpFeatureType == undefined) {
				var monitorvalue = feature.get("monitorvalue");
				var name = feature.get("name");
				if(name != undefined && monitorvalue != undefined) {
					var tmpContent = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + name + "】</font></label><br/></div>" +
						"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">参数值:</font>" + monitorvalue +
						"</label><br/>";
					popups_content.innerHTML = tmpContent;
					var tmpCoords = feature.getGeometry().getCoordinates()[0][0];
					popups.setPosition(tmpCoords);
				}
			}
			return;
		}
	});
	mapselect.getFeatures().on(['remove'], function(e) {
		//popups.hide();
		popups_hide();
	})
}

function loadStationLayers() { //加载断面站点
	//刷新设施位置信息
	$.ajax({
		url: ServerURL + "/ServiceHandler/BigDataHandler.ashx?method=getfacilitiesinfobyprojectid&projectid="+CurrentProjectID+"&token="+defaultToken,
		type: "GET",
		dataType: "json",
		success: function(joResult) {
			if(joResult.success == true) {
				var tmpObjs = joResult.data;
				StationLayerVectorSource.clear();
				for(var i in tmpObjs) {
					var tmpObj = tmpObjs[i];
					var tmpX = parseFloat(tmpObj.X),
						tmpY = parseFloat(tmpObj.Y);
					if(tmpX > 1 && tmpY > 1) {
						var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
						var tmpNickName = tmpObj.Name;
						var tmpType = tmpObj.FacilitiesType;
						var tmpMsg =
							"<label>设施类型：" + tmpObj.FacilitiesType + "</label><br/>" +
							"<label>设施信息：" + tmpObj.Info + "</label>";							
						var tmpPoint = new ol.geom.Point(tmpCoordinate);
						var tmpFeature = new ol.Feature({
							name: tmpNickName,
							geometry: tmpPoint
						});
						tmpFeature.set("ftype", "StationLayer", false);
						tmpFeature.set("nickname", tmpNickName, false);
						tmpFeature.set("msg", tmpMsg, false);
						tmpFeature.set("stationtype", tmpType, false);
						StationLayerVectorSource.addFeature(tmpFeature);
					}
				};
			}
		}
	});
}

var m_GradientColorMax = 100,
	m_GradientColorMin = 0,
	m_GradientColorStep = 1;

var riverSectionSource, riverSectionLayer; //河道计算断面图层
var riverSectionStyles = {};

var gradient = gradientColor('#0000FF', '#FF0000', 10); //'#e82400','#8ae800'
for(var i = 0; i < 10; i++) {
	riverSectionStyles[i] = new ol.style.Style({
		stroke: new ol.style.Stroke({
			color: gradient[i],
			width: 1
		}),
		fill: new ol.style.Fill({
			color: gradient[i]
		})
	});
}

var riverSectionStyleFunction = function(feature) {
	var tmpStyleIndex = 0;
	var tmpmonitorvalue = feature.get('monitorvalue');
	if(m_GradientColorStep <= 0)
		m_GradientColorStep = 1;
	var tmpI = parseInt((tmpmonitorvalue - m_GradientColorMin) / m_GradientColorStep);
	if(tmpI > 9)
		tmpI = 9;
	return riverSectionStyles[tmpI];
};

riverSectionSource = new ol.source.Vector({        
	projection: 'EPSG:4326',
	url: ServerURL + '/ServiceHandler/RiverBoundaryHandler.ashx?method=getorigriverboundary', //'riversection.geojson',
	useSpatialIndex: false,
	format: new ol.format.GeoJSON()      
});

riverSectionLayer = new ol.layer.Vector({
	name: "河道断面",
	source: riverSectionSource,
	style: riverSectionStyleFunction
	//	new ol.style.Style({
	//		stroke: new ol.style.Stroke({
	//			color: 'yellow',
	//			width: 1
	//		}),
	//		fill: new ol.style.Fill({
	//			color: 'rgba(255, 255, 0, 0.1)'
	//		})
	//	}) 
});

/*
// startColor：开始颜色hex
// endColor：结束颜色hex
// step:几个阶级（几步）
*/
function gradientColor(startColor, endColor, step) {
	startRGB = colorToRgb(startColor); //转换为rgb数组模式
	startR = startRGB[0];
	startG = startRGB[1];
	startB = startRGB[2];

	endRGB = colorToRgb(endColor);
	endR = endRGB[0];
	endG = endRGB[1];
	endB = endRGB[2];

	sR = (endR - startR) / step; //总差值
	sG = (endG - startG) / step;
	sB = (endB - startB) / step;

	var colorArr = [];
	for(var i = 0; i < step; i++) {
		//计算每一步的hex值
		var hex = colorToHex('rgb(' + parseInt((sR * i + startR)) + ',' + parseInt((sG * i + startG)) + ',' + parseInt((sB * i + startB)) + ')');
		colorArr.push(hex);
	}
	return colorArr;
}

// 将hex表示方式转换为rgb表示方式(这里返回rgb数组模式)
function colorToRgb(sColor) {
	var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/;
	var sColor = sColor.toLowerCase();
	if(sColor && reg.test(sColor)) {
		if(sColor.length === 4) {
			var sColorNew = "#";
			for(var i = 1; i < 4; i += 1) {
				sColorNew += sColor.slice(i, i + 1).concat(sColor.slice(i, i + 1));
			}
			sColor = sColorNew;
		}
		//处理六位的颜色值
		var sColorChange = [];
		for(var i = 1; i < 7; i += 2) {
			sColorChange.push(parseInt("0x" + sColor.slice(i, i + 2)));
		}
		return sColorChange;
	} else {
		return sColor;
	}
};

// 将rgb表示方式转换为hex表示方式
function colorToHex(rgb) {
	var _this = rgb;
	var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/;
	if(/^(rgb|RGB)/.test(_this)) {
		var aColor = _this.replace(/(?:\(|\)|rgb|RGB)*/g, "").split(",");
		var strHex = "#";
		for(var i = 0; i < aColor.length; i++) {
			var hex = Number(aColor[i]).toString(16);
			hex = hex < 10 ? 0 + '' + hex : hex; // 保证每个rgb的值为2位
			if(hex === "0") {
				hex += hex;
			}
			strHex += hex;
		}
		if(strHex.length !== 7) {
			strHex = _this;
		}

		return strHex;
	} else if(reg.test(_this)) {
		var aNum = _this.replace(/#/, "").split("");
		if(aNum.length === 6) {
			return _this;
		} else if(aNum.length === 3) {
			var numHex = "#";
			for(var i = 0; i < aNum.length; i += 1) {
				numHex += (aNum[i] + aNum[i]);
			}
			return numHex;
		}
	} else {
		return _this;
	}
}

function setRiverSectionsValue(values) {
	try {
		var tmpFeatures = riverSectionSource.getFeaturesCollection();
		var tmpTotalCount = values.length;
		var tmpCount = tmpFeatures.getLength();
		if(tmpTotalCount > tmpCount)
			tmpTotalCount = tmpCount;
		var tmpLastLevel = 0;
		m_GradientColorMax = -10000000;
		m_GradientColorMin = 10000000;
		for(var i = 0; i < tmpTotalCount; i++) {
			var tmpObj = values[i];
			var tmpLevel = parseFloat(tmpObj);
			var tmpFeature = tmpFeatures.item(i);
			tmpFeature.set("monitorvalue", tmpLevel, false);
			tmpLastLevel = tmpLevel;
			if(m_GradientColorMax < tmpLevel)
				m_GradientColorMax = tmpLevel;
			if(m_GradientColorMin > tmpLevel)
				m_GradientColorMin = tmpLevel;
		}
		m_GradientColorStep = (m_GradientColorMax - m_GradientColorMin) / 10;
		for(var i = tmpTotalCount; i < tmpCount; i++) {
			var tmpFeature = tmpFeatures.item(i);
			tmpFeature.set("monitorvalue", tmpLastLevel, false);
		}
	} catch(e) {
		//TODO handle the exception
	}
}

initialMap(); //地图初始化
loadStationLayers(); //加载
M_map.addLayer(riverSectionLayer);