import React from 'react';
import ReactDOM from 'react-dom';

import PressRelease from './press-release.jsx';

var clientAppNode = document.getElementById('client-app');
if (clientAppNode) {
    ReactDOM.render(
        <PressRelease />,
        clientAppNode
    );
}
