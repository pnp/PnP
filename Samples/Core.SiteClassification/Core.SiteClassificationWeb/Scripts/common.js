//////////////////////////////////////////////////////////////////
//
// Global Functions
//
//////////////////////////////////////////////////////////////////

function AnimatedShow(selector) {
    if ($(selector).css('display') == 'none')
        $(selector).show('fold', null, 100, null);
}
function AnimatedHide(selector) {
    if ($(selector).css('display') != 'none')
        $(selector).hide('fold', null, 100, null);
}
function Show(selector) {
    $(selector).css('display', 'block');
}
function ShowInline(selector) {
    $(selector).css('display', 'inline');
}
function Hide(selector) {
    $(selector).css('display', 'none');
}
function Disable(selector) {
    $(selector).attr('disabled', true);
    $(selector).attr('aria-disabled', true);
    $(selector).addClass("ui-button-disabled ui-state-disabled");
}
function Enable(selector) {
    $(selector).attr('disabled', false);
    $(selector).attr('aria-disabled', false);
    $(selector).removeClass("ui-button-disabled ui-state-disabled");
}

function Check(selector) {
    $(selector).attr('checked', true);
}
function Uncheck(selector) {
    $(selector).attr('checked', false);
}
function SelectedValueOf(name) {
    return $('input[name=' + name + ']:checked').val();
}
function ClearValue(selector) {
    $(selector).val("");
}
function SetValue(selector, value) {
    $(selector).val(value);
}
function ClearValidationMessage(name) {
    $('span[data-valmsg-for=' + name + ']').attr("class", "field-validation-valid");
}
function SetTooltip(itemID, gravity) {
    SetTooltip(itemID, gravity, null);
}

function SetTooltip(itemID, gravity, hoverCallback) {
    gravity = gravity || "sw";
    $("#" + itemID).tipsy({ html: true, gravity: gravity, fade: true, trigger: 'manual' });

    //Three status for hover:
    //HOVERED: the mouse is over the link/tooltip, and the tooltip is opened;
    //CLOSING: it is ready to close the tooltip;
    //OUT: the tooltip has been closed;
    $("#" + itemID).hover(
            function () {
                var currBtn = $(this);
                var _currDom = currBtn[0];
                if (_currDom.style.ishover == "CLOSING") {
                    _currDom.style.ishover = "HOVERED";
                    return;
                }
                _currDom.style.ishover = "HOVERED";
                setTimeout(function () {
                    if (_currDom.style.ishover != "HOVERED") {
                        return;
                    }
                    try {
                        currBtn.tipsy('show');
                    }
                    catch (err) { }
                }, 500);
                if (hoverCallback != null)
                    hoverCallback();
            },
            function () {
                var currBtn2 = $(this);
                var _currDom = currBtn2[0];
                _currDom.style.ishover = "CLOSING";
                setTimeout(function () {
                    if (_currDom.style.ishover == "HOVERED" || _currDom.style.ishover == "OUT") {
                        return;
                    }
                    try {
                        currBtn2.tipsy('hide');
                    }
                    catch (err) { };
                    _currDom.style.ishover = "OUT";
                }, 500);
            }
        );
}
function SetDialogStyle(itemID) {
    $('#' + itemID + '_dialog').dialog({
        autoOpen: false,
        height: 300,
        width: 515,
        modal: true,
        resizable: false,
        buttons: {
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });
    $(window).resize(function () {
        $('#' + itemID + '_dialog').dialog("option", "position", "center");
    });
    $('#' + itemID).click(function () {
        $('#' + itemID + '_dialog').dialog("open");
    });
    $('.ui-dialog-titlebar-close').remove();
}
function AddDescription(selector, description) {
    $(selector).val(description);
    $(selector).addClass("description_style");
    $(selector).focus(function () {
        var value = $(this).val();
        $(this).removeClass("description_style");
        if (value == description) {
            $(this).val("");
        }
    });
    $(selector).focusout(function () {
        var value = $(this).val().Trim();
        if (value == "") {
            $(this).val(description);
            $(this).addClass("description_style");
        }
        else {
            $(this).removeClass("description_style");
        }
    });
}

function UrlEncode(url) {
    var url2 = url;
    url2 = url2.replace(/\s/g, '%20');
    url2 = url2.replace(/\#/g, '%23');
    url2 = url2.replace(/\&/g, '%26');
    url2 = url2.replace(/\+/g, '%2B');
    url2 = url2.replace(/\=/g, '%3D');
    url2 = url2.replace(/\?/g, '%3F');
    url2 = url2.replace(/\@/g, '%40');
    return url2;
}
function StrReplace(str) {
    var str2 = str;
    str2 = str2.replace(/\s/g, "20");
    str2 = str2.replace(/\!/g, "21");
    str2 = str2.replace(/\"/g, "22");
    str2 = str2.replace(/\#/g, "23");
    str2 = str2.replace(/\%/g, "25");
    str2 = str2.replace(/\&/g, "26");
    str2 = str2.replace(/\'/g, "27");
    str2 = str2.replace(/\(/g, "28");
    str2 = str2.replace(/\)/g, "29");
    str2 = str2.replace(/\*/g, "2A");
    str2 = str2.replace(/\+/g, "2B");
    str2 = str2.replace(/\,/g, "2C");
    str2 = str2.replace(/\./g, "2E");
    str2 = str2.replace(/\//g, "2F");
    str2 = str2.replace(/\:/g, "3A");
    str2 = str2.replace(/\</g, "3C");
    str2 = str2.replace(/\=/g, "3D");
    str2 = str2.replace(/\>/g, "3E");
    str2 = str2.replace(/\?/g, "3F");
    str2 = str2.replace(/\@/g, "40");
    str2 = str2.replace(/\[/g, "5B");
    str2 = str2.replace(/\\/g, "5C");
    str2 = str2.replace(/\]/g, "5D");
    str2 = str2.replace(/\^/g, "5E");
    str2 = str2.replace(/\`/g, "60");
    str2 = str2.replace(/\{/g, "7B");
    str2 = str2.replace(/\|/g, "7C");
    str2 = str2.replace(/\}/g, "7D");
    str2 = str2.replace(/\~/g, "7E");
    return str2;
}

//////////////////////////////////////////////////////////////////
//
// Object Extensions
//
//////////////////////////////////////////////////////////////////

//time in milliseconds
var oneMinute = 60 * 1000;
var twoMinutes = oneMinute * 2;
var fortyMinutes = 40 * oneMinute;
var oneHour = 60 * oneMinute;
var oneThirty = 90 * oneMinute;
var oneDay = 24 * oneHour;
var threeDays = oneDay * 3;
var thisYear = new Date().getFullYear();
//Date extension
Date.prototype.toRelativeString = function () {
    var now = new Date();
    var timeAgo = now - this; //time in milliseconds

    if (timeAgo <= twoMinutes) {
        return "about a minute ago";
    }
    else if (timeAgo <= fortyMinutes) {
        return Math.floor(timeAgo / oneMinute) + " minutes ago";
    }
    else if (timeAgo <= oneThirty) {
        return "about an hour ago";
    }
    else if (timeAgo <= oneDay) {
        //ceiling so we don't say 1 hours ago
        return Math.ceil(timeAgo / oneHour) + " hours ago";
    }
    else if (timeAgo <= threeDays) {
        var day;

        if (now.getDay() - this.getDay() == 1) {
            day = 'Yesterday'
        }
        else {
            day = this.toDateString().slice(0, 3); //day of week
        }

        return day + ' at ' + this.toTimeCompactString()
    }
    else {
        return this.toDateTimeCompactString();
    }
}
Date.prototype.toDateTimeCompactString = function () {
    var date = this.toDateString();

    if (this.getFullYear() == thisYear) {
        //slice 0,-4: start at beginning of string and stop 4 off the end (i.e. cuts off the the year: 2010)
        date = date.slice(0, -4)
    }

    //slice off day of the week
    date = date.slice(3);

    return date + ' at ' + this.toTimeCompactString();
}
Date.prototype.toTimeCompactString = function () {
    var minutes;
    var hours = this.getHours();
    var amOrPm = 'am';

    if (hours >= 12) {
        hours = hours - 12;
        amOrPm = 'pm';
    }

    if (hours == 0) {
        hours = 12;
    }

    minutes = this.getMinutes();

    if (minutes < 10) {
        minutes = '0' + minutes;
    }
    return hours + ':' + minutes + amOrPm;
}

//String extension
String.prototype.Trim = function () {
    return this.replace(/^\s+|\s+$/g, "");
}
String.prototype.StartWith = function (prefix) {
    return this.indexOf(prefix, 0) == 0;
}
String.prototype.Contains = function (string) {
    return this.indexOf(string, 0) >= 0;
}
