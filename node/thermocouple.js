console.log('Kurt J. Lesker Thermocouple');

var SerialPort = require("serialport");

var serialPort = new SerialPort.SerialPort("/dev/cu.usbserial", {
	parser: SerialPort.parsers.readline("\r\n") 
});

serialPort.on("open", function () {
	var pressure = 1999.0;
	serialPort.on('data', function(data) {
		if (data == '_---.') return;
		var reading = parseFloat(data);
		if (reading != pressure) {
			pressure = reading;
			// fire an event here...
		}
	});  
});