"use strict";

/// <reference path="..\..\..\..\typings\main.d.ts" />

export class Logger {
    private isLoggerDefined;
    private spacing;
    private template;
    constructor() {
        this.isLoggerDefined = false;
        if (console && console.log) {
           this.isLoggerDefined = true;
        }
        this.spacing = "\t\t";
        this.template = `{0} ${this.spacing} [{1}] ${this.spacing} [{2}] ${this.spacing} {3}`;
    }
    public info(object: string, message: string): void {
        this.print(String.format(this.template, new Date(), object, "Information", message));
    }
    public debug(object: string, message: string): void {
        this.print(String.format(this.template, new Date(), object, "Debug", message));
    }
    public error(object: string, message: string): void {
       this.print(String.format(this.template, new Date(), object, "Error", message));
    }
    private print(msg: string): void {
         if (this.isLoggerDefined) {
            console.log(msg);
        }
    }
}
