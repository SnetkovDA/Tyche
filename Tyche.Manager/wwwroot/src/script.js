var scannersList;
var rootContent;
var currentScannerName;
var currentScannerHost;
var scannersMap = {};

document.addEventListener('DOMContentLoaded', async function () {
    scannersList = document.getElementById('scanners-list');
    rootContent = document.getElementById('root-content');
    currentScannerName = document.getElementById('current-scanner-name');
    currentScannerHost = document.getElementById('current-scanner-host');
    currentScannerName.innerText = '<- Add new scanner to start';
    currentScannerName.id = '';
    await fillScannersList();
}, false);

async function addScanner() {
    const scannerName = prompt('Scanner name:', 'Scanner ' + (scannersList.childElementCount + 1));
    const scannerHost = prompt('Scanner host (CANNOT be changed after creation):', '');
    if (!(/^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/.test(scannerHost))) {
        alert('Cannot create scanner. Host is invalid: ' + scannerHost);
        return;
    }
    if (!confirm('Are you sure to create scanner with name: \'' + scannerName + '\' and host: \'' + scannerHost + '\' ?')) {
        alert('Cannot create scanner. Abort by user.');
        return;
    }
    try {
        const scannerId = await sendData('PUT', 'scanner/add', { Name: scannerName, Host: scannerHost}).then((response) => {
            return response.json()
        });
        console.log(scannerId);
        const scanner = await sendData('GET', 'scanner/' + scannerId.id, '').then((response) => { return response.json() });
        addScannerToList(scanner);
    } catch (error) {
        console.error(error);
        alert('Cannot create scanner. Error: ' + error);
    }
}

async function fillScannersList() {
    const responce = await sendData('GET', 'scanner/allscanners', '').then((response) => {
        return response.json()
    });
    scannersList.innerHTML = '';
    responce.forEach(element => {
        addScannerToList(element);
    });
    console.log(responce);
}

function addScannerToList(scannerObj) {
    var scanner = document.createElement('li');
    scanner.innerHTML = '<button onclick="selectScanner(\'' + scannerObj.id + '\')">\
    <p class="scanner-name">' + scannerObj.name + '</p>\
    <p class="scanner-host">' + scannerObj.host + '</p>\
    <p class="scanner-status" style="color: greenyellow;">Active</p>\
    </button>';
    scannersList.appendChild(scanner);
    scannersMap[scannerObj.id] = scannerObj;
    currentScannerName.innerText = scannerObj.name;
    currentScannerHost.innerText = scannerObj.host;
}

function selectScanner(scannerId) {
    const scannerObj = scannersMap[scannerId];
    currentScannerName.innerText = scannerObj.name;
    currentScannerName.id = scannerId;
    currentScannerHost.innerText = scannerObj.host;
    console.log('Scanner selected: ' + scannerObj.name);
}