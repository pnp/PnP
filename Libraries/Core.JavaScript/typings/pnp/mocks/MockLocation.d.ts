declare class MockLocation implements Location {
    hash: string;
    host: string;
    hostname: string;
    href: string;
    origin: string;
    pathname: string;
    port: string;
    protocol: string;
    search: string;
    assign(url: string): void;
    reload(forcedReload?: boolean): void;
    replace(url: string): void;
    toString(): string;
}
export = MockLocation;
