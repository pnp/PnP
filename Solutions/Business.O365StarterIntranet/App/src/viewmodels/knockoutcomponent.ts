export class KnockoutComponent {

    constructor(name: string, viewModel: Function, template: any) {

        ko.components.register(name, {
            template: template,
            viewModel: viewModel,
        });
    }
}
