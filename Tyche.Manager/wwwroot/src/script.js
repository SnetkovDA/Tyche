

document.addEventListener('DOMContentLoaded', function () {
    let newElem = document.createElement('p');
    newElem.textContent = 'Page loaded';
    document.getElementById('rootElement').appendChild(newElem);
}, false);