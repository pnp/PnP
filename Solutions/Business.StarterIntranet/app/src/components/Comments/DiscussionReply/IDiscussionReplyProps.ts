import { IDiscussionReply } from "../../../models/IDiscussionReply";

interface IDiscussionReplyProps {
    reply: IDiscussionReply;
    addNewReply: (parentId: number, replyBody: string) => {};
    deleteReply: (reply: IDiscussionReply) => {};
    updateReply: (reply: IDiscussionReply) => {};
}

export default IDiscussionReplyProps;