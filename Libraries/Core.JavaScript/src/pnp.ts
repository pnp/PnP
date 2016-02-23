import { Core } from "./Core";
import { PnPClientStorage } from "./Storage";

class PnP {
    public static core: Core = Core.$;
    public static storage: PnPClientStorage = PnPClientStorage.$;
}

export = PnP;
