import {IConfigurationProvider} from "../configuration";
import {ITypedHash} from "../../collections/collections";
import {Promise} from "es6-promise";
import { default as CachingConfigurationProvider } from "./cachingConfigurationProvider";
import * as ajax from "../../Utils/Ajax";

export default class SPListConfigurationProvider implements IConfigurationProvider {
    constructor(private webUrl: string, private listTitle = "config") {
    }

    public getWebUrl(): string {
        return this.webUrl;
    }

    public getListTitle(): string {
        return this.listTitle;
    }

    public getConfiguration(): Promise<ITypedHash<string>> {
        return new Promise((resolve, reject) => {
            let url = `${ this.webUrl }/_api/web/lists/getByTitle('${ this.listTitle }')/items?$select=Title,Value`;
            ajax.get(url).success(data => {
                let results: any = (data.d.hasOwnProperty("results")) ? data.d.results : data.d;
                let configuration: ITypedHash<string> = {};
                results.forEach(i => {
                    configuration[i.Title] = i.Value;
                });
                resolve(configuration);
            });
        });
    }

    public asCaching(): CachingConfigurationProvider {
        let cacheKey = `splist_${ this.webUrl}+${ this.listTitle }`;
        return new CachingConfigurationProvider(this, cacheKey);
    }
}
