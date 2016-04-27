import { IConfigurationProvider } from "../configuration";
import { ITypedHash } from "../../collections/collections";
import * as storage from "../../utils/storage";
export default class CachingConfigurationProvider implements IConfigurationProvider {
    private wrappedProvider;
    private store;
    private cacheKey;
    constructor(wrappedProvider: IConfigurationProvider, cacheKey: string, cacheStore?: storage.IPnPClientStore);
    getWrappedProvider(): IConfigurationProvider;
    getConfiguration(): Promise<ITypedHash<string>>;
    private selectPnPCache();
}
