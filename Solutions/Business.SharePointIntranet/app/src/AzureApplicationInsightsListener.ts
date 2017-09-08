import { AppInsights } from "applicationinsights-js";
import { LogEntry, Logger, LogLevel, LogListener } from "sp-pnp-js";

class AzureApplicationInsightsListener implements LogListener {

    public log(entry: LogEntry): void {

        switch (entry.level) {

            case LogLevel.Error:
                AppInsights.trackException(new Error(entry.message), null, { Severity: "Error" });
                break;

            case LogLevel.Warning:
                AppInsights.trackException(new Error(entry.message), null, { Severity: "Warning" });
                break;

            case LogLevel.Info:
                AppInsights.trackException(new Error(entry.message), null, { Severity: "Info" });
                break;

            default:
                AppInsights.trackException(new Error(entry.message), null, { Severity: "None" });
                break;
        }
    }
}

export default AzureApplicationInsightsListener;
