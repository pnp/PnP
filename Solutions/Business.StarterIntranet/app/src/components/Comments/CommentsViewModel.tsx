
// ========================================
// Discussion board View Model
// ========================================
import * as React from "react";
import * as ReactDOM from "react-dom";
import DiscussionBoard from "./DiscussionBoard/DiscussionBoard";

class CommentsViewModel {

    constructor() {

        // Check if the comments are enabled for the page
        // Theorically, the page layout must have this information
        const hiddenElt = $("#allow-page-comments-hidden");

        if (hiddenElt) {
            const showComments = hiddenElt.text().trim();
            if (parseInt(showComments, 10) === 1) {

                // We encapsulate the React component in a Knockout component to be able to control the DOM anchor point.
                // If you call the render() method directly in the main.ts, it means the element with id 'page-discussion-board' has to be present in the master page initially (error otherwise).
                ReactDOM.render(<DiscussionBoard listRootFolderUrl="Comments" />, document.getElementById("page-comments"));
            }
        }
    }
}

export default CommentsViewModel;
