import { IDiscussionReply } from "./IDiscussionReply";

interface IDiscussion {
    Id?: number;
    AssociatedPageId: number;
    Title: string;
    Body: string;
    Created?: Date;
    Author?: string;
    Replies?: IDiscussionReply[];
}

export default IDiscussion;