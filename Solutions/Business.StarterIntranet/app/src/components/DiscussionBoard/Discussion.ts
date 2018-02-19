import IDiscussion from "../../models/IDiscussion";
import IDiscussionReply from "../../models/IDiscussionReply";

class Discussion implements IDiscussion{
    Id: number;
    Title: string;
    Body: string;
    Replies: IDiscussionReply[];

    public static SelectFields = [
        "Title",
        "Body",
    ];

}