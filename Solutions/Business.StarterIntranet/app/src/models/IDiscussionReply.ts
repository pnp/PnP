import { PermissionKind } from "sp-pnp-js";

export interface IDiscussionReply {
    Id: number;
    ParentItemID?: number;
    Author?: {
        Id: number; // The id in the user info list
        DisplayName: string;
        PictureUrl: string;
    };
    Posted?: Date;
    Edited?: Date;
    UserPermissions?: DiscussionPermissionLevel[];
    Body: string;
    Children?: IDiscussionReply[];
    LikesCount?: number;
    LikedBy?: string[]; // Array of user ids
    ParentListId?: string;
}

export enum DiscussionPermissionLevel {
    Add,
    Delete,
    Edit,
    EditAsAuthor,
    DeleteAsAuthor,
    ManageWeb,
    ManageLists,
}
