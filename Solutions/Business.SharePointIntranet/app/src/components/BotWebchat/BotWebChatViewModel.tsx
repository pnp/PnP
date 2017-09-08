// ========================================
// Bot web chat View Model
// ========================================
import * as React from "react";
import * as ReactDOM from "react-dom";
import { BotChatControl } from "./BotChatControl";

class BotWebChatViewModel {

    constructor() {

        // We encapsulate the React component in a Knockout component to be able to control the DOM anchor point.
        // If you call the render() method directly in the main.ts, it means the element with id 'bot-webchat' has to be present in the master page initially (error otherwise).
        ReactDOM.render(<BotChatControl />, document.getElementById("bot-webchat"));
    }
}

export default BotWebChatViewModel;
