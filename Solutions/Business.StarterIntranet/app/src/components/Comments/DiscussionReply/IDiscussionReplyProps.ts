import { IDiscussionReply } from "../../../models/IDiscussionReply";

interface IDiscussionReplyProps {
    reply: IDiscussionReply;
    isLikeEnabled: boolean;
    addNewReply(parentId: number, replyBody: string): Promise<void>;
    deleteReply(reply: IDiscussionReply): Promise<void>;
    updateReply(reply: IDiscussionReply): Promise<void>;
    toggleLikeReply(reply: IDiscussionReply, isLiked: boolean): Promise<void>;
    isChildReply?: boolean;
    replyLevel: number;
}

export default IDiscussionReplyProps;