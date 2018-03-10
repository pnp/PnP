import { PermissionKind } from "sp-pnp-js";
import IDiscussion from "../../../models/IDiscussion";
import { DiscussionPermissionLevel } from "../../../models/IDiscussionReply";

interface IDiscussionBoardState {
    discussion: IDiscussion;
    userPermissions: DiscussionPermissionLevel[];
    inputValue: string;
    areCommentsLoading: boolean;
    inputPlaceHolderValue: string;
    isAdding: boolean;
}

export default IDiscussionBoardState;
