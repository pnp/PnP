import { IDiscussionReply } from "./IDiscussionReply";

interface IDiscussion {
    Id?: number;
    AreLikesEnabled: boolean;
    AssociatedPageId: number;
    Title: string;
    Body: string;
    Created?: Date;
    Author?: string;
    Replies?: IDiscussionReply[];
    ListId?: string;
}

export default IDiscussion;
