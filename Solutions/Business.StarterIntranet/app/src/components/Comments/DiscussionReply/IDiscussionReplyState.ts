export interface IDiscussionReplyState {
    showInput: boolean;
    editMode: EditMode;
}

export enum EditMode {
    NewComment,
    UpdateComment,
}