// import { Promise } from "es6-promise";

export class ProvisioningStep {
    private name: string;
    private index: number;
    private objects: any;
    private parameters: any;
    private handler: any;

    public execute(dependentPromise?) {
        let _handler = new this.handler();
        if (!dependentPromise) {
            return _handler.ProvisionObjects(this.objects, this.parameters);
        }
        return new Promise((resolve, reject) => {
            dependentPromise.then(() => {
                return _handler.ProvisionObjects(this.objects, this.parameters).then(resolve, resolve);
            });
        });
    }

    constructor(name: string, index: number, objects: any, parameters: any, handler: any) {
        this.name = name;
        this.index = index;
        this.objects = objects;
        this.parameters = parameters;
        this.handler = handler;
    }
}
