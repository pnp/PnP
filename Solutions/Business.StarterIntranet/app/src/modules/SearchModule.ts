import { Logger, LogLevel, Util, Web} from "sp-pnp-js";
import ISearchNavigationNode from "../models/ISearchNavigationNode";
import SearchNavigationNode from "../models/SearchNavigationNode";
class SearchModule {

    public getSearchNavigationSettings(): Promise<ISearchNavigationNode[]> {

        // Get the search navigation nodes
        const fetchUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/web/Navigation/GetNodeById(1040)/Children";

        const p = new Promise<ISearchNavigationNode[]>((resolve, reject) => {

            fetch(fetchUrl, {
                credentials: "same-origin",
                headers: {
                    Accept: "application/json; odata=verbose",
                },
                method: "GET",
            }).then((response) => {

                if (response.ok) {

                    response.json().then((data: any) => {

                        const nodes: ISearchNavigationNode[] =  data.d.results.map((elt) => {

                            let url = elt.Url;
                            if (!Util.isUrlAbsolute(url)) {

                                // IE fix
                                let origin = window.location.origin;
                                if (!origin) {
                                    origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ":" + window.location.port : "");
                                }

                                url = Util.combinePaths(origin, url);
                            }

                            return new SearchNavigationNode(elt.Title, url);
                        });

                        resolve(nodes);
                    });

                } else {
                    Logger.write("[SearchModule.getSearchNavigationSettings()] Error: " + response.statusText, LogLevel.Error);
                    reject();
                }
            });
        });

        return p;
    }
}

export default SearchModule;
