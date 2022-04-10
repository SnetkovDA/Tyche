
async function sendData(method = 'POST', url = '', data = {}, auth = false) {
    const response = await fetch(url, {
        method: method,
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': auth ? 'Bearer ' + sessionStorage.accessToken : ''
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer',
        body: method == 'GET' ? null : JSON.stringify(data)
    });
    return response;
}