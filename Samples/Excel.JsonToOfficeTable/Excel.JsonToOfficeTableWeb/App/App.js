// The initialize function must be run each time a new page is loaded
Office.initialize = function (reason) {
    //extension to Office.TableData to add headers
    Office.TableData.prototype.addHeaders = function (obj) {
        var h = new Array();
        for (var prop in obj) {
            //ignore complex types empty columns and __type from WCF
            if (typeof (obj[prop]) != 'object' &&
                prop.trim().length > 0 &&
                prop != '__type')
                h.push(prop);
        }
        this.headers = h;
    }

    //extension to Office.TableData to add a range of rows
    Office.TableData.prototype.addRange = function (array) {
        for (i = 0; i < array.length; i++) {
            var itemsTemp = new Array();
            $(this.headers[0]).each(function (ii, ee) {
                itemsTemp.push(array[i][ee]);
            });
            this.rows.push(itemsTemp);
        }
    }

    $(document).ready(function () {
        //initalize selected items
        $('#nav' + $('#hdnActiveTab').val()).addClass('selected');
        $('#content' + $('#hdnActiveTab').val()).addClass('selected');

        //check for json data loaded during server-side processing
        if (typeof jsonData == 'object') {
            //initalize the Office.TableData and load headers/rows from data
            var officeTable = new Office.TableData();
            officeTable.addHeaders(jsonData[0]);
            officeTable.addRange(jsonData);
            setExcelData(officeTable);
        }

        //wire up navigation
        $('.navItem').click(function () {
            //change the selected item
            $('.navItem').removeClass('selected');
            $('.content').removeClass('selected');
            $(this).addClass('selected');
            $('#content' + this.id.substring(3)).addClass('selected');
            $('#hdnActiveTab').val(this.id.substring(3));
        });

        //hide the message
        $('#message').click(function () { $('#message').hide(); });

        //wire up client-side processing
        $('#btnSubmit1').click(function () {
            $.ajax({
                url: '../Services/Stocks.svc/GetHistory?stock=' + $('#txtSymbol1').val() + '&fromyear=' + $('#cboFromYear1').val(),
                method: 'GET',
                success: function (data) {
                    //initalize the Office.TableData and load headers/rows from data
                    var officeTable = new Office.TableData();
                    officeTable.addHeaders(data.d[0]);
                    officeTable.addRange(data.d);
                    setExcelData(officeTable);
                },
                error: function (err) {
                    showMessage('Error calling Stock Service');
                }
            });
            return false;
        });
    });
};

//write the TableData to Excel
function setExcelData(officeTable) {
    if (officeTable != null) {
        Office.context.document.setSelectedDataAsync(officeTable, { coercionType: Office.CoercionType.Table }, function (asyncResult) {
            if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                showMessage('Set Selected Data Failed');
            }
            else {
                showMessage('Set Selected Data Success');
            }
        });
    }
}

//show message
function showMessage(txt) {
    $('#message').html(txt);
    $('#message').show();
}