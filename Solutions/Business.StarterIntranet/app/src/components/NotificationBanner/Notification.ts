// ========================================
// Represents a news alert notification
// ========================================
import INotification from "./INotification";

class Notification implements INotification {

    public static SelectFields = [
        "Title",
        "IntranetNotificationDescription",
        "IntranetNotificationBgColor",
        "IntranetNotificationTextColor"
    ];

    public Title: string;
    public IntranetNotificationDescription: string;
    public IntranetNotificationBgColor: string;
    public IntranetNotificationTextColor: string;   
}

export default Notification;