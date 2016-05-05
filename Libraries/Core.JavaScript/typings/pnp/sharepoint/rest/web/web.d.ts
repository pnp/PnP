import { Queryable } from "../Queryable";
import { Lists } from "./Lists/Lists";
export declare class Web extends Queryable {
    constructor(url: Array<string>);
    lists: Lists;
}
