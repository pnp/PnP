import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";
export declare class ObjectNavigation extends ObjectHandlerBase {
    constructor();
    ProvisionObjects(object: INavigation): Promise<{}>;
    private ConfigureQuickLaunch(nodes, clientContext, navigation);
}
