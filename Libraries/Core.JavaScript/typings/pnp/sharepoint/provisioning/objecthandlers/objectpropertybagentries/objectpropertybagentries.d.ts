import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";
export declare class ObjectPropertyBagEntries extends ObjectHandlerBase {
    constructor();
    ProvisionObjects(entries: Array<IPropertyBagEntry>): Promise<{}>;
}
