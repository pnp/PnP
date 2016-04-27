import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";
export declare class ObjectFiles extends ObjectHandlerBase {
    constructor();
    ProvisionObjects(objects: Array<IFile>): Promise<{}>;
    private RemoveWebPartsFromFileIfSpecified(clientContext, limitedWebPartManager, shouldRemoveExisting);
    private GetWebPartXml(webParts);
    private AddWebPartsToWebPartPage(dest, src, webParts, shouldRemoveExisting);
    private ApplyFileProperties(dest, fileProperties);
    private GetViewFromCollectionByUrl(viewCollection, url);
    private ModifyHiddenViews(objects);
    private GetFolderFromFilePath(filePath);
    private GetFilenameFromFilePath(filePath);
}
