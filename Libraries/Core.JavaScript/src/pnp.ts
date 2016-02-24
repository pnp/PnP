import { Util } from "./Util";
import { PnPClientStorage } from "./Storage";

class PnP {
    public static util: Util = Util.$;
    public static storage: PnPClientStorage = PnPClientStorage.$;
}

export = PnP;
