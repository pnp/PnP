export interface IDiscussionReplyState {
    showInput: boolean;
    editMode: EditMode;
    inputValue: string;
}

export enum EditMode {
    NewComment,
    UpdateComment,
}