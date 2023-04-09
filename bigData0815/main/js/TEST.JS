var ServerURL = "../../SZRiverSys/"
function loadStationLayers() { //加载断面站点        
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