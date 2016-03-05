/// <reference path="ihiddenview.d.ts" />
/// <reference path="iwebpart.d.ts" />

interface IFile {
    Overwrite: boolean;
    Dest: string;
    Src: string;
    Properties: Object;
    RemoveExistingWebParts: boolean;
    WebParts: Array<IWebPart>;
    Views: Array<IHiddenView>;
}
