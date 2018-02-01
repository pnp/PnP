// ========================================
// Represents a news alert notification
// ========================================
import INotification from "./INotification";

class Notification implements INotification {

    public static SelectFields = [
        "Title",
        "IntranetNotificationDescription",
    ];

    public Title: string;
    public IntranetNotificationDescription: string;
}

export default Notification;