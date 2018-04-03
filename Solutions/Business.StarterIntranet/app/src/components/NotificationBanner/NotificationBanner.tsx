import * as React from "react";
import pnp, { Logger, LogLevel, spODataEntityArray, Web } from "sp-pnp-js";
import UtilityModule from "../../modules/UtilityModule";
import INotification from "./INotification";
import Notification from "./Notification";

export class NotificationBannerState {
    public notifications: INotification[];
}

// tslint:disable-next-line:max-classes-per-file
export class NotificationBannerProps {
    // No props for this component
}

// tslint:disable-next-line:max-classes-per-file
class NotificationBanner extends React.Component<NotificationBannerProps, NotificationBannerState> {

    public utilityModule: UtilityModule;

    public constructor() {
        super();

        this.utilityModule = new UtilityModule();

        this.state = {
            notifications: [],
        };
    }

    public render() {

        // Content is theorically safe here.
        const renderNotifications = this.state.notifications.map((notification, index) => {

            return <div className="message" 
                        key={ index } 
                        style={{
                            // Color values should be correct here thanks to the SharePoint column validation formula so we can apply them safely
                            backgroundColor: notification.IntranetNotificationBgColor,
                            color: notification.IntranetNotificationTextColor
                        }}
                        dangerouslySetInnerHTML={ {__html: this.utilityModule.stripScripts(notification.IntranetNotificationDescription) }}>
                        
                    </div>;
        });

        return  <div>
                    { renderNotifications }
                </div>;
    }

    public async componentDidMount() {

        try {

            const notificationsFromList = await this._getNotifications();

            this.setState({
                notifications: notificationsFromList,
            });

        } catch (errorMesssage) {
            Logger.write("[NotificationBanner._getNotifications]: " + errorMesssage, LogLevel.Error);
        }
    }

    /**
     * Gets the items from the notifications list
     */
    private async _getNotifications(): Promise<INotification[]> {

        const web = new Web(_spPageContextInfo.webAbsoluteUrl);
        const notifications = await web.getList(_spPageContextInfo.webServerRelativeUrl + "/Lists/Notifications")
            .items.select(Notification.SelectFields.toString())
            .getAs(spODataEntityArray(Notification));

        return notifications;
    }
}

export default NotificationBanner;
