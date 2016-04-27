import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";
export declare class ObjectLists extends ObjectHandlerBase {
    constructor();
    ProvisionObjects(objects: Array<IListInstance>): Promise<{}>;
    private EnsureLocationBasedMetadataDefaultsReceiver(clientContext, list);
    private CreateFolders(params);
    private ApplyContentTypeBindings(params);
    private ApplyListInstanceFieldRefs(params);
    private ApplyFields(params);
    private ApplyLookupFields(params);
    private GetFieldXmlAttr(fieldXml, attr);
    private GetFieldXml(field, lists, list);
    private ApplyListSecurity(params);
    private CreateViews(params);
    private InsertDataRows(params);
}
