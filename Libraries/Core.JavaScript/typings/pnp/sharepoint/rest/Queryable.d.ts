/**
 * Queryable Base Class
 *
 */
export declare class Queryable {
    _url: Array<string>;
    _query: Array<string>;
    constructor(base: Array<string>, component: string);
    select(select: Array<string>): this;
    filter(filter: string): this;
    url(): string;
    get(): Promise<{}>;
}
