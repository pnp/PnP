import { IConfigurationProvider } from "../configuration";
import { ITypedHash } from "../../collections/collections";
import { default as CachingConfigurationProvider } from "./cachingConfigurationProvider";
export default class SPListConfigurationProvider implements IConfigurationProvider {
    private webUrl;
    private listTitle;
    constructor(webUrl: string, listTitle?: string);
    getWebUrl(): string;
    getListTitle(): string;
    getConfiguration(): Promise<ITypedHash<string>>;
    asCaching(): CachingConfigurationProvider;
}
