import { Queryable } from "../../../Queryable";
export declare class Items extends Queryable {
    constructor(url: Array<string>);
    getById(id: number): this;
}
