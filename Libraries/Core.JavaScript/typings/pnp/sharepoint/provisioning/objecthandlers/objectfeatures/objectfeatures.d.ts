import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";
export declare class ObjectFeatures extends ObjectHandlerBase {
    constructor();
    ProvisionObjects(features: Array<IFeature>): Promise<{}>;
}
