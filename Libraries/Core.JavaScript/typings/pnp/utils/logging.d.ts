import Collections = require("../collections/Collections");
/**
 * A set of logging levels
 *
 */
export declare enum LogLevel {
    Verbose = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    Off = 99,
}
/**
 * Interface that defines a log entry
 *
 */
export interface ILogEntry {
    /**
     * The main message to be logged
     */
    message: string;
    /**
     * The level of information this message represents
     */
    level: LogLevel;
    /**
     * Any associated data that a given logging listener may choose to log or ignore
     */
    data?: Collections.ITypedHash<string>;
}
/**
 * Interface that defines a log listner
 *
 */
export interface ILogListener {
    /**
     * Any associated data that a given logging listener may choose to log or ignore
     *
     * @param entry The information to be logged
     */
    log(entry: ILogEntry): void;
}
/**
 * Class used to subscribe ILogListener and log messages throughout an application
 *
 */
export declare class Logger {
    activeLogLevel: LogLevel;
    private subscribers;
    /**
     * Creates a new instance of the Logger class
     *
     * @constructor
     * @param activeLogLevel the level used to filter messages (Default: LogLevel.Warning)
     * @param subscribers [Optional] if provided will initialize the array of subscribed listeners
     */
    constructor(activeLogLevel?: LogLevel, subscribers?: ILogListener[]);
    /**
     * Adds an ILogListener instance to the set of subscribed listeners
     *
     */
    subscribe(listener: ILogListener): void;
    /**
     * Gets the current subscriber count
     */
    count(): number;
    /**
     * Writes the supplied string to the subscribed listeners
     *
     * @param message The message to write
     * @param level [Optional] if supplied will be used as the level of the entry (Default: LogLevel.Verbose)
     */
    write(message: string, level?: LogLevel): void;
    /**
     * Logs the supplied entry to the subscribed listeners
     *
     * @param entry The message to log
     */
    log(entry: ILogEntry): void;
    /**
     * Logs performance tracking data for the the execution duration of the supplied function using console.profile
     *
     * @param name The name of this profile boundary
     * @param f The function to execute and track within this performance boundary
     */
    measure<T>(name: string, f: () => T): T;
}
/**
 * Implementation of ILogListener which logs to the browser console
 *
 */
export declare class ConsoleListener implements ILogListener {
    /**
     * Any associated data that a given logging listener may choose to log or ignore
     *
     * @param entry The information to be logged
     */
    log(entry: ILogEntry): void;
    /**
     * Formats the message
     *
     * @param entry The information to format into a string
     */
    private format(entry);
}
/**
 * Implementation of ILogListener which logs to Azure Insights
 *
 */
export declare class AzureInsightsListener implements ILogListener {
    private azureInsightsInstrumentationKey;
    /**
     * Creats a new instance of the AzureInsightsListener class
     *
     * @constructor
     * @param azureInsightsInstrumentationKey The instrumentation key created when the Azure Insights instance was created
     */
    constructor(azureInsightsInstrumentationKey: string);
    /**
     * Any associated data that a given logging listener may choose to log or ignore
     *
     * @param entry The information to be logged
     */
    log(entry: ILogEntry): void;
    /**
     * Formats the message
     *
     * @param entry The information to format into a string
     */
    private format(entry);
}
/**
 * Implementation of ILogListener which logs to the supplied function
 *
 */
export declare class FunctionListener implements ILogListener {
    private method;
    /**
     * Creates a new instance of the FunctionListener class
     *
     * @constructor
     * @param  method The method to which any logging data will be passed
     */
    constructor(method: (entry: ILogEntry) => void);
    /**
     * Any associated data that a given logging listener may choose to log or ignore
     *
     * @param entry The information to be logged
     */
    log(entry: ILogEntry): void;
}
