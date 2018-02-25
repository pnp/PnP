import { PermissionKind } from "sp-pnp-js/lib/pnp";

export interface IDiscussionReply {
    Id: number;
    ParentItemID?: number;
    Author?: {
        DisplayName: string;
        PictureUrl: string; 
    };
    Posted?: Date;
    UserPermissions?: DiscussionPermissionLevel[];
    Body: string;
}

export enum DiscussionPermissionLevel {
    Add,
    Delete,
    Edit,
    EditAsAuthor,
    ManageWeb,
    ManageLists,
}