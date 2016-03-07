interface IView {
    Title: string;
    Paged: boolean;
    PersonalView: boolean;
    Query: string;
    RowLimit: number;
    Scope: number;
    SetAsDefaultView: boolean;
    ViewFields: Array<string>;
    ViewTypeKind: string;
}
