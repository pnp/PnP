import IDiscussionReply from "../../models/IDiscussionReply";

interface IDiscussionReplyProps {
    reply: IDiscussionReply;
    addNewReply: (parentId: number, replyBody: string) => {};
    deleteReply: (replyId: number) => {};
}

export default IDiscussionReplyProps;