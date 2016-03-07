"use strict";

/// <reference path="..\schema\icustomaction.d.ts" />
// import { Promise } from "es6-promise";
import { ObjectHandlerBase } from "../ObjectHandlerBase/ObjectHandlerBase";

export class ObjectCustomActions extends ObjectHandlerBase {
    constructor() {
        super("CustomActions");
    }
    public ProvisionObjects(customactions: Array<ICustomAction>) {
        super.scope_started();
        return new Promise((resolve, reject) => {
            const clientContext = SP.ClientContext.get_current();
            let userCustomActions = clientContext.get_web().get_userCustomActions();
            clientContext.load(userCustomActions);
            clientContext.executeQueryAsync(
                () => {
                    customactions.forEach((obj) => {
                        let objExists = jQuery.grep(userCustomActions.get_data(), (userCustomAction) => {
                            return userCustomAction.get_title() === obj.Title;
                        }).length > 0;
                        if (!objExists) {
                            let objCreationInformation = userCustomActions.add();
                            if (obj.Description) { objCreationInformation.set_description(obj.Description); }
                            if (obj.CommandUIExtension) { objCreationInformation.set_commandUIExtension(obj.CommandUIExtension); }
                            if (obj.Group) { objCreationInformation.set_group(obj.Group); }
                            if (obj.Title) { objCreationInformation.set_title(obj.Title); }
                            if (obj.Url) { objCreationInformation.set_url(obj.Url); }
                            if (obj.ScriptBlock) { objCreationInformation.set_scriptBlock(obj.ScriptBlock); }
                            if (obj.ScriptSrc) { objCreationInformation.set_scriptSrc(obj.ScriptSrc); }
                            if (obj.Location) { objCreationInformation.set_location(obj.Location); }
                            if (obj.ImageUrl) { objCreationInformation.set_imageUrl(obj.ImageUrl); }
                            if (obj.Name) { objCreationInformation.set_name(obj.Name); }
                            if (obj.RegistrationId) { objCreationInformation.set_registrationId(obj.RegistrationId); }
                            if (obj.RegistrationType) { objCreationInformation.set_registrationType(obj.RegistrationType); }
                            if (obj.Rights) { objCreationInformation.set_rights(obj.Rights); }
                            if (obj.Sequence) { objCreationInformation.set_sequence(obj.Sequence); }
                            objCreationInformation.update();
                        }
                    });
                    clientContext.executeQueryAsync(
                        () => {
                            super.scope_ended();
                            resolve();
                        }, () => {
                            super.scope_ended();
                            resolve();
                        });
                }, () => {
                    super.scope_ended();
                    resolve();
                });
        });
    }
}
