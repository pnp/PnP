class BaseKnockoutComponent {

    constructor(name: string, componentViewModel: Function, componentHtmlTemplate: any) {

        ko.components.register(name, {
            template: componentHtmlTemplate,
            viewModel: componentViewModel,
        });
    }
}

export default BaseKnockoutComponent;
