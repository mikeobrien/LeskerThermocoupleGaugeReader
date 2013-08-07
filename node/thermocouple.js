var serialPort = require("serialport"),
	http = require("http"),
	url = require("url"),
	files = require("fs"),
	path = require("path"),
	mime = require("mime"),
	events = require("events");

console.log('Kurt J. Lesker Thermocouple Gauge');

var gaugeEvents = new events.EventEmitter();

var vacuumGauge = new serialPort.SerialPort("/dev/cu.usbserial", {
	parser: serialPort.parsers.readline("\r\n") 
});

var pressure = 1999.0;

vacuumGauge.on("open", function () {
	vacuumGauge.on('data', function(data) {
		if (data == '_---.') return;
		var reading = parseFloat(data);
		if (reading != pressure) {
			pressure = reading;
			gaugeEvents.emit("pressure", pressure);
		}
	}); 
});

function renderPage(response, pagePath) {
	pagePath = path.join(__dirname, pagePath);
	response.writeHead(200, { 
		"Content-Type": mime.lookup(pagePath),
		"Content-Length": files.statSync(pagePath).size });
	files.createReadStream(pagePath).pipe(response);
}

function streamEvents(request, response) {
	response.writeHead(200, {
		"Content-Type": "text/event-stream",
		"Cache-Control": "no-cache",
		"Connection": "keep-alive"
	});
	response.write("retry: 10000\n");
	var sendPressure = function(pressure) {
		response.write("id: " + (new Date()).toLocaleTimeString() + "\n");
		response.write("event: pressure\n");
		response.write("data: " + pressure + "\n\n");
	};
	sendPressure(pressure);
	gaugeEvents.on("pressure", sendPressure);
	request.connection.once("close", function() {
		gaugeEvents.removeListener("pressure", sendPressure);
	})
}

http.createServer(function(request, response) {
	switch (url.parse(request.url).pathname) {
		case "/": renderPage(response, 'thermocouple.html'); break;
		case "/data": streamEvents(request, response); break;
		default:
			response.writeHead(404);
			response.end();
	}
}).listen(5150); 