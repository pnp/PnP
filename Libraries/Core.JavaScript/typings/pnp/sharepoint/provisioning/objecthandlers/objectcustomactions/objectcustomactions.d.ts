import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";
export declare class ObjectCustomActions extends ObjectHandlerBase {
    constructor();
    ProvisionObjects(customactions: Array<ICustomAction>): Promise<{}>;
}
