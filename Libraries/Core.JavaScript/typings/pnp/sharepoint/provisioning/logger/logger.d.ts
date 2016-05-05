export declare class Logger {
    private isLoggerDefined;
    private spacing;
    private template;
    constructor();
    info(object: string, message: string): void;
    debug(object: string, message: string): void;
    error(object: string, message: string): void;
    private print(msg);
}
