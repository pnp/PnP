import {IConfigurationProvider} from "configuration";
import {ITypedHash} from "../collections/collections";
import * as storage from "../../utils/storage";
import {Promise} from "es6-promise";

export default class CachingConfigurationProvider implements IConfigurationProvider {
    private wrappedProvider: IConfigurationProvider;
    private store: storage.IPnPClientStore;
    private cacheKey: string;

    constructor(wrappedProvider: IConfigurationProvider, cacheKey: string, cacheStore?: storage.IPnPClientStore) {
        this.wrappedProvider = wrappedProvider;
        this.store = (cacheStore) ? cacheStore : this.selectPnPCache();
        this.cacheKey = `_configcache_${ cacheKey }`;
    }

    public getWrappedProvider(): IConfigurationProvider {
        return this.wrappedProvider;
    }

    public getConfiguration(): Promise<ITypedHash<string>> {
        // Cache not available, pass control to  the wrapped provider
        if ((! this.store) || (!this.store.enabled)) {
            return this.wrappedProvider.getConfiguration();
        }

        // Value is found in cache, return it directly
        let cachedConfig = this.store.get(this.cacheKey);
        if (cachedConfig) {
            return new Promise<ITypedHash<string>>((resolve, reject) => {
                resolve(cachedConfig);
            });
        }

        // Get and cache value from the wrapped provider
        let providerPromise = this.wrappedProvider.getConfiguration();
        providerPromise.then((providedConfig) => {
            this.store.put(this.cacheKey, providedConfig);
        });
        return providerPromise;
    }

    private selectPnPCache(): storage.IPnPClientStore {
        let pnpCache = new storage.PnPClientStorage();
        if ((pnpCache.local) && (pnpCache.local.enabled)) {
            return pnpCache.local;
        }
        if ((pnpCache.session) && (pnpCache.session.enabled)) {
            return pnpCache.session;
        }
        throw new Error("Cannot create a caching configuration provider since cache is not available.");
    }
}
