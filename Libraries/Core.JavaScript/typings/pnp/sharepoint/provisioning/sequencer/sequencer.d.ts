export declare class Sequencer {
    private functions;
    private parameter;
    private scope;
    constructor(__functions: Array<any>, __parameter: any, __scope: any);
    execute(): Promise<{}>;
    private deferredArray(__functions);
}
