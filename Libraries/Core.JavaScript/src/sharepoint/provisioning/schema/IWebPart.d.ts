/// <reference path="icontents.d.ts"" />
interface IWebPart {
    Title: string;
    Order: number;
    Zone: string;
    Row: number;
    Column: number;
    Contents: IContents;
}
