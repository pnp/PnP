class MockLocation implements Location {
    public hash: string;
    public host: string;
    public hostname: string;
    public href: string;
    public origin: string;
    public pathname: string;
    public port: string;
    public protocol: string;
    public search: string;

    public assign(url: string): void {
        return;
    }

    public reload(forcedReload?: boolean): void {
        return;
    }

    public replace(url: string): void {
        return;
    }
    public toString(): string {
        return "MockLocation.toString";
    }
}

export = MockLocation
