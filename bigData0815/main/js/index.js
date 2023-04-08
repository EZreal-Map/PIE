
var scn_data={		
		dtu:{ on:2,off:20},      //DTU在线
		user:{on:10,off:200},      //用户在线数		
		waterQClass:"Ⅱ",
		currentDataCount:"0G",//当前流量
		dataTypeArray:{v1:10,v2:11,v3:12,v3:14,v4:15,v5:17,v6:18},
		almMsg:[{msg:"系统正在启动..."}
				],
		dataCountArray:[{recv:2000,send:200},
			{recv:2000,send:400},
			{recv:3000,send:500},
			{recv:4000,send:305},
			{recv:4000,send:400},
			{recv:4000,send:101},
			{recv:4000,send:660},
			{recv:1500,send:707},
			{recv:6000,send:880},
			{recv:4500,send:220},
			{recv:5000,send:990},
			{recv:6000,send:1000},
			{recv:8000,send:1110},
			{recv:7000,send:2220},
			{recv:5000,send:3330},
			{recv:7000,send:110},
			{recv:6000,send:330},
			{recv:6500,send:550},
			{recv:7000,send:770},
			{recv:8500,send:900}
			],
		map:[{area:"湖北",cnt:60},
			{area:"浙江",cnt:25},
			{area:"江苏",cnt:15},
			{area:"四川",cnt:20},
			{area:"广东",cnt:20},
			{area:"北京",cnt:10},
			{area:"湖南",cnt:10},
			{area:"贵州",cnt:10},
			{area:"西藏",cnt:10},
			{area:"新疆",cnt:10},
			{area:"青海",cnt:10}
		],
		
		tableHeader:[
	        {"categories":"方案名称"},
	        {"categories":"方案编码"},
	        {"categories":"操作"}
    	],
		tableData:[ //BDCardID,DeviceName,MsgInterval,MonitorParas,Address
			{"BDCardID":"0920865","DeviceName": "北斗DTU01", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        {"BDCardID":"0920731","DeviceName": "北斗DTU02", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        {"BDCardID":"0920733","DeviceName": "北斗DTU03", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        {"BDCardID":"0920738","DeviceName": "北斗DTU04", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        {"BDCardID":"0920702","DeviceName": "北斗DTU05", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        {"BDCardID":"0920598","DeviceName": "北斗DTU06", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        {"BDCardID":"0920487","DeviceName": "北斗DTU07", "MsgInterval": 60,"MonitorParas": "温度;湿度;PM2.5","Address": "湖北省武汉市洪山区鲁磨路442"},
	        ]
		,
		reciver:["1#","2#","3#","4#","5#","6#","7#","8#","9#","10#"]
		,reciverData:{d01:[4,4,4,4,4,4,4,4,4,4],d02:[4,4,4,4,4,4,4,4,4,4],d03:[4,4,4,4,4,4,4,4,4,4],d04:[4,4,4,4,4,4,4,4,4,4],
			d05:[4,4,4,4,4,4,4,4,4,4],d06:[4,4,4,4,4,4,4,4,4,4],d07:[4,4,4,4,4,4,4,4,4,4],d08:[4,4,4,4,4,4,4,4,4,4],
			d09:[4,4,4,4,4,4,4,4,4,4],d10:[4,4,4,4,4,4,4,4,4,4]}
		//reciver:[{name:"1#"},{name:"2#"},{name:"4#"},{name:"5#"},{name:"6#"}]
	};
	
var vm = new Vue({
	el: '#content',
	data: scn_data,
	methods: {
		details: function() {
			
		}
	}
})