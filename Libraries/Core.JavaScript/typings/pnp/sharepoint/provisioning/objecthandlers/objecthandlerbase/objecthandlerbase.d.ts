export declare class ObjectHandlerBase {
    private name;
    constructor(name: string);
    ProvisionObjects(objects: any, parameters?: any): Promise<{}>;
    scope_started(): void;
    scope_ended(): void;
}
