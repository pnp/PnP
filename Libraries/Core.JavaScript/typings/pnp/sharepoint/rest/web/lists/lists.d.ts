import { Queryable } from "../../Queryable";
export declare class Lists extends Queryable {
    constructor(url: Array<string>);
    getByTitle(title: string): any;
    getById(id: string): any;
}
