import IDiscussion from "../../../models/IDiscussion";
import { PermissionKind } from "sp-pnp-js";
import { DiscussionPermissionLevel } from "../../../models/IDiscussionReply";

interface IDiscussionBoardState {
    discussion: IDiscussion;
    userPermissions: DiscussionPermissionLevel[];
    inputValue: string;
    isLoading: boolean;
}

export default IDiscussionBoardState;