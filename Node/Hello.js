
let http = require("http");

http.createServer(function (request, response)
{
    response.writeGead(200, {'Content-Type' : 'text/plain'});

    response.end("Hello world");
}).listen(8000);

console.log("Server running");