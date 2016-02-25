"use strict";

import * as Util from "./Utils/Util";
import { PnPClientStorage } from "./Utils/Storage";

class PnP {
    public static util = Util;
    public static storage: PnPClientStorage = PnPClientStorage.$;
}

export = PnP;
