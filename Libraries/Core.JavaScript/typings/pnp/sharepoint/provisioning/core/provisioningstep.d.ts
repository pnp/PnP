export declare class ProvisioningStep {
    private name;
    private index;
    private objects;
    private parameters;
    private handler;
    execute(dependentPromise?: any): any;
    constructor(name: string, index: number, objects: any, parameters: any, handler: any);
}
