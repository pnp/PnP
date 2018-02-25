import { IDiscussionReply } from "../../../models/IDiscussionReply";

interface IDiscussionReplyProps {
    reply: IDiscussionReply;
    addNewReply: (parentId: number, replyBody: string) => {};
    deleteReply: (replyId: number) => {};
    updateReply: (reply: IDiscussionReply) => {};
}

export default IDiscussionReplyProps;