/// <reference path="ilistinstance.d.ts" />
/// <reference path="ifile.d.ts" />
/// <reference path="icustomaction.d.ts" />
/// <reference path="ifeature.d.ts" />
interface SiteSchema {
    Lists: Array<IListInstance>;
    Files: Array<IFile>;
    Navigation: INavigation;
    CustomActions: Array<ICustomAction>;
    ComposedLook: IComposedLook;
    PropertyBagEntries: Object;
    Parameters: Object;
    WebSettings: IWebSettings;
    Features: Array<IFeature>;
}
