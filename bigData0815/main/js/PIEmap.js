let entityCollection;
let viewer;
let layer_basemapTxt;
let tmsImageryProvider_basemapTxt;

createEarthModule().then(function () {
    viewer = new Earth.Viewer('mapContainer', {

        sceneMode: Earth.SceneMode.SCENE3D, //Earth.SceneMode.SCENE2D
        center: [114.181236, 22.590030], //初始中心点
        zoom: 12, //初始层级
        cameraSmooth: false, //是否开启相机缓冲效果
        // projection: new Earth.Projection.WebMercator(),
        imageryProvider: new Earth.TileMapServiceImageryProvider({
            url: 'tiles/basemap/L{z}/{y}-{x}.png',
            // tilingScheme: new Earth.EPSG3857TilingScheme(),
            // tilingScheme: tilingScheme,
        }),
    });

    viewer.scene.skyBox.show = false;
    entityCollection = viewer.entities;


    let tmsImageryProvider_nightmap = new Earth.TileMapServiceImageryProvider({
        url: 'tiles/nightmap/{z}/{x}/{y}.png'
    })
    const layer_nightmap = viewer.imageryLayers.addImageryProvider(tmsImageryProvider_nightmap, 0);

    // const tmsImageryProvider_basemap = new Earth.TileMapServiceImageryProvider({
    //     url: 'tiles/basemap/L{z}/{y}-{x}.png',
    // });
    // const layer_basemap = viewer.imageryLayers.addImageryProvider(tmsImageryProvider_basemap);

    tmsImageryProvider_basemapTxt = new Earth.TileMapServiceImageryProvider({
        url: 'tiles/basemapTxt/L{z}/{y}-{x}.png'
    })
    layer_basemapTxt = viewer.imageryLayers.addImageryProvider(tmsImageryProvider_basemapTxt);

    // 原来index.html中的button方法，已经整合到page_home中去了，无用，待删除  clearLabelBox暂时还有用
    // （已经转移clearLabelBox）
    // clearLabelBox = function () {
    //     for (var i in entityCollection._entities) {
    //         entityCollection._entities[i].labelBox.show = false;
    //     }
    // }

    // stationLayerShow = function () {
    //     for (var i in entityCollection._entities) {
    //         // console.log(i)
    //         if (entityCollection._entities[i].layer === 'stationLayer') {
    //             entityCollection._entities[i].show = !entityCollection._entities[i].show;
    //             entityCollection._entities[i].labelBox.show = false;

    //         }
    //     }
    // }

    // riverSectionLayerShow = function () {
    //     for (var i in entityCollection._entities) {
    //         // console.log(entityCollection._entities[i].layer)
    //         if (entityCollection._entities[i].layer === 'riverSectionLayer') {
    //             entityCollection._entities[i].show = !entityCollection._entities[i].show;
    //             entityCollection._entities[i].labelBox.show = false;

    //         }
    //     }
    // }

    // userGPSLayerShow = function () {
    //     for (var i in entityCollection._entities) {
    //         // console.log(i)
    //         if (entityCollection._entities[i].layer === 'userGPSLayer') {
    //             entityCollection._entities[i].show = !entityCollection._entities[i].show;
    //             entityCollection._entities[i].labelBox.show = false;

    //         }
    //     }
    // }

    // GYEventLayerShow = function () {
    //     for (var i in entityCollection._entities) {
    //         // console.log(i)
    //         if (entityCollection._entities[i].layer === 'GYEventLayer') {
    //             entityCollection._entities[i].show = !entityCollection._entities[i].show;
    //             entityCollection._entities[i].labelBox.show = false;

    //         }
    //     }
    // }

    // FlyTargetLocation = function () {
    //     viewer.camera.flyTo({
    //         destination: Earth.Cartesian3.fromDegrees(114.187336, 22.595030, 3000.0),

    //         orientation: {
    //             heading: Earth.Math.toRadians(0),
    //             pitch: Earth.Math.toRadians(0),
    //             roll: 0.0,
    //         },
    //         duration: 3,
    //         complete: () => {
    //             console.log('complete');
    //         },
    //     });
    // }

    // ChangeSceneMode = function () {
    //     if (viewer.scene.mode === 3) {
    //         viewer.scene.mode = 2;
    //     } else {
    //         viewer.scene.mode = 3;
    //     }
    // }

    // zoomin zoomout button废除了，所有对应的方法也待删除
    // document.querySelector('.zoomIn').addEventListener('click', () => viewer.camera.zoomIn());
    // document.querySelector('.zoomOut').addEventListener('click', () => viewer.camera.zoomOut());

    // const measureLength = document.querySelector('.measureLength');
    // const clearMeasureLength = document.querySelector('.clearMeasureLength');

    // measureLength.addEventListener('click', function () {
    //     viewer.scene.globe.setGlobeTool(Earth.GlobeToolType.MeasureLength);
    //     const measureTool = viewer.scene.globe.getGlobeTool();
    //     console.log(measureTool);
    //     measureLength.innerHTML = '测距中... （鼠标左键增加节点，鼠标右键结束测距）';
    //     measureTool.getResultEvent().addEventListener(() => {
    //         measureLength.innerHTML = '重新测距';
    //         clearMeasureLength.style.display = 'inline-block';
    //     });

    //     clearMeasureLength.addEventListener('click', () => {
    //         viewer.scene.globe.setGlobeTool(Earth.GlobeToolType.Pan);
    //         measureLength.innerHTML = '测距';
    //         clearMeasureLength.style.display = 'none';
    //     });
    // })

    // function loadData(callback) {
    // 	fetch('data.json')
    // 		.then(response => response.json())
    // 		.then(jsonData => {
    // 			callback(jsonData);
    // 		})
    // }



    /**
    * 刷新用户定位信息
    */
    function refreshUserGPSInfo() {
        $.ajax({
            url: ServerURL + "/ServiceHandler/BigDataHandler.ashx?method=getuserfinalgps&projectid=" + CurrentProjectID,
            type: "GET",
            dataType: "json",
            success: function (joResult) {
                if (joResult.success == true) {
                    var tmpObjs = joResult.data;
                    // if(UserGPSLayerVectorSource == null) {
                    // 	UserGPSLayerVectorSource = new ol.source.Vector();
                    // 	UserGPSLayer.setSource(UserGPSLayerVectorSource);
                    // } else
                    // 	UserGPSLayerVectorSource.clear(true);

                    var tmpTotalUsersCount = tmpObjs.length;
                    var tmpOnlineUsersCount = 0;
                    for (var i in tmpObjs) {
                        var tmpObj = tmpObjs[i];
                        var tmpX = parseFloat(tmpObj.longitude),
                            tmpY = parseFloat(tmpObj.latitude);
                        if (tmpX > 1 && tmpY > 1) {
                            var dateNow = new Date();
                            var tmpTime01 = Date.parse(tmpObj.gpsTime);
                            if (dateNow - tmpTime01 > 300000) //5分钟之内都表示在线  此处修改，让所有人员上线
                            {
                                // tmpIsOnline = "true";
                                tmpOnlineUsersCount++;

                                var tmpSpeed = parseFloat(tmpObj.speed);
                                var tmpaccuracy = parseFloat(tmpObj.accuracy);
                                var tmpNickName = tmpObj.RealName || '郑地秀';
                                var tmpSpeed = parseFloat(tmpObj.speed);
                                var tmpaccuracy = parseFloat(tmpObj.accuracy);

                                const entity = new Earth.Entity({
                                    name: 'userGPS',
                                    show: true,
                                    layer: 'userGPSLayer',
                                    position: Earth.Cartesian3.fromDegrees(tmpX, tmpY),
                                    point: new Earth.Point({
                                        show: false,
                                        pixelSize: 1,
                                        color: Earth.Color.BLUE.withAlpha(1),
                                        heightReference: Earth.HeightReference.CLAMP_TO_GROUND, //贴地
                                        outlineColor: undefined, //不支持
                                        outlineWidth: undefined, //不支持
                                        distanceDisplayCondition: undefined, //暂不支持
                                    }),
                                    billboard: {
                                        show: true,
                                        image: './images/user01.png',
                                        scale: 1,
                                        horizontalOrigin: Earth.HorizontalOrigin.CENTER,
                                        verticalOrigin: Earth.VerticalOrigin.CENTER
                                    },
                                    label: {
                                        show: true,
                                        text: tmpNickName,
                                        font: '20px',
                                        fillColor: Earth.Color.RED.withAlpha(1),//可设置透明度
                                        pixelOffset: { x: 0, y: 35 },
                                        backgroundColor: Earth.Color.RED,
                                    },
                                    labelBox: {
                                        show: false,
                                        type: Earth.LabelBoxType.box10,
                                        scale: 0.8,
                                        offset: { x: -75, y: 35 },
                                        maxHeight: 20000000,
                                        style: {
                                            html: `
                                                    <div class="userGPSPop">
                                                        <div class="userGPSPopHeader">
                                                            <font>【${tmpNickName}】</font>
                                                        </div>
                                                        <div class="userGPSPopBody">
                                                            <table>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">时间:</td>
                                                                    <td>${tmpObj.gpsTime}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">经度:</td>
                                                                    <td>${tmpX.toFixed(3)}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">纬度:</td>
                                                                    <td>${tmpY.toFixed(3)}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">速度:</td>
                                                                    <td>${tmpSpeed.toFixed(2)}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">精度:</td>
                                                                    <td>${tmpaccuracy.toFixed(2)}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">地址:</td>
                                                                    <td>${tmpObj.address}</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="userGPSPopFirstColTd">备注:</td>
                                                                    <td>${tmpObj.remark}</td>
                                                                </tr>    
                                                            </table>
                                                        </div>
                                                    </div>`,
                                            color: 'black',
                                            backgroundColor: '#0000004D'
                                        },
                                    },

                                })
                                // console.log(i)
                                // console.log(tmpNickName)

                                entityCollection.add(entity);
                            }
                        }
                    }
                    // 暂时没有gy-LABEL03 加入page_home后有了gy-LABEL03   20230420
                    $('#gy_Label03').text(tmpOnlineUsersCount + "/" + tmpTotalUsersCount);
                }
            }
        })
    }


    // 刷新设施位置信息
    $.ajax({
        url: ServerURL + "/ServiceHandler/BigDataHandler.ashx?method=getfacilitiesinfobyprojectid&projectid=" + CurrentProjectID + "&token=" + defaultToken,
        type: "GET",
        dataType: "json",
        success: function (joResult) {
            if (joResult.success == true) {
                var tmpObjs = joResult.data;
                // StationLayerVectorSource.clear();
                var tmpObjs = joResult.data;

                for (var i in tmpObjs) {
                    tmpObj = tmpObjs[i];
                    var tmpX = parseFloat(tmpObj.X),
                        tmpY = parseFloat(tmpObj.Y);
                    // tmpXY = translate.translatePoint([114.191, 22.592]);
                    // tmpX = tmpXY[0]
                    // tmpY = tmpXY[1]
                    var tmpNickName = tmpObj.Name;
                    var tmpStationType = tmpObj.FacilitiesType;
                    var tmpMsg = tmpObj.Info;
                    //1、创建贴地点
                    const entity = new Earth.Entity({
                        name: 'station',
                        show: true,
                        layer: 'stationLayer',
                        // zIndex: 10,
                        position: Earth.Cartesian3.fromDegrees(tmpX, tmpY), //经度，纬度，高度
                        point: new Earth.Point({
                            show: false,
                            pixelSize: 1,
                            color: Earth.Color.BLUE.withAlpha(1),
                            heightReference: Earth.HeightReference.CLAMP_TO_GROUND, //贴地
                            outlineColor: undefined, //不支持
                            outlineWidth: undefined, //不支持
                            distanceDisplayCondition: undefined, //暂不支持
                        }),
                        billboard: {
                            show: true,
                            image: 'images/' + tmpStationType + '.png',
                            scale: 1,
                            horizontalOrigin: Earth.HorizontalOrigin.CENTER,
                            verticalOrigin: Earth.VerticalOrigin.CENTER
                        },
                        // label: {
                        //   show: true,
                        //   text: viewer.camera.zoom < 10 ? "" : tmpNickName,
                        //   fillColor: Earth.Color.BLUE.withAlpha(0.8),//可设置透明度
                        // },
                        labelBox: {
                            show: false,
                            type: Earth.LabelBoxType.box10,
                            scale: 0.8,
                            offset: { x: -75, y: 25 },
                            maxHeight: 20000000,
                            style: {
                                html: `
                                          <div class="stationPop">
                                              <div class="stationPopHeader">
                                                  <font>【${tmpNickName}】</font>
                                              </div>
                                              <div class="stationPopBody">
                                                  <table>
                                                      <tr>
                                                          <td class="stationPopFirstColTd">设备类型 :</td>
                                                          <td>${tmpStationType}</td>
                                                      </tr>
                                                      <tr>
                                                          <td class="stationPopFirstColTd">设施信息 :</td>
                                                          <td>${tmpMsg}</td>
                                                      </tr>
                                                  </table>
                                              </div>
                                          </div>`,
                                color: 'black',
                                backgroundColor: '#0000004D'
                            },
                        },

                    });
                    // console.log(`设置labelBox.show为false之前,labelBox的值${entity.labelBox.show}`)
                    // entity.labelBox.show = false
                    // console.log(`entity.show:${entity.show}`)
                    // console.log(`entity.labelBox.show:${entity.labelBox.show}`)
                    //3、添加
                    entityCollection.add(entity);
                    // console.log(`entityCollection._entities[${i}].show:${entityCollection._entities[i].show}`)
                }
            }
        }
    });




    // 加载geojson数据 后续没使用过riverzone变量
    // const riverzone = viewer.dataSources.add(
    // 	Earth.GeoJsonDataSource.load('./riverzone.geojson', {
    // 		stroke: Earth.Color.RED,
    // 		strokeWidth: 2,
    // 		fill: Earth.Color.BLACK.withAlpha(0.2),
    // 		// fill: "rgba(0, 128, 128, 0.2)",
    // 	})
    // );
    // console.log(line.entities)

    // const riversection = viewer.dataSources.add(
    //   Earth.GeoJsonDataSource.load('./riversection.geojson', {
    //     stroke: Earth.Color.RED,
    //     strokeWidth: 1,
    //     fill: Earth.Color.BLACK.withAlpha(0.2),
    //     // fill: "rgba(0, 128, 128, 0.2)",
    //   })
    // );
    // console.log(riversection.entities)



    const handler = new Earth.ScreenSpaceEventHandler(viewer.scene.canvas);
    handler.setInputAction(function (movement) {
        const pickObj = viewer.scene.pick(movement.position);
        // console.log(pickObj.id)
        // 还没有drillPick函数
        // const pickObjs = viewer.scene.drillPick(movement.position);

        // pickObj && alert('拾取成功：' + pickObj.name);
        if (pickObj && pickObj.show) {
            // console.log(pickObj.labelBox.show)
            pickObj.labelBox.show = !pickObj.labelBox.show;
            if (pickObj.polygon) {
                if (pickObj.labelBox.show) {
                    pickObj.polygon.color = Earth.Color.fromBytes(255, 0, 0, 200);
                }
                else {
                    pickObj.polygon.color = Earth.Color.fromBytes(0, 0, 255, 200);
                }
            }


        }
    }, Earth.ScreenSpaceEventType.LEFT_CLICK);


    // 一些更新地图的函数的调用
    refreshUserGPSInfo()
    refreshSectionElvData(28181)
    // refreshMapRiverShow();
    MyrefreshGYEventsInfo()

    setTimeout(function() {
        // 解决labelBox.show false的初始化bug
        // for (var i in entityCollection._entities) {
        //     entityCollection._entities[i].labelBox.show = false;
        // }
        // console.log('最初的clearLabelBox 2s')
        
        $("#mapBottomControlBtnClearLabelBox").click() 
        // 初始化地图高度和视角
        $("#mapBottomControlBtnTHome").click() 

      }, 2000);


});// createEarthModule结束 new Earth




function getPolygonCentroid(points) {
    let sumX = 0;
    let sumY = 0;
    let sumArea = 0;
    for (let i = 0; i < points.length; i++) {
        let point1 = points[i];
        let point2 = i === points.length - 1 ? points[0] : points[i + 1];
        let area = point1[0] * point2[1] - point2[0] * point1[1];
        sumX += (point1[0] + point2[0]) * area;
        sumY += (point1[1] + point2[1]) * area;
        sumArea += area;
    }
    let x = sumX / (3 * sumArea);
    let y = sumY / (3 * sumArea);
    return [x, y];
}

// var m_RiverSectionVDataArray = [];
// /**
//  * 刷新断面的高程及水面高程数据
//  * @param {Object} planID
//  */
// function refreshSectionElvData(planID) {
//     RunWebServiceJSON(true, "getTSDBLeveldata", "/pms/Base", "/doreaddata/getdata",
//         "guid#timeindex", String(planID) + "#-1", function (data) {
//             var jqueryObj = $(data);
//             var message = jqueryObj.children();
//             var text = message.text();
//             var tmpObjs = eval("(" + text + ")");
//             if (tmpObjs != null && tmpObjs.length > 0) {
//                 var tmpArray = [], tmpArray2 = [], tmpArray3 = [], tmpArray4 = [];
//                 for (var i in tmpObjs) {
//                     var tmpObj = tmpObjs[i];
//                     tmpArray[i] = parseFloat(tmpObj.V);
//                     tmpArray2[i] = parseFloat(tmpObj.MAXV);
//                     tmpArray3[i] = parseFloat(tmpObj.MINV);
//                     tmpArray4[i] = tmpArray[i] - tmpArray2[i];
//                     if (tmpArray4[i] < 0)
//                         tmpArray4[i] = 0;
//                 }
//                 //initialSectionChartControl([tmpArray,tmpArray2,tmpArray3]);	

//                 m_RiverSectionVDataArray = [tmpArray, tmpArray2, tmpArray3, tmpArray4];
//                 // console.log(m_RiverSectionVDataArray)


//             }
//         });
// }

function refreshMapRiverShow() {
    fetch('./riversection.geojson')
        .then(response => response.json())
        .then(data => {
            // console.log(data.features); // data为JSON对象
            var tmpObjs = data.features;

            for (var i in tmpObjs) {
                tmpObj = tmpObjs[i];
                // console.log(tmpObj)
                tmpNickName = tmpObj.properties.name;
                var tmpCoordinates = [];
                tmpCoordinatesbigArray = tmpObj.geometry.coordinates[0];
                tmpCoordinatesbigArray.forEach(function (arr) {
                    tmpCoordinates = tmpCoordinates.concat(arr);
                });

                // 通过自己定义的函数 getPolygonCentroid获取polygon的重心点
                polygonCentroid = getPolygonCentroid(tmpCoordinatesbigArray)
                // console.log(polygonCentroid);

                // --80写死了，待处理-- 读取后台数据，更新m_RiverSectionVDataArray
                var tmpRiverSectionWaterLevel = m_RiverSectionVDataArray[0][i].toFixed(2);
                var tmpRiverSectionWaterDepth = m_RiverSectionVDataArray[3][i].toFixed(2);

                const polygon = new Earth.Entity({
                    name: tmpNickName,
                    show: true,
                    layer: 'riverSectionLayer',
                    // zIndex: 5,
                    position: Earth.Cartesian3.fromDegrees(polygonCentroid[0], polygonCentroid[1]), //经度，纬度，高度

                    polygon: new Earth.Polygon({
                        show: true,
                        hierarchy: Earth.Cartesian3.fromDegreesArray(tmpCoordinates),
                        color: Earth.Color.fromBytes(0, 0, 255, 200),
                        heightReference: Earth.HeightReference.CLAMP_TO_GROUND, //贴地
                        outline: true, //好像暂时不支持
                        outlineColor: Earth.Color.RED, //好像暂时不支持
                    }),

                    labelBox: {
                        show: false,
                        type: Earth.LabelBoxType.box10,
                        scale: 0.8,
                        offset: { x: -85, y: 25 },
                        maxHeight: 20000000,
                        style: {
                            html: `
                                <div class="riverSectionPop">
                                    <div class="riverSectionPopHeader">
                                        <font>【${tmpNickName}】</font>
                                    </div>
                                    <div class="riverSectionPopBody">
                                        <table>
                                            <tr>
                                            <td class="riverSectionPopFirstColTd">水深:</td>
                                                <td>${tmpRiverSectionWaterDepth} m</td>
                                            </tr>
                                            <tr>
                                            <td class="riverSectionPopFirstColTd">水位:</td>
                                                <td>${tmpRiverSectionWaterLevel} m</td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>`,
                            color: 'black',
                            backgroundColor: '#0000004D'
                        },
                    },

                });
                entityCollection.add(polygon);
            }
        });
}

/**
     * 获取最近的事件 My文件名需要重新取名
     */
function MyrefreshGYEventsInfo() {
    m_LastGYEventQueryTimeStr = (new Date()).Format("yyyy/MM/dd HH:mm:ss");
    var dataObj = {
        method: "geteventslastinfo",
        projectid: CurrentProjectID,
        count: "10"
    }
    $.ajax({
        type: "post",
        url: ServerURL + "/ServiceHandler/BigDataHandler.ashx",
        async: true,
        data: dataObj,
        dataType: "json",
        success: function (joResult) {
            if (joResult.success == true) {
                var tmpObjs = joResult.data;
                // var tmpTotalHTML = '<ul class="clearfix">';
                for (var tmpI in tmpObjs) {
                    var tmpObj = tmpObjs[tmpI];
                    // var tmpLClass = "lableRC_Blue";
                    // if(tmpObj.EmergencyType.indexOf("重要")>=0)
                    // 	tmpLClass = "lableRC_Yellow";
                    // else if(tmpObj.EmergencyType.indexOf("十分重要")>=0)
                    // 	tmpLClass = "lableRC_Red";
                    var tmpStr = tmpObj.EventName + " " + tmpObj.UploadTime;
                    if (tmpStr.length > 20)
                        tmpStr = tmpStr.substring(0, 20);
                    if (tmpObj.X == '')
                        tmpObj.X = '114.1817251111';
                    if (tmpObj.Y == '')
                        tmpObj.Y = '22.5951098889';

                    addGYEventPoint(tmpObj.X, tmpObj.Y, tmpObj);
                    // tmpTotalHTML+='<li class="clearfix" onClick="centerOnMapAndAnim('+tmpObj.X+','+tmpObj.Y+');"><label class="'
                    // 	+tmpLClass+'">'+tmpObj.EmergencyType+'</label> '+tmpObj.EventType
                    // 	+'|'+tmpStr+'</li>';
                }
                // tmpTotalHTML += "</ul>";
                // $('#patrolListDiv').html(tmpTotalHTML);
                // patrolListAnimation();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

        }
    });
}

function addGYEventPoint(lng, lat, obj) {
    var tmpX = parseFloat(lng),
        tmpY = parseFloat(lat);

    // var tmpCoordinate = ol.proj.transform([tmpX, tmpY], 'EPSG:4326', 'EPSG:3857');
    var tmpNickName = obj.EventName;
    // var tmpPoint = new ol.geom.Point(tmpCoordinate);

    // var tmpFeature = new ol.Feature({
    // 	name: tmpNickName,
    // 	geometry: tmpPoint
    // });
    var tmpEventType = 0;
    if (obj.EventType == "突发应急事件")
        tmpEventType = 1;
    else if (obj.EventType == "违法违规")
        tmpEventType = 2;
    else if (obj.EventType == "涉河工程")
        tmpEventType = 3;

    var tmpMsg = "<div style=\"margin:0 auto;text-align:center;\"><label style=\"font-size:16px;\"><font color=\"#6FFF6F\">【" + tmpNickName + "】</font></label><br/></div>" +
        "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">事件类型：</font>" + obj.EventType + "</label><br/>" +
        "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">紧急程度：</font>" + obj.EmergencyType + "</label><br/>" +
        "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">发布人：</font>" + obj.UploadUserNickName + "</label><br/>" +
        "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">发布时间：</font>" + obj.UploadTime + "</label><br/>" +
        "<label style=\"font-size:16px;\"><font color=\"#01FFFF\">事件描述：</font>" + obj.Content + "</label><br/>";
    const entity = new Earth.Entity({
        name: 'GYEvent',
        show: true,
        layer: 'GYEventLayer',
        position: Earth.Cartesian3.fromDegrees(tmpX, tmpY),
        point: new Earth.Point({
            show: false,
            pixelSize: 1,
            color: Earth.Color.BLUE.withAlpha(1),
            heightReference: Earth.HeightReference.CLAMP_TO_GROUND, //贴地
            outlineColor: undefined, //不支持
            outlineWidth: undefined, //不支持
            distanceDisplayCondition: undefined, //暂不支持
        }),
        billboard: {
            show: true,
            image: `./images/GYEvent_0${tmpEventType}.png`,
            scale: 1,
            horizontalOrigin: Earth.HorizontalOrigin.CENTER,
            verticalOrigin: Earth.VerticalOrigin.CENTER
        },
        // label: {
        //     show: true,
        //     text: tmpNickName,
        //     font: '20px',
        //     fillColor: Earth.Color.RED.withAlpha(1),//可设置透明度
        //     pixelOffset:{ x: 0, y: 35 },
        //     backgroundColor: Earth.Color.RED,
        // },
        labelBox: {
            show: false,
            type: Earth.LabelBoxType.box10,
            scale: 0.8,
            offset: { x: -75, y: 35 },
            maxHeight: 20000000,
            style: {
                html: `
                        <div class="GYEventPop">
                            <div class="GYEventPopHeader">
                                <font>【${tmpNickName}】</font>
                            </div>
                            <div class="GYEventPopBody">
                                <table>
                                    <tr>
                                        <td class="GYEventPopFirstColTd">事件类型:</td>
                                        <td>${obj.EventType}</td>
                                    </tr>
                                    <tr>
                                        <td class="GYEventPopFirstColTd">紧急程度:</td>
                                        <td>${obj.EmergencyType}</td>
                                    </tr>
                                    <tr>
                                        <td class="GYEventPopFirstColTd">发布人:</td>
                                        <td>${obj.UploadUserNickName}</td>
                                    </tr>
                                    <tr>
                                        <td class="GYEventPopFirstColTd">发布时间:</td>
                                        <td>${obj.UploadTime}</td>
                                    </tr>
                                    <tr>
                                        <td class="GYEventPopFirstColTd">事件描述:</td>
                                        <td>${obj.Content}</td>
                                    </tr>   
                                </table>
                            </div>
                        </div>`,
                color: 'black',
                backgroundColor: '#0000004D'
            },
        },

    })
    // tmpFeature.set("ftype", "GYEventLayer", false);
    // tmpFeature.set("feventtype", tmpEventType, false);
    // tmpFeature.set("nickname", tmpNickName, false);
    // tmpFeature.set("msg", tmpMsg, false);
    entityCollection.add(entity)
    // GYEventLayerVectorSource.addFeature(tmpFeature);
}



// 控制按钮的函数
$(document).ready(function () {
    
    const mapCanvas = document.getElementById("globe");
    const imgDiv = document.getElementById("mapContainerBottomImgDiv");
    
    $("#mapBottomControlBtnfullScreen").click(function (){
        if (mapCanvas.classList.contains("fullscreen")) {
            mapCanvas.classList.remove("fullscreen");
            imgDiv.classList.remove("fullscreen");
            document.getElementById("localtime").classList.remove("fullscreen")
            document.getElementById("localtime2").classList.remove("fullscreen")
            $("#mapBottomControlBtnfullScreen").attr("src", "images/fullscreen_on.png");
            $("#mapBottomControlBtnfullScreen").attr("title", "全屏");

        } else {
            mapCanvas.classList.add("fullscreen");
            imgDiv.classList.add("fullscreen")
            document.getElementById("localtime").classList.add("fullscreen")
            document.getElementById("localtime2").classList.add("fullscreen")
            $("#mapBottomControlBtnfullScreen").attr("src", "images/fullscreen_off.png");
            $("#mapBottomControlBtnfullScreen").attr("title", "退出全屏");

        }
    })


	$("#mapBottomControlBtnHome").click(function () {
		// gotoHome(); 替换原来的openlayer方法，使用新的PIE方法
		viewer.camera.flyTo({
			destination: Earth.Cartesian3.fromDegrees(114.181236, 22.598030, 2800.0),

			orientation: {
				heading: Earth.Math.toRadians(0),
				pitch: Earth.Math.toRadians(0),
				roll: 0.0,
			},
			duration: 1,
			complete: () => {
				console.log('complete');
			},
		});
	});

	// $("#mapBottomControlBtnTurb").click(function () {
	//     refreshTyphoonData();
	// });

	$("#mapBottomControlBtnClearLabelBox").click(function () {
		for (var i in entityCollection._entities) {
			entityCollection._entities[i].labelBox.show = false;
			if (entityCollection._entities[i].polygon) {
				entityCollection._entities[i].polygon.color = Earth.Color.fromBytes(0, 0, 255, 200);
			}
		}
	});

	var m_Layer_User_Vis = true;
	$("#mapBottomControlBtnUser").click(function () {
		if (m_Layer_User_Vis) {
			$("#mapBottomControlBtnUser").attr("src", "images/layer_user_off.png");
			m_Layer_User_Vis = false;
			// clearGPSTrackLayers();
		}
		else {
			$("#mapBottomControlBtnUser").attr("src", "images/layer_user_on.png");
			m_Layer_User_Vis = true;
		}
		// UserGPSLayer.setVisible(m_Layer_User_Vis);
		// GPSTrackLayer.setVisible(m_Layer_User_Vis);
		// GPSTrackPointLayer.setVisible(m_Layer_User_Vis);
		for (var i in entityCollection._entities) {
			// console.log(i)
			if (entityCollection._entities[i].layer === 'userGPSLayer') {
				entityCollection._entities[i].show = !entityCollection._entities[i].show;
				entityCollection._entities[i].labelBox.show = false;
			}
		}

	});
	// var m_Layer_Camera_Vis = true;
	// $("#mapBottomControlBtnCamera").click(function () {
	//     if (m_Layer_Camera_Vis) {
	//         $("#mapBottomControlBtnCamera").attr("src", "images/layer_camera_off.png");
	//         m_Layer_Camera_Vis = false;
	//     }
	//     else {
	//         $("#mapBottomControlBtnCamera").attr("src", "images/layer_camera_on.png");
	//         m_Layer_Camera_Vis = true;
	//     }
	//     CameraLayer.setVisible(m_Layer_Camera_Vis);
	// });

	var m_Layer_SheShi_Vis = true;
	$("#mapBottomControlBtnSheShi").click(function () {
		if (m_Layer_SheShi_Vis) {
			$("#mapBottomControlBtnSheShi").attr("src", "images/layer_sheshi_off.png");
			m_Layer_SheShi_Vis = false;
		}
		else {
			$("#mapBottomControlBtnSheShi").attr("src", "images/layer_sheshi_on.png");
			m_Layer_SheShi_Vis = true;
		}
		// StationLayer.setVisible(m_Layer_SheShi_Vis);
		for (var i in entityCollection._entities) {
			// console.log(entityCollection._entities[i].layer)
			if (entityCollection._entities[i].layer === 'stationLayer') {
				entityCollection._entities[i].show = !entityCollection._entities[i].show;
				entityCollection._entities[i].labelBox.show = false;
			}
		}
	});

	var m_Layer_Event_Vis = true;
	$("#mapBottomControlBtnEvent").click(function () {
		if (m_Layer_Event_Vis) {
			$("#mapBottomControlBtnEvent").attr("src", "images/layer_event_off.png");
			m_Layer_Event_Vis = false;
		}
		else {
			$("#mapBottomControlBtnEvent").attr("src", "images/layer_event_on.png");
			m_Layer_Event_Vis = true;
		}
		// GYEventLayer.setVisible(m_Layer_Event_Vis);
		for (var i in entityCollection._entities) {
			// console.log(i)
			if (entityCollection._entities[i].layer === 'GYEventLayer') {
				entityCollection._entities[i].show = !entityCollection._entities[i].show;
				entityCollection._entities[i].labelBox.show = false;
			}
		}
	});

	var m_Layer_River_ShowMode = 0;
	$("#mapBottomControlBtnRiver").click(function () {
		if (m_Layer_River_ShowMode == 0) {
			$("#mapBottomControlBtnRiver").attr("src", "images/layer_river_off.png");
			m_Layer_River_ShowMode = 1;
		}
		else {
			$("#mapBottomControlBtnRiver").attr("src", "images/layer_river_on.png");
			m_Layer_River_ShowMode = 0;
		}
		// refreshMapRiverShow();
	});

	// var m_Layer_Radar_Vis = true;
	// $("#mapBottomControlBtnRadarMap").click(function () {
	//     if (m_Layer_Radar_Vis) {
	//         $("#mapBottomControlBtnRadarMap").attr("src", "images/layer_radar_off.png");
	//         m_Layer_Radar_Vis = false;
	//     }
	//     else {
	//         $("#mapBottomControlBtnRadarMap").attr("src", "images/layer_radar_on.png");
	//         m_Layer_Radar_Vis = true;
	//     }
	//     RadarTrackLayer.setVisible(m_Layer_Radar_Vis);
	// });

	var m_Layer_Sal_Vis = true;
	$("#mapBottomControlBtnSalMap").click(function () {
		if (m_Layer_Sal_Vis) {
			$("#mapBottomControlBtnSalMap").attr("src", "images/layer_salmap_off.png");
			m_Layer_Sal_Vis = false;
			viewer.imageryLayers.remove(layer_basemapTxt)
		}
		else {
			$("#mapBottomControlBtnSalMap").attr("src", "images/layer_salmap_on.png");
			m_Layer_Sal_Vis = true;
			layer_basemapTxt = viewer.imageryLayers.addImageryProvider(tmsImageryProvider_basemapTxt);
		}
		// TianDitu_Val_Layer.setVisible(m_Layer_Sal_Vis);
	});

	// 暂时没有使用 riverSection颜色
	// function refreshMapRiverShow()
	// {
	// 	if(m_RiverSectionVDataArray!=null && m_RiverSectionVDataArray.length>0)
	// 	{
	// 		if(m_Layer_River_ShowMode == 0)//显示水位
	// 		{
	// 			DefaultMapTipParaName = "水位(m)";
	// 			// setRiverSectionsValue(m_RiverSectionVDataArray[0]); 暂时注释，没有引用basicmap_home.js 20230419
	// 		}
	// 		else if(m_Layer_River_ShowMode == 1)//显示水深
	// 		{
	// 			DefaultMapTipParaName = "水深(m)";
	// 			// setRiverSectionsValue(m_RiverSectionVDataArray[3]); 暂时注释，没有引用basicmap_home.js 20230419
	// 		}
	// 	}
	// }
})


