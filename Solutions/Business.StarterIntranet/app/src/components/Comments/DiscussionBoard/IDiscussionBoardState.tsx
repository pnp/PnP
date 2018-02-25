import IDiscussion from "../../../models/IDiscussion";
import { PermissionKind } from "sp-pnp-js";
import { DiscussionPermissionLevel } from "../../../models/IDiscussionReply";

interface IDiscussionBoardState {
    discussion: IDiscussion;
    userPermissions: DiscussionPermissionLevel[]
}

export default IDiscussionBoardState;