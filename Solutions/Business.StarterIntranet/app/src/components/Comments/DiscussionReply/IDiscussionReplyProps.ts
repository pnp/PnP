import { IDiscussionReply } from "../../../models/IDiscussionReply";

interface IDiscussionReplyProps {
    reply: IDiscussionReply;
    addNewReply(parentId: number, replyBody: string): Promise<void>;
    deleteReply(reply: IDiscussionReply): Promise<void>;
    updateReply(reply: IDiscussionReply): Promise<void>;
    toggleLikeReply(reply: IDiscussionReply, isLiked: boolean): Promise<void>;
}

export default IDiscussionReplyProps;