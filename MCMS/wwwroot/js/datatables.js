function bindDefaultDataTables(tableElem, url, initialConfig, actionsColumnContent, hasStaticIndexColumn, lang) {
    initialConfig.columns = initialConfig.columns.slice();
    var shouldMakeActionsCellContent = actionsColumnContent.trim().length > 0;
    var langConfig = {
        'en': {"url": "/lib/datatables/English.json"},
        'ro': {"url": "/lib/datatables/Romanian.json"}
    };

    var config = {
        processing: true,
        ajax: {
            url: url,
            dataSrc: function (json) {
                for (var i = 0; i < json.length; i++) {
                    if (hasStaticIndexColumn) {
                        json[i].dataTablesIndex = '#';
                    }
                    if (shouldMakeActionsCellContent) {
                        json[i]._actions = actionsColumnContent.replace(/ENTITY_ID/g, json[i].id);
                    }
                }
                return json;
            },
            error: function (jqXHR, textStatus, errorThrown, c) {
                if (errorThrown === 'abort') {
                    return;
                }
                var msg = (jqXHR.responseJSON ? jqXHR.responseJSON.error : jqXHR.responseText) || "An error occurred while trying to access data. Please try again.";
                alert(msg);
                tableJQuery._fnProcessingDisplay(false);
            }
        },
        bAutoWidth: false,
        iDisplayLength: 50,
        lengthMenu: [[10, 25, 50, 100, 250, 500, -1], [10, 25, 50, 100, 250, 500, "All"]],
        fixedHeader: {
            headerOffset: 50
        },
        language: langConfig[lang],
        dom: "<'processing-overlay'><'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'<f>>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 d-flex justify-content-between'pB>>",
        buttons: [
            {
                text: '<i class="fas fa-grip-lines-vertical fa-fw"></i><i class="fas fa-search fa-fw"></i>',
                className: 'btn-light btn-outline-info',
                action: function (e, dt, node, config) {
                    toggleDataTablesColumnSearch(tableElem, initialConfig.columns);
                }
            },
            {
                text: '<i class="fas fa-sync-alt"></i>',
                className: 'btn-light btn-outline-info',
                action: function (e, dt, node, config) {
                    dt.ajax.reload();
                }
            }
        ]
    };

    config = Object.assign(config, initialConfig);

    if (hasStaticIndexColumn) {
        config.columns.splice(0, 0, {"data": "dataTablesIndex"});
        config.columnDefs = [{
            searchable: false,
            orderable: false,
            targets: 0
        }];
        config.aaSorting = [];
    }
    sumTotalRowIfNeeded(config);

    var tableJQuery = tableElem.dataTable(config);
    var table = tableJQuery.api();

    if (hasStaticIndexColumn) {
        table.on('order.dt search.dt', function () {
            table.column(0, {search: 'applied', order: 'applied'}).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
    }

    table.on('processing.dt', function (e, settings, processing) {
        var overlay = $(e.currentTarget).closest('.dataTables_wrapper').find('.processing-overlay');
        if (processing) {
            overlay.show();
        } else {
            overlay.hide();
        }
    });

    properlyDestroyInModal(tableElem, table);

    return table;
}

function sumTotalRowIfNeeded(config) {
    if (!config.columns.some(function (c) {
        return c.sumTotal;
    })) {
        return;
    }
    var oldDrawCallback = config.drawCallback;
    config.drawCallback = function () {
        if (oldDrawCallback) {
            oldDrawCallback.call(this);
        }
        var row = this.find(".sum-total-row");
        if (!row.data("build")) {
            row.toggle().data("build", true);
            var cols = row.find('th')
            cols.each(function (index) {
                $(this).html('');
            });
            cols.first().html("Total");
        }
        var api = this.api();
        api.columns().every(function (index) {
            var sumTotalOpt = config.columns[index].sumTotal;
            if (!sumTotalOpt)
                return;
            var sum = "";
            if (sumTotalOpt === true) {
                sum = api.column(index, {page: 'current'}).data().sumTotal;
            } else {
                var data = api.column(index, {page: 'current'}).data();
                if (typeof sumTotalOpt === "string" && data.hasOwnProperty(sumTotalOpt)) {
                    sum = data[sumTotalOpt];
                }
            }
            if (typeof sum === 'function') {
                sum = sum();
            }
            row.children().eq(index).html(sum + "");
        });
    }
}

function toggleDataTablesColumnSearch(table, columnsConfig) {
    var searchRow = table.find('tfoot tr.column-search-row');
    searchRow.toggle();
    if (!searchRow.data('build')) {
        searchRow.data('build', true)

        var searchCells = searchRow.find('td');
        searchCells.each(function (index) {
            if (!columnsConfig[index].searchable) {
                $(this).html('');
                return;
            }
            var title = $(this).text();
            $(this).html('&nbsp;<input type="text" placeholder="Search ' + title + '" />');
        });

        table.dataTable().api().columns().every(function (index) {
            var column = this;
            $(searchCells[index]).find("input").on('keyup change clear', function () {
                if (column.search() !== this.value) {
                    column.search(this.value).draw();
                }
            });
        });
    }
}

function properlyDestroyInModal(tableElem, datatable) {
    var modal = tableElem.closest(".modal");
    if (!modal.length) {
        return;
    }
    modal.on('hide.bs.modal', function () {
        datatable.destroy();
    });
}

jQuery.fn.dataTable.Api.register('sumTotal', function () {
    return this.flatten().reduce(function (a, b) {
        if (typeof a === 'string') {
            a = a.replace(/[^\d.-]/g, '') * 1;
        }
        if (typeof b === 'string') {
            b = b.replace(/[^\d.-]/g, '') * 1;
        }
        return a + b;
    }, 0);
});