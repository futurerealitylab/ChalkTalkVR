var udp = require('dgram');
var math3d = require('math3d');

count = 0;
/* process.argv.forEach(function (val, index, array) {
	if (count > 1) {
		ip_addresses.push(val);
	}
	count++;
});
 */
//console.log("Sending to: " + ip_addresses)

// for verizon 5g
downstramPorts = [];

var sourceIP = "127.0.0.1";
var argv = require('minimist')(process.argv.slice(2));
if(argv["source"] != null){
	sourceIP = argv["source"];
}

if(argv["dwPorts"] != null){
	var dwPorts = argv["dwPorts"];
	//var fs = require('fs');

dwPorts.substr(1,dwPorts.length-2).split(",").forEach(function(port){
			downstramPorts.push(port);
		});
console.log(downstramPorts);
/* 	fs.readFileSync(dwfv, {encoding: 'utf-8'}, function(err, data) {
		console.log("utf 8 " + data);
		data.toString().split(" ").forEach(function(port){
			downstramPorts.push(port);
		});
		console.log("read file " + downstramPorts); */
  //});
}

var framerate = 30.;
if (argv["framerate"] != null) {
	framerate = argv["framerate"];
}

var tracked_ips_only = false;
if (argv['tracked-ips-only'])
	tracked_ips_only = true;

var target_data_only = false;
if (argv['target-data-only'])
	target_data_only = true;

//These will get unicast to no matter what!
saved_ips = ['192.168.1.6', '192.168.1.20', '192.168.1.28', '192.168.1.42', '192.168.1.39', '192.168.1.71', '192.168.1.70']

//These are overriding ip mappings. 
tracked_ip_mappings = { 
	"RB2" : "192.168.1.42"
}

for (var i = 1; i < 151; i++) {
	name = "RB" + i.toString();
	ip = "192.168.1." + (100 + i).toString();
	if (tracked_ip_mappings[name] == undefined)
		tracked_ip_mappings[name] = ip;
}

console.log("\n" + downstramPorts + "\n");
const holojam = require('holojam-node')(['relay'], '0.0.0.0', 9593, 9592, downstramPorts);
console.log("Sending at " + framerate + " fps.");
//holojam.ucAddresses = holojam.ucAddresses.concat(saved_ips);
var Vector3 = math3d.Vector3;
var Quaternion = math3d.Quaternion;

var pool = {}

holojam.on('update', (flakes, scope, origin) => {
	for (var i=0; i < flakes.length; i++) {
		var flake = flakes[i];
		pool[flake['label']] = flake
	}
});

function isEmpty(obj) {
  return Object.keys(obj).length === 0;
}

function getCurrentDate() {
	var currentdate = new Date(); 
	var datetime = "Last Sync: " + currentdate.getDate() + "/"
                + (currentdate.getMonth()+1)  + "/" 
                + currentdate.getFullYear() + " @ "  
                + currentdate.getHours() + ":"  
                + currentdate.getMinutes() + ":" 
                + currentdate.getSeconds();
    return datetime;
}

function pprintItem(key, value) {
	var memo = ((("               ") + key).slice(-15)) + ":\t";
	//Vector3s
	if (value['vector3s'] != undefined && !isEmpty(value['vector3s'])) {
		memo += "Vector3s: ";
		for (var index in value['vector3s']) {
			memo += value['vector3s'][index].x.toFixed(3) + " ";
			memo += value['vector3s'][index].y.toFixed(3) + " ";
			memo += value['vector3s'][index].z.toFixed(3) + "\t";
		}
	}
	//Vector4s
	if (value['vector4s'] != undefined && !isEmpty(value['vector4s'])) {
		memo += "Vector4s: ";
		for (var index in value['vector4s']) {
			memo += value['vector4s'][index].x.toFixed(3) + " ";
			memo += value['vector4s'][index].y.toFixed(3) + " ";
			memo += value['vector4s'][index].z.toFixed(3) + " ";
			memo += value['vector4s'][index].w.toFixed(3) + "\t";
		}
	}
	//floats
	if (value['floats'] != undefined && !isEmpty(value['floats'])) {
		memo += "Floats: ";
		for (var index in value['floats']) {
			memo += value['floats'][index] + " ";
		}
	}
	//ints
	if (value['ints'] != undefined && !isEmpty(value['ints'])) {
		memo += "Ints: ";
		for (var index in value['ints']) {
			memo += value['ints'][index] + " ";
		}
	}
	//text
	if (value['text'] != undefined && value['text'] != null) {
		memo += "Text: " + value['text'];
	}
	console.log(memo);
}

setInterval(() => {	
	console.log("\n" + getCurrentDate());
	if (!isEmpty(pool)) {
		sorted_keys = []
		for (var key in pool) {
			sorted_keys.push(key);
		}
		sorted_keys.sort();
		for (var key in sorted_keys) {
			key = sorted_keys[key]
			pprintItem(key, pool[key]);
		}
		console.log("\n");
		pool = {};
	}
	//console.log("IP addresses: " + holojam.ucAddresses + "\n");
	if (lighthouses.count > 0) console.log("Vive Sources: " + Object.keys(lighthouses));
	if (calibratedSource) console.log("Calibrated Source: " + calibratedSource);
}, 1000);

////////////////////////////////////////////////////////////////////////////////
//////////////////// Optitrack
////////////////////////////////////////////////////////////////////////////////


optirx = require('optirx');
var optitrack = udp.createSocket({type: 'udp4', reuseAddr: true});
optitrack.on('listening', function () {
    var address = optitrack.address();
    console.log('\nOptitrack listening on ' + address.address + ":" + address.port);
    //optitrack.setBroadcast(true);
    optitrack.setMulticastTTL(128); 
    optitrack.addMembership('239.255.42.99', '127.0.0.1');
});

optitrack.bind({
	address: '127.0.0.1',
	port: 1511  
});

optitrack.on('error', function(error) {
	console.log("Error: " + error);
	optitrack.close()
});

optitrack.on('close', function(){
	console.log('Optitrack server closed.');
});

rigidbodies = [];
optitrack.on('message', function(message, info){
	unpackedData = optirx.unpack(message);
	rigidbodies = [];
	rigidbodies.push({
		label : "connected",
	});
	for (var i = 0; i < unpackedData['rigid_bodies'].length; i++) {
		var rbody = unpackedData['rigid_bodies'][i];

		if (!rbody['tracking_valid']) continue; //Skip untracked markers.

		var position = rbody['position'];
		var rotation = rbody['orientation'];
		var id = "RB" + rbody['id'];

		body = {
			label: id,
			// note negation of some values to convert to unity's left-hand coordinate system
			vector3s: [{x: -position[0], y:position[1], z:position[2]}],
			vector4s: [{x: -rotation[0], y:rotation[1], z:rotation[2], w:-rotation[3]}]
		}
		rigidbodies.push(body);
		pool[body['label']] = body;
	}
});

setInterval(() => {
	if (rigidbodies.length == 0) return;

	ips = saved_ips;
	if (target_data_only) {
		//Only send to specific target.
		for (var i = 0; i < rigidbodies.length; i++) {
			body = rigidbodies[i];
			if (!tracked_ip_mappings[body['label']]) continue;
			var ip = tracked_ip_mappings[body['label']];		
			var pack = holojam.BuildUpdate('Optitrack', [body]);
			holojam.SendToAddress(pack, ip);
		}
		holojam.Send(holojam.BuildUpdate('Optitrack', rigidbodies));
	} else {
		if (tracked_ips_only) {
			for (var i = 0; i < rigidbodies.length; i++) {
				body = rigidbodies[i];
				if (!tracked_ip_mappings[body['label']]) continue;
				var ip = tracked_ip_mappings[body['label']];
				if (!ips.includes(ip)) ips.push(ip);
			}
			//holojam.ucAddresses = ips;
		}
		holojam.Send(holojam.BuildUpdate('Optitrack', rigidbodies));
	}
}, 1000./framerate);


////////////////////////////////////////////////////////////////////////////////
//////////////////// Vive
////////////////////////////////////////////////////////////////////////////////

var viveServer = udp.createSocket('udp4');
viveServer.bind({
  address: sourceIP,
  port: 10000,
  reuseAddr: true,
});

viveServer.on('error', function(error) {
	console.log("Error: " + error);
	viveServer.close()
});

viveServer.on('close', function(){
	console.log('Vive socket closed.');
});

viveServer.on('listening', function(){
	var address = viveServer.address();
	console.log('\nVive listening on ' + address.address + ":" + address.port);
});


var calibratedSource = undefined
var lighthouses = {}

function tryAssignLighthouse(address, trackedObject) {
	if (!calibratedSource && viveServer.address().address == address)
		calibratedSource = address
	lighthouses[address] = trackedObject;
}

function tryCalibrateObject(address, trackedObject) {
	if (!(calibratedSource) || calibratedSource == address) {
		return trackedObject;
	} 
	if (lighthouses[address] == undefined) {
		return trackedObject;
	}

	var toPos = trackedObject['vector3s'][0]
	toPos = new Vector3(toPos.x, toPos.y, toPos.z);
	var toRot = trackedObject['vector4s'][0]
	toRot = new Quaternion(toRot.x, toRot.y, toRot.z, toRot.w);

	var lh1 = lighthouses[calibratedSource];
	var lh1Pos = lh1['vector3s'][0]
	lh1Pos = new Vector3(lh1Pos.x, lh1Pos.y, lh1Pos.z);
	var lh1Rot = lh1['vector4s'][0]
	lh1Rot = new Quaternion(lh1Rot.x, lh1Rot.y, lh1Rot.z, lh1Rot.w);

	var lh2 = lighthouses[address];
	var lh2Pos = lh2['vector3s'][0]
	lh2Pos = new Vector3(lh2Pos.x, lh2Pos.y, lh2Pos.z);
	var lh2Rot = lh2['vector4s'][0]
	lh2Rot = new Quaternion(lh2Rot.x, lh2Rot.y, lh2Rot.z, lh2Rot.w);

	var tempQuat = lh1Rot.mul(lh2Rot.inverse());
	var deltaPos = lh1Pos.sub(tempQuat.mulVector3(lh2Pos));

	var newPos = deltaPos.add(tempQuat.mulVector3(toPos));
	var newRot = tempQuat.mul(toRot);

	trackedObject['vector3s'] = [{x: newPos.x, y: newPos.y, z:newPos.z}]
	trackedObject['vector4s'] = [{x: newRot.x, y: newRot.y, z: newRot.z, w: newRot.w}]

	return trackedObject;
}

var trackedObjects = [];
viveServer.on('message', function(message, info){
	var json = JSON.parse(message.toString());
	trackedObjects = [];
	for (var key in json) {
		if(!json.hasOwnProperty(key) || key == 'time'){
			continue;
		}
		trackedObject = { 
			label: json[key].id,
	    	vector3s: [{x: parseFloat(json[key].x), y: parseFloat(json[key].y), z: -parseFloat(json[key].z)}],
			vector4s: [{x: parseFloat(json[key].qx), y: parseFloat(json[key].qy), z: -parseFloat(json[key].qz), w: -parseFloat(json[key].qw)}]
		};
		if (json[key]["triggerPress"] != undefined) {
			trackedObject['ints'] = [parseInt(json[key]['appMenuPress']), parseInt(json[key]['gripPress']), parseInt(json[key]['touchpadPress']), parseInt(json[key]['triggerPress']), 0, 0]
			trackedObject['floats'] = [0., 0., 0., 0., 0., 0.]
		}

		if (trackedObject['label'].includes("LIGHTHOUSE")) {
			tryAssignLighthouse(info.address, trackedObject);
		}

		if (trackedObject['label'].includes("LIGHTHOUSE")) {
			//Don't send lighthouses!
			continue;
		}
		
		trackedObject = tryCalibrateObject(info.address, trackedObject)

		if (trackedObject['label'] != undefined) {
			trackedObjects.push(trackedObject);
		}
		pool[json[key].id] = trackedObject;
	}
});

setInterval(() => {
	if (!isEmpty(trackedObjects)) {
		holojam.Send(holojam.BuildUpdate('Vive', trackedObjects));
	}
}, 1000./framerate);