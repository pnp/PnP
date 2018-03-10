
// ========================================
// Discussion board View Model
// ========================================
import * as React from "react";
import * as ReactDOM from "react-dom";
import DiscussionBoard from "./DiscussionBoard/DiscussionBoard";

class CommentsViewModel {

    constructor() {

        // We encapsulate the React component in a Knockout component to be able to control the DOM anchor point.
        // If you call the render() method directly in the main.ts, it means the element with id 'page-discussion-board' has to be present in the master page initially (error otherwise).
        ReactDOM.render(<DiscussionBoard listRootFolderUrl="Comments" />, document.getElementById("page-comments"));
    }
}

export default CommentsViewModel;
