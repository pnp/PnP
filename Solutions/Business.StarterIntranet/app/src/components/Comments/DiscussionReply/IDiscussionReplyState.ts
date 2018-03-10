export interface IDiscussionReplyState {
    showInput: boolean;
    editMode: EditMode;
    inputValue: string;
    isLoading: boolean;
}

export enum EditMode {
    NewComment,
    UpdateComment,
}
