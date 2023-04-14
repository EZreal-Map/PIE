var mapView, M_map;


var TianDiTuTK = "653640d0fb1d75ae322353e33e8a0e90";

var CurrentMapMouseType = 0;

var TianDitu_Val_Layer = null;
var mapselect = null;//选择对象

//站点图层
var StationLayerVectorSource = new ol.source.Vector({});
var StationLayerStyles = {};
var StationLayer = null;

//事件图层
var GYEventLayerVectorSource = new ol.source.Vector({});
var GYEventLayerStyles = {};
var GYEventLayer = null;

//用户定位图层
var UserGPSLayerVectorSource = new ol.source.Vector({});
var UserGPSLayerStyles = {};
var UserGPSLayer = null;

//台风图层
var TyphoonLayerVectorSource = new ol.source.Vector({});
var TyphoonLayerStyles = {};
var TyphoonLayer = null;
var TyphoonTrackLayerVectorSource = new ol.source.Vector({});
var TyphoonTrackLayer = null;

//摄像头图层
var Camera01Coord = [114.1820986016,22.5940973347];
var Camera02Coord = [114.1821896689,22.5941197594];
var Camera01Feature = null, Camera02Feature = null;
var CameraLayerVectorSource = new ol.source.Vector({});
var CameraLayer = null;

//雷达信息
var RadarTrackLayerVectorSource = new ol.source.Vector({});
var RadarTrackLayer = null;

var DefaultMapTipParaName = "参数值:";

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


		//GPS轨迹节点图层	
		var gpsTrackPointsVectorSource = new ol.source.Vector({});
		var GPSTrackPointLayer = new ol.layer.Vector({
				name: "GPS轨迹节点",
				source: gpsTrackPointsVectorSource,
				projection: 'EPSG:3857',
				style: new ol.style.Style({
						image:new ol.style.Circle({
							radius: 30,
							fill: null  
					})
			})
		});
			
		//GPS轨迹图层
		var gpsTrackLineFeatures = new ol.Collection();
		var GPSTrackLayer = new ol.layer.Vector({
			name: 'GPS轨迹',
			source: new ol.source.Vector({
				features: gpsTrackLineFeatures
			}),
			style: function(f) {
				var styles = [
			        new ol.style.Style({
			        stroke: new ol.style.Stroke({
			            color: '#FFA500',
			            lineDash:[10,10],
			            width: 5
				        })
				    })
				];        
				var geometry = f.getGeometry();
				geometry.forEachSegment(function(start, end) {
			          var dx = end[0] - start[0];
			          var dy = end[1] - start[1];
			          var rotation = Math.atan2(dy, dx);
			          // arrows
			          styles.push(new ol.style.Style({
			            geometry: new ol.geom.Point(end),
			            image: new ol.style.Icon({
			              src: 'images/arrowright002.png',
			              anchor: [0.75, 0.5],
			              rotateWithView: true,
			              rotation: -rotation,
			              scale: 1
			            })
			        }));
			    });
			    return styles;        	
			}
		});

var m_DefaultMapCenterOfWGS84 = [114.178097031, 22.590081100];
var m_DefaultMapCenter = ol.proj.fromLonLat(m_DefaultMapCenterOfWGS84);

function initialMap() {
	mapView = new ol.View({
		maxZoom: 20,
		center: m_DefaultMapCenter,
		//rotation: -Math.PI / 9,
		zoom: 13
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
			            mouseWheelZoom: true,
			            shiftDragZoom: false,
			            pinchZoom: false          
		})
	});
	
	var TianDitu_Val_LayerArray = [new ol.layer.Tile({
								name: '天地图-矢量',
								visible: true,
								source: new ol.source.XYZ({
									crossOrigin:'anonymous',
									url: "http://t3.tianditu.com/DataServer?T=img_w&x={x}&y={y}&l={z}&tk="+TianDiTuTK,
									maxZoom: 18
								})
							}),new ol.layer.Tile({
								name: '天地图-注记',
								visible: false,
								source: new ol.source.XYZ({
									crossOrigin:'anonymous',
									url: "http://t2.tianditu.com/DataServer?T=cva_w&x={x}&y={y}&l={z}&tk="+TianDiTuTK
								})
							})];
							
	TianDitu_Val_Layer = new ol.layer.Group({name:'天地图-矢量',visible: false, layers:TianDitu_Val_LayerArray});	
	M_map.addLayer(TianDitu_Val_Layer);		
	
	var MyBaseLayer = new ol.layer.Tile({
		name: '我的地图',
		visible: true,
		//opacity:0.3,
		source: new ol.source.XYZ({
			url: "tiles/basemap/L{z}/{y}-{x}.png"
		})
	});
	M_map.addLayer(MyBaseLayer);
	var MyBaseLayer2 = new ol.layer.Tile({
		name: '我的地图',
		visible: true,
		//opacity:0.3,
		source: new ol.source.XYZ({
			url: "tiles/basemapTxt/L{z}/{y}-{x}.png"
		})
	});
	M_map.addLayer(MyBaseLayer2);
	MyBaseLayer2.setZIndex(8);
	$(".ol-zoom, .ol-zoomslider").remove();

	//全流域范围
	var allRiverZoneSource =  new ol.source.Vector({		
			                projection: 'EPSG:3857',
			                url: 'riverzone.geojson',
			                format:new ol.format.GeoJSON()
			            });		
			
	var allRiverZoneLayer = new ol.layer.Vector({
			name: "流域",
			minResolution:4.777314267823516,
			        source: allRiverZoneSource,
			        style: new ol.style.Style({
								stroke: new ol.style.Stroke({
										color: 'red',
										width: 1
								}),
								fill: new ol.style.Fill({
										color: 'rgba(0,128,128,0.2)'
								})
						})
		    });	
	M_map.addLayer(allRiverZoneLayer);	
		    
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
	StationLayer.setZIndex(9);
	
	//管养事件图层
	GYEventLayer = new ol.layer.Vector({
		name: "管养事件图层",
		source: GYEventLayerVectorSource,
		projection: 'EPSG:3857',
		style: function(feature) {
			var feventtype = feature.get('feventtype');
			var tmpStyle = GYEventLayerStyles[feventtype];
			if(!tmpStyle) {
				tmpStyle = new ol.style.Style({
					image: new ol.style.Icon({
						opacity: 1,
						src: 'images/GYEvent_0' + feventtype + '.png',
						scale: 0.7
					}),
					text: new ol.style.Text({
						font: 'Bold 14px Arial,sans-serif',
						exceedLength: true,
						offsetY: 30,
						fill: new ol.style.Fill({
							color: '#FF8800'
						})
					})
				});
				GYEventLayerStyles[feventtype] = tmpStyle;
			}
			//tmpStyle.getText().setText(feature.get('nickname'));
			return tmpStyle;
		}
	});
	M_map.addLayer(GYEventLayer);
	GYEventLayer.setZIndex(10);
	
	//用户定位图层
	UserGPSLayer = new ol.layer.Vector({
		name: "用户定位图层",
		source: UserGPSLayerVectorSource,
		projection: 'EPSG:3857',
		style: function(feature) {
			var tmpIsOnline = feature.get('isonline');
			if(tmpIsOnline == "true") //在线
			{
				var tmpStyle = UserGPSLayerStyles[0];
				if(!tmpStyle) {
					tmpStyle = new ol.style.Style({
						image: new ol.style.Icon({
							opacity: 1,
							src: 'images/user01.png',
							scale: 0.7
						}),
						text: new ol.style.Text({
							font: 'Bold 14px Arial,sans-serif',
							exceedLength: true,
							offsetY: 30,
							fill: new ol.style.Fill({
								color: '#ff0000'
							}),
							stroke: new ol.style.Stroke({
								color: '#fff',
								width: 3
							})
						})
					});
					UserGPSLayerStyles[0] = tmpStyle;
				}
				tmpStyle.getText().setText(feature.get('nickname'));
				return tmpStyle;
			} else {
				var tmpStyle = UserGPSLayerStyles[1];
				if(!tmpStyle) {
					tmpStyle = new ol.style.Style({
						image: new ol.style.Icon({
							opacity: 1,
							src: 'images/user00.png',
							scale: 0.7
						}),
						text: new ol.style.Text({
							font: 'Bold 14px Arial,sans-serif',
							exceedLength: true,
							offsetY: 30,
							fill: new ol.style.Fill({
								color: '#808080'
							}),
							stroke: new ol.style.Stroke({
								color: '#fff',
								width: 3
							})
						})
					});
					UserGPSLayerStyles[1] = tmpStyle;
				}
				tmpStyle.getText().setText(feature.get('nickname'));
				return tmpStyle;
			}
		}
	});
	M_map.addLayer(UserGPSLayer);
	UserGPSLayer.setZIndex(11);

	//用户轨迹
	M_map.addLayer(GPSTrackLayer);
	M_map.addLayer(GPSTrackPointLayer);
	GPSTrackLayer.setZIndex(12);
	GPSTrackPointLayer.setZIndex(13);
	
	//台风图层
	TyphoonLayer = new ol.layer.Vector({
		name: "台风图层",
		source: TyphoonLayerVectorSource,
		projection: 'EPSG:3857',
		style: function(feature) {
			var tmpTpClass = feature.get("tyclass");
			tmpTpClass = parseInt(tmpTpClass);
			var tmpStyle = TyphoonLayerStyles[tmpTpClass];
			if(!tmpStyle){
				var tmpRdius = [6,8,10,12,15,18,20];
				var tmpColors = ['#A4CDFF','#43FF4B','#2779F9','#DCE149','#E6B13E','#DC83F3','#FF0000'];
				tmpStyle = new ol.style.Style({
						image: new ol.style.Circle({
							radius: tmpRdius[tmpTpClass],
							fill: new ol.style.Fill({
								color: tmpColors[tmpTpClass]
							})
						}),
						stroke: new ol.style.Stroke({
							color: tmpColors[tmpTpClass],
							width: 2
						})
					});		
				TyphoonLayerStyles[tmpStyle] = tmpStyle;
			}
			return tmpStyle;
		}
	});	
	TyphoonTrackLayer = new ol.layer.Vector({
		name: "台风轨迹图层",
		source: TyphoonTrackLayerVectorSource,
		projection: 'EPSG:3857',
		style: function(feature) {
			var styles = [
					new ol.style.Style({
						stroke: new ol.style.Stroke({
							color: '#1296db',
							width: 2
						})
					})
				];
				var geometry = feature.getGeometry();
				geometry.forEachSegment(function(start, end) {
					var dx = end[0] - start[0];
					var dy = end[1] - start[1];
					var rotation = Math.atan2(dy, dx);
					// arrows
					styles.push(new ol.style.Style({
						geometry: new ol.geom.Point(end),
						image: new ol.style.Icon({
							src: 'images/arrowright.png',
							anchor: [0.75, 0.5],
							rotateWithView: true,
							rotation: -rotation,
							scale: 0.8
						})
					}));
				});
				return styles;
		}
	});
	M_map.addLayer(TyphoonLayer);
	M_map.addLayer(TyphoonTrackLayer);
	
	CameraLayer = new ol.layer.Vector({
		name: "摄像头图层",
		source: CameraLayerVectorSource,
		projection: 'EPSG:3857',
		style: new ol.style.Style({
						image: new ol.style.Icon({
							opacity: 1,
							src: 'images/camera.png',
							scale: 0.7
						})
			})
	});
	M_map.addLayer(CameraLayer);
	
	var tmpCoordinate01 = ol.proj.transform(Camera01Coord, 'EPSG:4326', 'EPSG:3857');
	var tmpPoint01 = new ol.geom.Point(tmpCoordinate01);
	Camera01Feature = new ol.Feature({
							name: "摄像头01",
							geometry: tmpPoint01
	});
	Camera01Feature.set("ftype", "camera", false);
	Camera01Feature.set("nickname", "摄像头01", false);
	CameraLayerVectorSource.addFeature(Camera01Feature);
						
	var tmpCoordinate02 = ol.proj.transform(Camera02Coord, 'EPSG:4326', 'EPSG:3857');
	var tmpPoint02 = new ol.geom.Point(tmpCoordinate02);
	Camera02Feature = new ol.Feature({
							name: "摄像头02",
							geometry: tmpPoint02
	});
	Camera02Feature.set("ftype", "camera", false);
	Camera02Feature.set("nickname", "摄像头02", false);
	CameraLayerVectorSource.addFeature(Camera02Feature);
	
	var m_RadarStyle = new ol.style.Style({
		    image: new ol.style.Circle({
		      radius: 15,
		      fill: new ol.style.Fill({
		        color: '#76EE00'
		      })
		    }),
		    text: new ol.style.Text({
						font: 'Bold 12px Arial,sans-serif',
						exceedLength: true,
						fill: new ol.style.Fill({
							color: '#000000'
						})
					})
		 });		 
	//雷达图层
	RadarTrackLayer = new ol.layer.Vector({
		name: "雷达信息图层",
		source: RadarTrackLayerVectorSource,
		projection: 'EPSG:3857',
		style:function(feature)
		{
			var tmpStyle = m_RadarStyle;
			tmpStyle.getText().setText(feature.get('nickname'));
			return tmpStyle;
		}
	});
	RadarTrackLayer.setZIndex(12);
	M_map.addLayer(RadarTrackLayer);
		
	mapselect = new ol.interaction.Select({
		layers: [riverSectionLayer, StationLayer, GYEventLayer, UserGPSLayer, TyphoonLayer,CameraLayer,GPSTrackPointLayer]
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
				} else if(tmpFeatureType == "GYEventLayer") {
					var tmpMsg = feature.get("msg");
					content = tmpMsg.replace(/<n>/g, "<br/>");
					popups_content.innerHTML = content;
					popups.setPosition(feature.getGeometry().getCoordinates());
					return;
				}else if(tmpFeatureType == "TyphoonLayer") {
					var tmpMsg = feature.get("msg");
					content = tmpMsg.replace(/<n>/g, "<br/>");
					popups_content.innerHTML = content;
					popups.setPosition(feature.getGeometry().getCoordinates());
					return;
				}
				else if(tmpFeatureType == "camera") {
					var tmpTitle = feature.get("imgtitle");
					var tmpImgID = feature.get("imgid");
					if(tmpTitle!=undefined && tmpTitle!=null && tmpTitle!="")
					{
						openCameraImg(tmpTitle, tmpImgID);
					}
					return;
				}
			} else if(tmpFeatureType == undefined) {
				var monitorvalue = feature.get("monitorvalue");
				var name = feature.get("name");
				if(name != undefined && monitorvalue != undefined) {
					var tmpContent = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + name + "】</font></label><br/></div>" +
						"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">" + DefaultMapTipParaName + ":</font>" + monitorvalue +
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
//	var data = {
//		method: "getsensordatabytype",
//		typeid: "2" //typeid=0 为水质   1水位2雨量3位移
//	}
//	$.ajax({
//		url: ServerURL + "/ServiceHandler/StationHandler.ashx",
//		type: "Post",
//		data: data,
//		dataType: "json",
//		success: function(joResult) {
//			if(joResult.success == true) {
//				var tmpObjs = joResult.msg;
//
//				for(var i = 0; i < tmpObjs.length; i++) {
//					var datahtml = "<div id=\"Stationpopup" + i + "\" class=\"ol-popup\">" +
//						"<a href=\"#\" id=\"Stationpopup-closer" + i + "\" class=\"ol-popup-closer\"></a>" +
//						"<div id=\"Stationpopup-content" + i + "\">" +
//						"</div>" +
//						"</div>";
//					var tmpObj = tmpObjs[i];
//					var tmpX = parseFloat(tmpObj.ELSO),
//						tmpY = parseFloat(tmpObj.NTLA);
//					if(tmpX > 1 && tmpY > 1) {
//						var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
//						var tmpNickName = tmpObj.STNM;
//						var tmpMsg =
//							"<div  style=\"line-height: 20px;float:left;\"><label>所属区域：" + tmpObj.STADDR + "</label></div>" +
//							"<div  style=\"line-height: 20px;float:left;\"><label>经度：" + tmpX + "</label></div>" +
//							"<div  style=\"line-height: 20px;float:left;\"><label>纬度：" + tmpY + "</label></div>";
//						var tmpPoint = new ol.geom.Point(tmpCoordinate);
//
//						var tmpFeature = new ol.Feature({
//							name: tmpMsg,
//							geometry: tmpPoint
//						});
//						var id = tmpObj.STCDT;
//						tmpFeature.set("STCDT", id, false);
//						tmpFeature.set("ftype", "StationLayer", false);
//
//						tmpFeature.set("nickname", tmpNickName, false);
//						tmpFeature.set("msg", tmpMsg, false);
//						tmpFeature.set("stationtype", 0, false);
//						StationLayerVectorSource.addFeature(tmpFeature);
//					}
//				};
//				
//			}
//		}
//	});
//	//水库
//	$.ajax({
//		url: ServerURL + "/ServiceHandler/StationHandler.ashx?method=getallrtuinfo&type=2",
//		type: "GET",
//		dataType: "json",
//		success: function(joResult) {
//			if(joResult.success == true) {
//				var tmpObjs = joResult.msg;
//				for(var i = 0; i < tmpObjs.length; i++) {						
//					var tmpObj = tmpObjs[i];
//					var tmpX = parseFloat(tmpObj.ESLO),
//						tmpY = parseFloat(tmpObj.NTLA);
//					if(tmpX > 1 && tmpY > 1) {
//						var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
//						var tmpNickName = tmpObj.RTUNAME;
//						var tmpMsg =
//							"<div  style=\"line-height: 20px;float:left;\"><label>信息：" + tmpObj.SHOW_T + "</label></div>";
//						var tmpPoint = new ol.geom.Point(tmpCoordinate);
//						var tmpFeature = new ol.Feature({
//							name: tmpNickName,
//							geometry: tmpPoint
//						});
//						tmpFeature.set("ftype", "StationLayer", false);
//						tmpFeature.set("nickname", tmpNickName, false);
//						tmpFeature.set("msg", tmpMsg, false);
//						tmpFeature.set("stationtype", 1, false);
//						StationLayerVectorSource.addFeature(tmpFeature);
//					}
//				};
//				
//			}
//		}
//	});
//	//排洪口
//	$.ajax({
//		url: ServerURL + "/ServiceHandler/StationHandler.ashx?method=getallrtuinfo&type=1",
//		type: "GET",
//		dataType: "json",
//		success: function(joResult) {
//			if(joResult.success == true) {
//				var tmpObjs = joResult.msg;
//				for(var i = 0; i < tmpObjs.length; i++) {						
//					var tmpObj = tmpObjs[i];
//					var tmpX = parseFloat(tmpObj.ESLO),
//						tmpY = parseFloat(tmpObj.NTLA);
//					if(tmpX > 1 && tmpY > 1) {
//						var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
//						var tmpNickName = tmpObj.RTUNAME;
//						var tmpMsg =
//							"<div  style=\"line-height: 20px;float:left;\"><label>信息：" + tmpObj.SHOW_T + "</label></div>";
//						var tmpPoint = new ol.geom.Point(tmpCoordinate);
//						var tmpFeature = new ol.Feature({
//							name: tmpNickName,
//							geometry: tmpPoint
//						});
//						tmpFeature.set("ftype", "StationLayer", false);
//						tmpFeature.set("nickname", tmpNickName, false);
//						tmpFeature.set("msg", tmpMsg, false);
//						tmpFeature.set("stationtype", 2, false);
//						StationLayerVectorSource.addFeature(tmpFeature);
//					}
//				};
//				
//			}
//		}
//	});
//	//桥
//	$.ajax({
//		url: ServerURL + "/ServiceHandler/StationHandler.ashx?method=getallrtuinfo&type=4",
//		type: "GET",
//		dataType: "json",
//		success: function(joResult) {
//			if(joResult.success == true) {
//				var tmpObjs = joResult.msg;
//				for(var i = 0; i < tmpObjs.length; i++) {						
//					var tmpObj = tmpObjs[i];
//					var tmpX = parseFloat(tmpObj.ESLO),
//						tmpY = parseFloat(tmpObj.NTLA);
//					if(tmpX > 1 && tmpY > 1) {
//						var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
//						var tmpNickName = tmpObj.RTUNAME;
//						var tmpMsg =
//							"<div  style=\"line-height: 20px;float:left;\"><label>信息：" + tmpObj.SHOW_T + "</label></div>";
//						var tmpPoint = new ol.geom.Point(tmpCoordinate);
//						var tmpFeature = new ol.Feature({
//							name: tmpNickName,
//							geometry: tmpPoint
//						});
//						tmpFeature.set("ftype", "StationLayer", false);
//						tmpFeature.set("nickname", tmpNickName, false);
//						tmpFeature.set("msg", tmpMsg, false);
//						tmpFeature.set("stationtype", 3, false);
//						StationLayerVectorSource.addFeature(tmpFeature);
				StationLayerVectorSource.clear();
				//					}
//				};
//				
//			}
//		}
//	});
	
	//刷新设施位置信息
	$.ajax({
		// ServerURL = "../../SZRiverSys/" 在common.js中定义
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

/**
 * 刷新用户定位信息
 */
function refreshUserGPSInfo() {
	$.ajax({
		url: ServerURL + "/ServiceHandler/BigDataHandler.ashx?method=getuserfinalgps&projectid="+CurrentProjectID,
		type: "GET",
		dataType: "json",
		success: function(joResult) {
			if(joResult.success == true) {
				var tmpObjs = joResult.data;
				if(UserGPSLayerVectorSource == null) {
					UserGPSLayerVectorSource = new ol.source.Vector();
					UserGPSLayer.setSource(UserGPSLayerVectorSource);
				} else
					UserGPSLayerVectorSource.clear(true);

				var tmpTotalUsersCount = tmpObjs.length;
				var tmpOnlineUsersCount = 0;
				for(var i in tmpObjs) {
					var tmpObj = tmpObjs[i];
					var tmpX = parseFloat(tmpObj.longitude),
						tmpY = parseFloat(tmpObj.latitude);
					if(tmpX > 1 && tmpY > 1) {
						var dateNow = new Date();
						var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
						var tmpNickName = tmpObj.RealName;
						var tmpPoint = new ol.geom.Point(tmpCoordinate);
						var tmpFeature = new ol.Feature({
							name: tmpMsg,
							geometry: tmpPoint
						});
						var tmpSpeed = parseFloat(tmpObj.speed);
						var tmpaccuracy = parseFloat(tmpObj.accuracy);
						var tmpMsg = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + tmpNickName + "】</font></label><br/></div>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">时间：</font>" + tmpObj.gpsTime + "</label><br/>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">经度：</font>" + tmpX + "</label><br/>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">纬度：</font>" + tmpY + "</label><br/>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">速度：</font>" + tmpSpeed.toFixed(2) + "m/s</label><br/>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">精度：</font>" + tmpaccuracy.toFixed(2) + "m</label><br/>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">地址：</font>" + tmpObj.address + "</label><br/>" +
							"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">备注：</font>" + tmpObj.remark + "</label><br/>" +
							"<br/><a onclick=\"queryusertrack('"+tmpObj.username+"','"+tmpNickName+"');\"><label style=\"font-size:16px;\"><font color=\"#FFFF00\">查询用户轨迹</font></label></a>"
							+"&nbsp;&nbsp;&nbsp;<a onclick=\"clearGPSTrackLayers();\"><label style=\"font-size:16px;\"><font color=\"#FFFF00\">清空用户轨迹</font></label></a><br/>";

						var tmpIsOnline = false;
						var tmpTime01 = Date.parse(tmpObj.gpsTime);
						if(dateNow - tmpTime01 < 300000) //5分钟之内都表示在线
						{
							tmpIsOnline = "true";
							tmpOnlineUsersCount++;
							
							tmpFeature.set("ftype", "usergpslayer", false);
							tmpFeature.set("nickname", tmpNickName, false);
							tmpFeature.set("isonline", tmpIsOnline, false);
							tmpFeature.set("msg", tmpMsg, false);
	
							UserGPSLayerVectorSource.addFeature(tmpFeature);
						}
					}
				}
				$('#gy_Label03').text(tmpOnlineUsersCount + "/" + tmpTotalUsersCount);				
			}
		}
	})
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

function addGYEventPoint(lng, lat, obj) {
	var tmpX = parseFloat(lng),
		tmpY = parseFloat(lat);

	var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
	var tmpNickName = obj.EventName;
	var tmpPoint = new ol.geom.Point(tmpCoordinate);

	var tmpFeature = new ol.Feature({
		name: tmpNickName,
		geometry: tmpPoint
	});
	var tmpEventType = 0;
	if(obj.EventType == "突发应急事件")
		tmpEventType = 1;
	else if(obj.EventType == "违法违规")
		tmpEventType = 2;
	else if(obj.EventType == "涉河工程")
		tmpEventType = 3;

	var tmpMsg = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + tmpNickName + "】</font></label><br/></div>" +
		"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">事件类型：</font>" + obj.EventType + "</label><br/>" +
		"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">紧急程度：</font>" + obj.EmergencyType + "</label><br/>" +
		"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">发布人：</font>" + obj.UploadUserNickName + "</label><br/>" +
		"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">发布时间度：</font>" + obj.UploadTime + "</label><br/>" +
		"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">事件描述：</font>" + obj.Content + "</label><br/>" ;

	tmpFeature.set("ftype", "GYEventLayer", false);
	tmpFeature.set("feventtype", tmpEventType, false);
	tmpFeature.set("nickname", tmpNickName, false);
	tmpFeature.set("msg", tmpMsg, false);

	GYEventLayerVectorSource.addFeature(tmpFeature);
}

var m_FocusPointOverlay_Red = null;

function centerOnMapAndAnim(lng, lat) {
	var tmpLng = parseFloat(lng);
	var tmpLat = parseFloat(lat);

	var tmpPoint = ol.proj.transform([tmpLng, tmpLat], 'EPSG:4326', 'EPSG:3857');
	mapView.setCenter(tmpPoint);
	mapView.setZoom(19);

	if(m_FocusPointOverlay_Red == null) {
		var point_div = document.createElement('div');
		point_div.className = "css_map_focusAnimation";
		m_FocusPointOverlay_Red = new ol.Overlay({
			element: point_div,
			positioning: 'center-center'
		});
		
		point_div.onclick = function() {
			m_FocusPointOverlay_Red.setPosition(undefined);	
			return true;
		}
		M_map.addOverlay(m_FocusPointOverlay_Red);
		
	}
	m_FocusPointOverlay_Red.setPosition(tmpPoint);
}

function gotoHome() {
	mapView.setCenter(m_DefaultMapCenter);
	mapView.setZoom(14);
}

function zoomToExtent(extend) {
	var tmpWidth = ol.extent.getWidth(extend);
	if(tmpWidth == 0)
		mapView.setCenter(ol.extent.getCenter(extend));
	else {
		M_map.getView().fit(extend, M_map.getSize());
	}
}

function JudgeClassLevel(speed)
{
	var tmpClass = 0;
			if(speed>=61.3)
			tmpClass = 18;
		else if(speed>=56.1)
			tmpClass = 17;	
		else if(speed>=51)
			tmpClass = 16;
		else if(speed>=46.2)
			tmpClass = 15;
		else if(speed>=41.5)
			tmpClass = 14;
		else if(speed>=37)
			tmpClass = 13;
		else if(speed>=32.7)
			tmpClass = 12;
		else if(speed>=28.5)
			tmpClass = 11;
			else if(speed>=24.5)
			tmpClass = 10;
			else if(speed>=20.8)
			tmpClass = 9;
			else if(speed>=17.2)
			tmpClass = 8;
			else if(speed>=13.9)
			tmpClass = 7;
			else if(speed>=10.8)
			tmpClass = 6;
	return tmpClass;
}

function JudgeTyphoonName(speed)
{
	var tmpClass = "";
			if(speed>=51)
			tmpClass = "超强台风";
		else if(speed>=41.5)
			tmpClass = "强台风";	
		else if(speed>=32.7)
			tmpClass = "台风";
		else if(speed>=24.5)
			tmpClass = "强热带风暴";
		else if(speed>=17.2)
			tmpClass = "热带风暴";
		else if(speed>=10.8)
			tmpClass = "热带低压";
	return tmpClass;
}

function JudgeTyphoonNameAndLevel(speed)
{
	var tmpClass = "";
	var tmpLevel = 0;
			if(speed>=51)
		{
			tmpClass = "超强台风";
			tmpLevel = 6;
		}	
		else if(speed>=41.5)
		{	
			tmpClass = "强台风";
			tmpLevel = 5;
	}
		else if(speed>=32.7)
		{	
			tmpClass = "台风";
			tmpLevel = 4;
	}
		else if(speed>=24.5)
		{	
			tmpClass = "强热带风暴";
			tmpLevel = 3;			
	}
		else if(speed>=17.2)
		{	
			tmpClass = "热带风暴";
			tmpLevel = 2;
	}
		else if(speed>=10.8)
		{	
			tmpClass = "热带低压";
		tmpLevel = 1;
	}
	return [tmpClass, tmpLevel];
}

/**
 * 添加台风路径数据
 * @param {Object} dataArray
 */
function addTyphoonData(dataArray) {
	TyphoonLayerVectorSource.clear();
	TyphoonTrackLayerVectorSource.clear();
	var lstring = [];
	var wgs84Sphere = new ol.Sphere(6378137);
	for(var i in dataArray) {
		var tmpObj = dataArray[i];

		var tmpX = parseFloat(tmpObj.LONGITUDE),
			tmpY = parseFloat(tmpObj.LATITUDE),
			tmpSpeed = parseFloat(tmpObj.WIND);
		var tmpTimeStr = tmpObj.ISSUEDATE;
		if(tmpTimeStr != "") {
			var tmpI01 = tmpTimeStr.indexOf('(');
			var tmpI02 = tmpTimeStr.indexOf(')');
			if(tmpI01 >= 0 && tmpI02 >= 0) {
				var tmpStr = tmpTimeStr.substring(tmpI01 + 1, tmpI02);
				var tmpTime01 = new Date(parseInt(tmpStr));
				tmpTimeStr = tmpTime01.Format("yyyy-MM-dd HH:mm:ss");
			}
		}
		var tmpMsg = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + tmpTimeStr + "】</font></label><br/></div>" +
			"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">经度：</font>" + tmpX + "°</label><br/>" +
			"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">纬度：</font>" + tmpY + "°</label><br/>" +
			"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">风速：</font>" + tmpSpeed + "m/s</label><br/>" +
			"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">中心气压：</font>" + tmpObj.AIRPRESSURE + "百帕</label><br/>" +
			"<label style=\"font-size:16px;\"><font color=\"#01FFFF\">移动速度：</font>" + tmpObj.MOVESPEED + "km/h</label><br/>";
		var tmpClass = JudgeClassLevel(tmpSpeed);
		var tmpTpTypes = JudgeTyphoonNameAndLevel(tmpSpeed);
		tmpMsg +="<label style=\"font-size:16px;\"><font color=\"#01FFFF\">风速等级：</font>" + tmpClass + "级</label><br/>";
		tmpMsg +="<label style=\"font-size:16px;\"><font color=\"#01FFFF\">台风种类：</font>" + tmpTpTypes[0] + "</label><br/>";
		var tmpDis = wgs84Sphere.haversineDistance(m_DefaultMapCenterOfWGS84,[tmpX, tmpY]);	
		tmpDis = tmpDis / 1000;
		tmpMsg +="<label style=\"font-size:16px;\"><font color=\"#01FFFF\">距离深圳：</font>" + tmpDis.toFixed(2) + "km</label><br/>";
		
		var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
		var tmpPoint = new ol.geom.Point(tmpCoordinate);
		var tmpFeature = new ol.Feature({
			name: tmpTimeStr,
			geometry: tmpPoint
		});		
		tmpFeature.set("ftype", "TyphoonLayer", false);
		tmpFeature.set("msg", tmpMsg, false);
		tmpFeature.set("tyclass", tmpTpTypes[1], false);
		
		TyphoonLayerVectorSource.addFeature(tmpFeature);
		lstring[i] = tmpCoordinate;
	}
	var tmpLineFeature = new ol.Feature(new ol.geom.LineString(lstring));
	TyphoonTrackLayerVectorSource.addFeature(tmpLineFeature);
	var tmpExtent = TyphoonLayerVectorSource.getExtent();
	zoomToExtent(tmpExtent);
}

initialMap(); //地图初始化
loadStationLayers(); //加载
M_map.addLayer(riverSectionLayer);
riverSectionLayer.setZIndex(3);

refreshUserGPSInfo();
setInterval(refreshUserGPSInfo, 5000); //刷新用户当前定位信息

var ws;
startConnect();
function startConnect()
		{
			try {
				ws = new WebSocket("ws://"+radarServerURL); //连接服务器 
				ws.onopen = function(event) {
					//RequestRadarData();
					ws.send("VERTIFY|XXX");
				};
				ws.onmessage = function(event) {
					var tmpData = event.data;
					var tmpDatas = tmpData.split("|");
					if(tmpDatas != null && tmpDatas.length > 0) {
						var tmpCommand = tmpDatas[0];
						if(tmpCommand == "VERTIFYRETURN") //用户验证返回
						{
							if(tmpDatas[1] == "true") //登入成功
							{
								startRequestRadarData();
							} else {
								swal("用户登入失败");
							}
						}
						else if(tmpCommand == "RETURNDATA")
						{
							if(tmpDatas[1] == "true")
							{
								var tmpDataStr = tmpDatas[2];
								
								RadarTrackLayerVectorSource.clear();
								var tmpStrs01 = tmpDataStr.split(';');
								if(tmpStrs01!=undefined && tmpStrs01.length>0)
								{
									for(var tmpI01 in tmpStrs01){
										var tmpStr001 = tmpStrs01[tmpI01];
										if(tmpStr001.length>0){
											var tmpStrs02 = tmpStr001.split(',');
											if(tmpStrs02!=undefined && tmpStrs02.length>4)
											{
												var tmpID = tmpStrs02[0];
												var tmpType = tmpStrs02[1];
												if(tmpType == "1"){
													if(tmpID.length>2)
														tmpID = tmpID.substring(tmpID.length-2);
													var tmpX = parseFloat(tmpStrs02[3]);
													var tmpY = parseFloat(tmpStrs02[4]);
													
													var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
													var tmpPoint = new ol.geom.Point(tmpCoordinate);
												
													var tmpFeature = new ol.Feature({
														name: tmpID,
														geometry: tmpPoint
													});
													tmpFeature.set("nickname", tmpID, false);
													tmpFeature.set("msg", tmpID, false);
												
													RadarTrackLayerVectorSource.addFeature(tmpFeature);	
												}
											}
										}
									}
								}
							}
						}
					}
				};
				ws.onclose = function(event) {
					swal("已经与服务器断开连接\r\n当前连接状态：" + this.readyState);
					if(m_RequestRadarDataInterval!=null)
				clearInterval(m_RequestRadarDataInterval);
				};
				ws.onerror = function(event) {
					swal("WebSocket异常！");
					if(m_RequestRadarDataInterval!=null)
				clearInterval(m_RequestRadarDataInterval);
				};
			} catch(ex) {
				swal(ex.message);
				if(m_RequestRadarDataInterval!=null)
				clearInterval(m_RequestRadarDataInterval);
			}
		}

		function RequestRadarData() {
			ws.send("GETRADARDATA|");
		}
		
		var m_RequestRadarDataInterval = null;
		
		function startRequestRadarData()
		{
			RequestRadarData();
			if(m_RequestRadarDataInterval!=null)
				clearInterval(m_RequestRadarDataInterval);
			m_RequestRadarDataInterval = setInterval(RequestRadarData,1000);//10s一次
		}
		
		
		//查询某用户GPS轨迹
		function queryGPSLine(queryUser, nickName, queryStartTime, queryEndTime)
		{
			clearGPSTrackLayers();
			var tmpRequetURL = ServerURL + "ServiceHandler/BigDataHandler.ashx?method=getusergpsbytime"
					+"&username="+queryUser+"&starttime="+queryStartTime+"&endtime="+queryEndTime;
			$.ajax({
				type: "GET",
				url: tmpRequetURL,
				async: true,
				dataType: "json",
				success: function(joResult) {
					if(joResult.success == true) {
						var tmpObjs = joResult.msg;
						var tmpCount = tmpObjs.length;
						if(tmpCount > 1)
						{						
							var tmpTid = 0;											
							var lstring = [];
							var tmpStartI = 0;
							for(var i = 0; i < tmpCount; i++) {
								var tmpObj = tmpObjs[i];
								var tmpCoordinate = ol.proj.transform([parseFloat(tmpObj.longitude), parseFloat(tmpObj.latitude)], 'EPSG:4326', 'EPSG:3857');
								lstring[i]=tmpCoordinate;
								var tmpTimeStr = tmpObj.gpsTime;
								var tmpSpeed = parseFloat(tmpObj.speed);
								var tmpaccuracy = parseFloat(tmpObj.accuracy);
								if(tmpObj.isfirst == "1")//是起点
								{
									if(i - tmpStartI >1)
									{
										var tmpLsString = [];
										var tmpTid01 = 0;
										for(var i2 = tmpStartI; i2 < i; i2++)
										{
											tmpLsString[tmpTid01++] = lstring[i2];
										}										
										gpsTrackLineFeatures.push(new ol.Feature(new ol.geom.LineString(tmpLsString)));
									}
									tmpStartI = i;
								}
								//添加点
								var	tmpMsg = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【"+nickName+"】</font></label><br/></div>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">时间：</font>"+tmpTimeStr+"</label><br/>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">经度：</font>"+tmpObj.longitude+"</label><br/>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">纬度：</font>"+tmpObj.latitude+"</label><br/>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">速度：</font>"+tmpSpeed.toFixed(2)+"m/s</label><br/>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">精度：</font>"+tmpaccuracy.toFixed(2)+"m</label><br/>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">地址：</font>"+tmpObj.address+"</label><br/>"
											+ "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">备注：</font>"+tmpObj.remark+"</label><br/>";
												
								var tmpPoint = new ol.geom.Point(tmpCoordinate);
								var tmpGPSMarker = new ol.Feature({
								    name: tmpMsg,
								    geometry: tmpPoint
								});
								tmpGPSMarker.set("ftype","gpslayer",false);
								tmpGPSMarker.set("msg",tmpMsg,false);
								gpsTrackPointsVectorSource.addFeature(tmpGPSMarker);
							}
							//gpsTrackLineFeatures.push(new ol.Feature(new ol.geom.LineString(lstring)));	
//							if(lstring.length>1)
//							{
//								gpsTrackLineFeatures.push(new ol.Feature(new ol.geom.LineString(lstring)));	
//							}
							if(tmpCount - tmpStartI >1)
									{
										var tmpLsString = [];
										var tmpTid01 = 0;
										for(var i2 = tmpStartI; i2 < i; i2++)
										{
											tmpLsString[tmpTid01++] = lstring[i2];
										}										
										gpsTrackLineFeatures.push(new ol.Feature(new ol.geom.LineString(tmpLsString)));
									}
							
							//设置地图缩放中心
							var tmpExtent = GPSTrackLayer.getSource().getExtent();
							zoomToExtent(tmpExtent);
						}
						else{
							swal('该时间段内没有定位信息.');
						}
					} else {
						swal(joResult.msg);
					}
				},
				error: function(XMLHttpRequest, textStatus, errorThrown) {
					//swal("未知错误");
				}
			});	
		}
		
		//清空历史轨迹数据
		function clearGPSTrackLayers()
		{
			//gpsLayerVectorSource.clear(true);
			gpsTrackPointsVectorSource.clear(true);
			gpsTrackLineFeatures.clear(true);
		}
		
		function queryusertrack(username, nickname)
	   {
	   		var newDate = new Date();
	   		var tmpEndStr = newDate.Format("yyyy-MM-dd-HH-mm-ss");
	   		var preDate = new Date(newDate.getTime() - 60*60*1000);//1个小时
	   		var tmpStartStr = preDate.Format("yyyy-MM-dd-HH-mm-ss");
	   		
	   		startqueryusertrack(username, nickname, tmpStartStr, tmpEndStr);
	   }