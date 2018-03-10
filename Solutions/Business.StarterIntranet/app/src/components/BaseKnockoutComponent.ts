class BaseKnockoutComponent {

    // tslint:disable-next-line:ban-types
    constructor(name: string, componentViewModel: Function, componentHtmlTemplate: any) {

        ko.components.register(name, {
            template: componentHtmlTemplate,
            viewModel: componentViewModel,
        });
    }
}

export default BaseKnockoutComponent;
