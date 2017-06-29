'use strict'
import React from 'react';
import ReactDOM from 'react-dom';

import ReleaseNotes from './press-release.jsx';

var clientAppNode = document.getElementById('client-app');
if (clientAppNode) {
    ReactDOM.render(
        <ReleaseNotes />,
        clientAppNode
    );
}
