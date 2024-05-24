var viewer;
function initOpenSeadragon(settings) {
    viewer = OpenSeadragon({
        id: "openseadragon1",
        prefixUrl: settings.prefixUrl, // URL to OpenSeadragon icons
        imageSmoothingEnabled: false, 
        tileSources: {
            width: settings.width,
            height: settings.height,
            tileSize: settings.tileSize,
            minLevel: settings.minLevel,
            maxLevel: settings.maxLevel,
            getTileUrl: function (level, x, y) {
                return settings.getTileUrl
                    .replace('{z}', level)
                    .replace('{x}', x)
                    .replace('{y}', y);
            }
        },
        maxZoomPixelRatio: 1000, // Default is usually 1.5
        minZoomLevel: 0,
        maxZoomLevel: 1000, // Adjust this based on your needs
    });
}

function zoomToPixel(x, y) {
    if (!viewer || !viewer.viewport) {
        console.error('Viewer has not been initialized or is missing viewport.');
        return;
    }

    // Log current input for debugging
    console.log("Zooming to X: " + x + ", Y: " + y);

    // Ensure coordinates are within the image bounds
    x = Math.min(x, 65535);  // Image width - 1
    y = Math.min(y, 65535);  // Image height - 1

    // Calculate the normalized coordinates of the pixel
    var point = new OpenSeadragon.Point(x, y);
    var viewportPoint = viewer.viewport.imageToViewportCoordinates(point);

    // Define the zoom level; you can adjust this value based on how close you want to zoom in
    var zoomLevel = 10;  // Example zoom level

    // Use fitBounds to ensure the viewport focuses correctly
    var rect = new OpenSeadragon.Rect(viewportPoint.x - 0.0005, viewportPoint.y, 0.001 );
    viewer.viewport.fitBounds(rect, false);
}

function zoomToPixel(ip) {
    if (!viewer || !viewer.viewport) {
        console.error('Viewer has not been initialized or is missing viewport.');
        return;
    }
    console.log(ip);
    const pos = dToXY(65536,ipToUint(ip));

    var x = pos.x;
    var y = pos.y;

    // Log current input for debugging
    console.log("Zooming to X: " + x + ", Y: " + y);

    // Ensure coordinates are within the image bounds
    x = Math.min(x, 65535);  // Image width - 1
    y = Math.min(y, 65535);  // Image height - 1

    // Calculate the normalized coordinates of the pixel
    var point = new OpenSeadragon.Point(x, y);
    var viewportPoint = viewer.viewport.imageToViewportCoordinates(point);

    // Use fitBounds to ensure the viewport focuses correctly
    var rect = new OpenSeadragon.Rect(viewportPoint.x - 0.0005, viewportPoint.y, 0.001);
    viewer.viewport.fitBounds(rect, false);
}

function dToXY(n, d) {
    let x = 0, y = 0;
    let t = d;
    let s = 1;

    while (s < n) {
        let rx = 1 & (t / 2);
        let ry = 1 & (t ^ rx);
        [x, y] = rotateAndFlip(s, x, y, rx, ry);
        x += s * rx;
        y += s * ry;
        t /= 4;
        s *= 2;
    }
    return { x, y };
}

function rotateAndFlip(n, x, y, rx, ry) {
    if (ry === 0) {
        if (rx === 1) {
            x = n - 1 - x;
            y = n - 1 - y;
        }
        // Swap x and y
        return [y, x];
    }
    return [x, y];
}
function ipToUint(ip) {
    const octets = ip.split('.').map(Number); // Split IP into an array of numbers
    if (octets.length !== 4 || octets.some(isNaN)) {
        throw new Error('Invalid IP address format');
    }

    return (octets[0] << 24) | (octets[1] << 16) | (octets[2] << 8) | octets[3];
}