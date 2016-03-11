"use strict";

import { Queryable } from "../../Queryable";

export class RoleAssignments extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/RoleAssignments");
    }
}
