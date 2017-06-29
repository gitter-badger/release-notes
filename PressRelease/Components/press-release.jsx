//@flow
'use strict'
import React from 'react';
import styled from 'styled-components';

const AppGoesHere = styled.div`
    color: ${props => props.primary ? "red" : "goldenrod"};
`;

let x = function (): string {
    return 5;
};

const PressRelease = (props: any) => (
    <form>
        <label>
            <span>Select a repository:</span>
            <select>
                <option value="1">Repository 1</option>
                <option value="2">Repository 2</option>
                <option value="3">Repository 3</option>
            </select>
        </label>
        <label>
            <span>Select a branch:</span>
            <select>
                <option value="1">Branchify 1</option>
                <option value="2">Branch 2</option>
                <option value="3">Branch 3</option>
            </select>
        </label>
    </form>
);

module.exports = PressRelease;