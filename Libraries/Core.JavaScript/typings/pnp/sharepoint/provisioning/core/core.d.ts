export declare class Core {
    private handlers;
    private options;
    private startTime;
    private queueItems;
    constructor();
    applyTemplate(path: string, _options?: IOptions): Promise<{}>;
    private start(json, queue);
}
