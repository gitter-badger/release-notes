import React from 'react';
import ReactDOM from 'react-dom';

var clientAppNode = document.getElementById('client-app');
if (clientAppNode) {
    ReactDOM.render(
        <h1>Hello, world!</h1>,
        clientAppNode
    );
}
