import { IDiscussionReply } from "./IDiscussionReply";

interface IDiscussion {
    Id?: number;
    AssociatedPageId: number;
    Title: string;
    Body: string;
    Replies?: IDiscussionReply[];
}

export default IDiscussion;