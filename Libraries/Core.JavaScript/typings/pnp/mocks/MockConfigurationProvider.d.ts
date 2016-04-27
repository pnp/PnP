import { IConfigurationProvider } from "../configuration/configuration";
import { ITypedHash } from "../collections/collections";
export default class MockConfigurationProvider implements IConfigurationProvider {
    mockValues: ITypedHash<string>;
    shouldThrow: boolean;
    shouldReject: boolean;
    constructor(mockValues?: ITypedHash<string>);
    getConfiguration(): Promise<ITypedHash<string>>;
}
