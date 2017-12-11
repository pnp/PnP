// ========================================
// Welcome overlay control
// ========================================
import * as i18n from "i18next";
import { Logger, LogLevel, Web } from "sp-pnp-js";
import * as sprintf from "sprintf-js";

class WelcomeOverlayViewModel {

    public welcomeMessage: KnockoutObservable<string>;

    constructor() {

        this.welcomeMessage = ko.observable("");

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);

        // Get the current user name
        web.getUserById(_spPageContextInfo.userId).get().then((user) => {

            this.welcomeMessage(sprintf.sprintf(i18n.t("welcomeMessage"), user.Title.split(" ")[0]));

        }).catch((errorMesssage) => {

            Logger.write(errorMesssage, LogLevel.Error);
        });
    }
}

export default WelcomeOverlayViewModel;
