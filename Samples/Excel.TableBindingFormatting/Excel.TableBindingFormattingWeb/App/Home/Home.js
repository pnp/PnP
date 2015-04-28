/// <reference path="../App.js" />
/// <reference path="chance.min.js" />
/*global app*/

(function () {
    'use strict';

    var tableStyles = ["TableStyleLight1", "TableStyleLight2", "TableStyleLight3", "TableStyleLight4", "TableStyleLight5", "TableStyleLight6", "TableStyleLight7", "TableStyleLight8", "TableStyleLight9", "TableStyleLight10", "TableStyleLight11", "TableStyleLight12", "TableStyleLight13", "TableStyleLight14", "TableStyleLight15", "TableStyleLight16", "TableStyleLight17", "TableStyleLight18", "TableStyleLight19", "TableStyleLight20", "TableStyleLight21", "TableStyleMedium1", "TableStyleMedium2", "TableStyleMedium3", "TableStyleMedium4", "TableStyleMedium5", "TableStyleMedium6", "TableStyleMedium7", "TableStyleMedium8", "TableStyleMedium9", "TableStyleMedium10", "TableStyleMedium11", "TableStyleMedium12", "TableStyleMedium13", "TableStyleMedium14", "TableStyleMedium15", "TableStyleMedium16", "TableStyleMedium17", "TableStyleMedium18", "TableStyleMedium19", "TableStyleMedium20", "TableStyleMedium21", "TableStyleMedium22", "TableStyleMedium23", "TableStyleMedium24", "TableStyleMedium25", "TableStyleMedium26", "TableStyleMedium27", "TableStyleMedium28", "TableStyleDark1", "TableStyleDark2", "TableStyleDark3", "TableStyleDark4", "TableStyleDark5", "TableStyleDark6", "TableStyleDark7", "TableStyleDark8", "TableStyleDark9", "TableStyleDark10", "TableStyleDark11"];
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

    // The initialize function must be run each time a new page is loaded
    Office.initialize = function (reason) {
        $(document).ready(function () {
            app.initialize();
            CheckButtons();
            $('#AddBindings').click(CreateTable);
            $('#AddData').click(AddRowsToTable);
            $('#ClearFormat').click(ClearFormat);
            $('#ClearData').click(ClearData);
            $('#CellFormatting').click(DoCellFormatting);
            $('#TableOptions').click(DoTableOptions);

            var sel = $("#TableStyle");
            $.each(tableStyles, function (i, style) {
                $('<option />', { value: style, text: style }).appendTo(sel);
            });
        });


        function CheckButtons() {
            Office.context.document.bindings.getByIdAsync("myTable", function (e) {
                if (e.status === Office.AsyncResultStatus.Succeeded) {
                    $('#AddBindings').prop('disabled', true);
                    $('#AddData').prop('disabled', false);
                    $('#ClearFormat').prop('disabled', false);
                    $('#ClearData').prop('disabled', false);
                    $('#CellFormatting').prop('disabled', false);
                    $('#TableOptions').prop('disabled', false);
                } else {
                    $('#AddBindings').prop('disabled', false);
                    $('#AddData').prop('disabled', true);
                    $('#ClearFormat').prop('disabled', true);
                    $('#ClearData').prop('disabled', true);
                    $('#CellFormatting').prop('disabled', true);
                    $('#TableOptions').prop('disabled', true);
                }
            });
        }

        function CreateTable() {
            var numRows = parseInt($("#numRows").val(), 10);
            var myTable = new Office.TableData();
            myTable.headers = [["Number of Widgets", "Order Needed By", "Month", "Color", "Customer"]];
            myTable.rows = createSampleDataRows(numRows);
            var dateFormat = $("#dateFormat").val();

            // we use a queue for applying the cell formatting becuase of Excel online limitations
            // which restrict cell formatting calls to 100 cells or less
            rangeFormatQueue = [];
            rangeFormatQueue.push({ cells: Office.Table.All, format: { width: "auto fit" } });
            for (var j = 0; j < myTable.rows.length; j++) {
                rangeFormatQueue.push({ cells: { row: j, column: 1 }, format: { numberFormat: dateFormat } });
                rangeFormatQueue.push({ cells: { row: j, column: 3 }, format: { fontColor: myTable.rows[j][3] } });
            }


            Office.context.document.setSelectedDataAsync(myTable,
				{
				    tableOptions: { bandedRows: true, filterButton: true, style: "TableStyleLight7" }
				},
				bindTable);
        }

        // Add rows to an existing table
        function AddRowsToTable() {

            Office.context.document.bindings.getByIdAsync("myTable", function (asyncResult) {
                if (asyncResult.status === Office.AsyncResultStatus.Succeeded) {
                    var numRows = parseInt($("#numRows").val(), 10);
                    var currentTable = asyncResult.value;
                    var rows = createSampleDataRows(numRows);
                    var rowOffset = currentTable.rowCount;

                    rangeFormatQueue = [];
                    rangeFormatQueue.push({ cells: Office.Table.All, format: { width: "auto fit" } });
                    for (var j = 0; j < rows.length; j++) {
                        rangeFormatQueue.push({ cells: { row: j + rowOffset, column: 1 }, format: { numberFormat: "dd-mmm-yyyy" } });
                        rangeFormatQueue.push({ cells: { row: j + rowOffset, column: 3 }, format: { fontColor: rows[j][3] } });
                    }

                    currentTable.addRowsAsync(rows,
						function (e) {
						    if (e.status === Office.AsyncResultStatus.Failed) {
						        app.showNotification("Error", e.error.name + ": " + e.error.message);
						        rangeFormatQueue = null;
						    } else {
						        executeRangeFormatQueue();
						        CheckButtons();
						    }
						});
                }
            });
        }

        function createSampleDataRows(numRows) {
            var rows = []
            for (var i = 0; i < numRows; i++) {
                rows.push([chance.integer({ min: 1, max: 100 }),
									chance.date({ string: true }),
									months[i % 12],
									chance.color({ format: 'hex' }),
									chance.name()
                ]);
            }
            return rows;
        }

        function bindTable() {
            Office.context.document.bindings.addFromSelectionAsync(Office.BindingType.Table, { id: 'myTable' }, function (asyncResult) {

                if (asyncResult.status === Office.AsyncResultStatus.Failed) {
                    var error = asyncResult.error;
                    app.showNotification("Error", error.name + ": " + error.message);
                } else {
                    executeRangeFormatQueue()
                }
                CheckButtons();
            });
        }

        function DoCellFormatting() {
            var startRow = parseInt($("#rfStartRow").val(), 10);
            var startCol = parseInt($("#rfStartCol").val(), 10);
            var endRow = parseInt($("#rfEndRow").val(), 10);
            var endCol = parseInt($("#rfEndCol").val(), 10);

            rangeFormatQueue = getCellFormatForRange(startRow, startCol, endRow, endCol, { borderStyle: "dash dot", borderBottomStyle: "double", borderColor: "red", fontStyle: "bold" });
            executeRangeFormatQueue();
        }

        var rangeFormatQueue;
        var maxCellsToApplyFormatting = 100;
        function executeRangeFormatQueue() {
            if (rangeFormatQueue && rangeFormatQueue.length > 0) {
                var cellFormatsToApply = [];
                while (rangeFormatQueue.length > 0 && cellFormatsToApply.length < maxCellsToApplyFormatting) {
                    cellFormatsToApply.push(rangeFormatQueue.pop());
                }

                Office.select("bindings#myTable").setFormatsAsync(cellFormatsToApply,
					function (e) {
					    if (e.status === Office.AsyncResultStatus.Failed) {
					        app.showNotification("Error", e.error.name + ": " + e.error.message);
					        rangeFormatQueue = null;
					    } else {
					        executeRangeFormatQueue();
					    }
					});
            }
        }

        function getCellFormatForRange(startRow, startCol, endRow, endCol, cellFormatting) {
            var cellFormat = [];
            for (var i = startRow; i <= endRow; i++) {
                for (var j = startCol; j <= endCol; j++) {
                    cellFormat.push({ cells: { row: i, column: j }, format: cellFormatting });
                }
            }
            return cellFormat;
        }

        function ClearFormat() {
            Office.context.document.bindings.getByIdAsync("myTable", function (asyncResult) {
                if (asyncResult.status === Office.AsyncResultStatus.Succeeded) {
                    try {
                        asyncResult.value.clearFormatsAsync(
                            function (e) {
                                if (e.status === Office.AsyncResultStatus.Failed) {
                                    app.showNotification(e.error.name, e.error.message);
                                }
                            }
                        );
                    }
                    catch (err) {
                        app.showNotification("Error Occurred", err);
                    }
                } else {
                    app.showNotification(asyncResult.error.name, asyncResult.error.message);
                }
            });
        }

        function ClearData() {
            Office.context.document.bindings.getByIdAsync("myTable", function (asyncResult) {
                asyncResult.value.deleteAllDataValuesAsync(
                    function (e) {
                        if (e.status === Office.AsyncResultStatus.Failed) {
                            app.showNotification(e.error.name, e.error.message);
                        }
                    }
                );
            });

        }

        function DoTableOptions() {
            var mystyle = $("#TableStyle").val();
            var showFilter = $("#filterButtonCheck").is(":checked");
            var tableOptions = { bandedRows: true, filterButton: showFilter, style: mystyle };

            Office.context.document.bindings.getByIdAsync("myTable", function (asyncResult) {
                asyncResult.value.setTableOptionsAsync(tableOptions, function (asyncResult) { });
            });
        }
    };
})();