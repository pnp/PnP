import { Core } from "./Core";
import { PnPClientStorage } from "./Storage";

export class pnp {
    public static core: Core = Core.$;
    public static storage: PnPClientStorage = PnPClientStorage.$;
}
