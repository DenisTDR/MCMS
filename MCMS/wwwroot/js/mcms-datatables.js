var mcmsDatatables = {
    bindDefaultDataTables: function (selector, initialConfig, actionsColumnContent, lang) {
        var tableId = selector.replace("#", "");
        var tableElem = $(selector);

        initialConfig.columns = initialConfig.columns.slice();
        var shouldMakeActionsCellContent = actionsColumnContent && actionsColumnContent.trim().length > 0;

        var initialPatchRowData = function (rowData) {
            if (shouldMakeActionsCellContent) {
                rowData._actions = actionsColumnContent.replace(/ENTITY_ID/g, rowData.id);
            }
        }

        var config = {
            stateSave: true,
            processing: true,
            ajax: {
                dataSrc: function (json) {
                    var data = json.data ? json.data : json;
                    if (shouldMakeActionsCellContent) {
                        for (var i = 0; i < data.length; i++) {
                            initialPatchRowData(data[i]);
                        }
                    }
                    if (initialConfig.serverSide) {
                        for (var i = 0; i < data.length; i++) {
                            data[i]._index = i + 1;
                        }
                    }
                    return data;
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
            lengthMenu: [[10, 25, 50, 100, 250, 500, 1000, -1], [10, 25, 50, 100, 250, 500, 1000, "All"]],
            fixedHeader: {headerOffset: 50},
            language: mcmsDatatables.getLang(lang),
            dom: "<'processing-overlay'><'row'<'col-sm-12 col-md-6 table-actions-container'><'col-sm-12 col-md-6'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-12 batch-actions-container'>>" +
                "<'row footer-table-row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 d-flex justify-content-between'pB>>",
            buttons: [
                {
                    text: '<i class="fas fa-sync-alt"></i>',
                    className: 'btn-light btn-outline-info',
                    action: function (e, dt, node, config) {
                        dt.ajax.reload();
                    }
                }
            ]
        };

        config = deepmerge(initialConfig, config);

        if (config.serverSide) {
            config.ajax = deepmerge(config.ajax, {
                type: "POST",
                contentType: 'application/json',
                data: function (d) {
                    return JSON.stringify(d);
                },
            })
        }
        console.log(config.searchDelay);

        if (config.hasStaticIndexColumn || config.checkboxSelection) {
            config.aaSorting = [];
        }
        if (config.checkboxSelection) {
            config.select = {
                style: 'multi+shift',
                // style: 'os',
                className: 'row-selected',
                selector: 'td:first-child'
            };
        }

        var tableJQuery = tableElem.dataTable(config);
        var table = tableJQuery.api();

        mcmsDatatables.sumTotalRowIfNeeded(config, table, tableJQuery);

        table.mcms = {
            id: tableId, $: tableJQuery, callbacks: {},
            customMethods: {initialPatchRowData: initialPatchRowData}
        };

        if (!config.skipDefaultModalEventHandlers) {
            mcmsDatatables.bindDefaultModalEventHandlers(table);
        }
        if (config.enableColumnSearch) {
            mcmsDatatables.enableColumnSearchRow(config, table);
        }
        if (config.hasStaticIndexColumn && !config.serverSide) {
            var staticIndexColumnIndex = config.checkboxSelection ? 1 : 0;
            table.on('order.dt search.dt', function () {
                table.column(staticIndexColumnIndex, {
                    search: 'applied',
                    order: 'applied'
                }).nodes().each(function (cell, i) {
                    cell.innerHTML = i + 1;
                });
            });
        }

        if (config.checkboxSelection) {
            mcmsDatatables.enableBatchActions(table, tableJQuery, config);
        }
        if (config.tableActions && config.tableActions.length) {
            mcmsDatatables.enableTableActions(table, tableJQuery, config);
        }

        table.on('processing.dt', function (e, settings, processing) {
            tableJQuery.closest('.dataTables_wrapper').find('.processing-overlay').toggle(processing);
        });

        mcmsDatatables.properlyDestroyInModal(tableElem, table);

        document.addEventListener('side-menu-toggled', function () {
            table.fixedHeader.adjust();
            setTimeout(function () {
                table.fixedHeader.adjust();
            }, 500);
        });
        mcmsTables.push(table);
        table.on('destroy', function () {
            mcmsTables.splice(mcmsTables.indexOf(table), 1);
        });
        return table;
    },
    getLang: function (lang) {
        var langConfigBasePath = typeof basePath !== 'undefined' ? basePath : '';
        var langConfig = {
            'en': {"url": langConfigBasePath + "/lib/datatables/English.json"},
            'ro': {"url": langConfigBasePath + "/lib/datatables/Romanian.json"}
        };
        return langConfig[lang];
    },
    sumTotalRowIfNeeded: function (config, tableApi, tableJQuery) {
        var sumTotalCols = config.columns.map(function (c, i) {
            return {idx: i, sumTotal: c.sumTotal};
        }).filter(function (c) {
            return c.sumTotal;
        });
        if (!sumTotalCols.length) {
            return;
        }
        tableApi.on('init', function () {
            var sumTotalRow = tableApi.footer().toJQuery().find('tr.sum-total-row');
            if (!sumTotalRow.length) {
                return;
            }

            try {
                var sumTotalFooterRowIndex = tableApi.footer().toJQuery().find('tr.sum-total-row').index();
                var sumTotalFooterObjects = tableApi.settings()[0].aoFooter[sumTotalFooterRowIndex];
                if (config.checkboxSelection || config.hasStaticIndexColumn) {
                    sumTotalFooterObjects[Math.max(!!config.checkboxSelection + !!config.hasStaticIndexColumn - 1, 0)].cell.innerHTML = "Total";
                }
            } catch (e) {
            }

            sumTotalRow.toggle();
            tableApi.on('draw', function () {
                tableJQuery.trigger('calcSumTotal.mcms');
            });
            tableJQuery.trigger('calcSumTotal.mcms');

        });
        tableApi.on('calcSumTotal.mcms', function () {
            var sumTotalFooterRowIndex = tableApi.footer().toJQuery().find('tr.sum-total-row').index();
            var sumTotalFooterObjects = tableApi.settings()[0].aoFooter[sumTotalFooterRowIndex];
            for (var i = 0; i < sumTotalCols.length; i++) {
                var colConfig = sumTotalCols[i];
                var col = tableApi.column(colConfig.idx);
                if (!col.visible())
                    return;
                var sumTotalOpt = colConfig.sumTotal;
                var sum = '';
                var visibleColData = tableApi.column(colConfig.idx, {page: 'current'}).data();
                if (sumTotalOpt === true) {
                    sum = visibleColData.sumTotal;
                } else {
                    if (typeof sumTotalOpt === "string" && visibleColData.hasOwnProperty(sumTotalOpt)) {
                        sum = visibleColData[sumTotalOpt];
                    }
                }
                if (typeof sum === 'function') {
                    sum = sum();
                }
                sumTotalFooterObjects[colConfig.idx].cell.innerHTML = sum + "";
            }
        });
    },
    toggleColumnSearchRow: function (tableApi, columnsConfig, tableConfig) {
        var searchRow = tableApi.footer().toJQuery().find('tr.column-search-row');
        searchRow.toggle();
        if (!searchRow.data('build')) {
            searchRow.data('build', true)
            var searchFooterRowIndex = searchRow.index();

            var debounceId = -1;

            var searchFooterObjects = tableApi.settings()[0].aoFooter[searchFooterRowIndex];
            for (var i = 0; i < searchFooterObjects.length; i++) {
                var cell = $(searchFooterObjects[i].cell);
                var colConfig = columnsConfig[i];
                if (!colConfig.searchable) {
                    cell.html('');
                    continue;
                }
                var title = cell.text();
                var input = mcmsDatatables.buildInputElementForColumnSearch(colConfig, title, tableConfig.serverSide);
                var col = tableApi.column(i);
                var currentSearch = col.search();
                if (currentSearch) {
                    input.val(currentSearch);
                }
                input.data('colApi', col);
                input.on('keyup change clear', function () {
                    var col = $(this).data('colApi');
                    var val = this.value;
                    if (col.search() !== val) {
                        if (tableConfig.serverSide) {
                            var localDebounceId = Math.floor(Math.random() * 10000);
                            debounceId = localDebounceId;
                            setTimeout(function () {
                                if (debounceId !== localDebounceId) return;
                                col.search(val).draw();
                            }, tableConfig.searchDelay);
                        } else {
                            col.search(val).draw();
                        }
                    }
                });
                cell.html('&nbsp;').append(input);
            }
        }
    },
    buildInputElementForColumnSearch: function (colConfig, placeholder, serverSide) {
        if (serverSide) {
            switch (colConfig.mType) {
                case 'bool':
                case 'select':
                    var elem = $('<select>');
                    if (colConfig.mFilterValues) {
                        for (var i = 0; i < colConfig.mFilterValues.length; i++) {
                            elem.append('<option value="' + colConfig.mFilterValues[i].value + '">' + colConfig.mFilterValues[i].label + '</option>');
                        }
                    }
                    return elem;
            }
        }
        return $('<input type="' + (colConfig.mType === 'number' ? 'number' : 'text') + '" placeholder="🔍 ' + placeholder + '" />');
    },
    enableColumnSearchRow: function (config, tableApi) {
        config.buttons.splice(0, 0,
            {
                text: '<i class="fas fa-grip-lines-vertical fa-fw"></i>🔍',
                className: 'btn-light btn-outline-info',
                action: function (e, dt, node, conf) {
                    mcmsDatatables.toggleColumnSearchRow(tableApi, config.columns, config);
                }
            });
        var search = tableApi.columns().search();
        //if there is any search in at least one column, then toggle (show) the row right now
        for (var i = 0; i < search.length; i++) {
            if (search[i]) {
                mcmsDatatables.toggleColumnSearchRow(tableApi, config.columns, config);
                break;
            }
        }
    },
    properlyDestroyInModal: function (tableElem, datatable) {
        var modal = tableElem.closest(".modal");
        if (!modal.length) {
            return;
        }
        modal.on('hide.bs.modal', function () {
            datatable.destroy();
        });
    },
    enableCheckboxSelection: function (table, tableJQuery) {
        table.header().toJQuery().add(table.footer().toJQuery()).find("th.select-all-checkbox").on('click', function () {
            if (table.rows({selected: true}).count() === table.rows().count()) {
                table.rows().deselect();
            } else {
                table.rows({search: 'applied'}).select();
            }
        });
        table.on('select deselect', function (e, dt, type, indexes) {
            table.updateSelectAllCheckbox(table, tableJQuery);
        });
        table.on('xhr', function (e, dt, type, indexes) {
            table.mcms.dataJustUpdated = true;
        });
        table.on('draw', function (e, dt, type, indexes) {
            if (table.mcms.dataJustUpdated) {
                table.mcms.dataJustUpdated = false;
                table.updateSelectAllCheckbox(table, tableJQuery);
            }
        });
    },
    enableBatchActions: function (table, tableJQuery, config) {
        if (!config.batchActions || !config.batchActions.length) {
            return;
        }
        mcmsDatatables.enableCheckboxSelection(table, tableJQuery);

        for (var i = 0; i < config.batchActions.length; i++) {
            var ba = config.batchActions[i];
            ba.action = function (e, dt, node, config) {
                if (!config.data || config.data.toggle !== 'ajax-modal') {
                    console.error(config);
                    return;
                }
                var cnt = dt.rows({selected: true}).count();
                var sdt = dt.rows({selected: true}).data();
                var ids = [];
                for (var i = 0; i < sdt.length; i++) {
                    ids.push(encodeURIComponent(sdt[i].id));
                }
                var pn = config.data['url-arg-name'];
                var url = config.data['url'] + '?' + pn + '=' + ids.join('&' + pn + '=');
                var vEl = $("<div></div>");
                var dataCloned = deepmerge({}, config.data);
                dataCloned.url = url;
                dataCloned['modal-callback'] = table.mcms.callbacks.modalClosed;
                dataCloned['modal-callback-target'] = ids;
                vEl.data(dataCloned);
                mModals.ajaxModalItemAction.apply(vEl[0]);
            }
        }
        table.one('init', function (e, settings, data) {
            var bab = new $.fn.dataTable.Buttons(table, {
                buttons: config.batchActions
            });
            var babContainer = bab.dom.container;
            babContainer.appendTo(tableJQuery.closest(".dataTables_wrapper").find(".batch-actions-container"));
        });
    },
    enableTableActions: function (table, tableJQuery, config) {
        for (var i = 0; i < config.tableActions.length; i++) {
            var ta = config.tableActions[i];
            if (typeof ta === 'string') {
                config.tableActions[i] = ta = {extend: ta};
            }
            ta.className = "btn-light btn-outline-secondary";
        }

        table.one('init', function (e, settings, data) {
            var tab = new $.fn.dataTable.Buttons(table, {
                buttons: config.tableActions
            });
            var babContainer = tab.dom.container;
            babContainer.appendTo(tableJQuery.closest(".dataTables_wrapper").find(".table-actions-container"));
        });
    },
    bindDefaultModalEventHandlers: function (table) {
        table.on("modalClosed.mcms", function (e, sender, params) {
            if (!params || params.failed) return;
            if (params.reload) {
                table.ajax.reload();
                return;
            }
            var senderData = sender.data();
            switch (senderData.tag) {
                case 'create':
                    var model = params && params.params && (params.params.secondaryModel || params.params.model);
                    if (model && typeof model === 'object') {
                        model.id = params.params.id;
                        table.mcms.customMethods.initialPatchRowData(model);
                        table.row.add(model).draw(false);
                    }
                    break;
                case 'edit':
                    var model = params && params.params && (params.params.secondaryModel || params.params.model);
                    if (model && typeof model === 'object') {
                        var index = table.mcms.getDataIndexById(table.data(), senderData.modalCallbackTarget);
                        if (index >= 0) {
                            table.mcms.customMethods.initialPatchRowData(model);
                            table.row(index).data(model).draw();
                        }
                    }
                    break;
                case 'delete':
                    table.mcms.removeRowWithDataId(table, table.data(), senderData.modalCallbackTarget);
                    table.mcms.dataJustUpdated = true;
                    table.draw();
                    break;
                case 'batch-delete':
                    if (!senderData.modalCallbackTarget) {
                        return;
                    }
                    for (var i = 0; i < senderData.modalCallbackTarget.length; i++) {
                        table.mcms.removeRowWithDataId(table, table.data(), senderData.modalCallbackTarget[i]);
                    }
                    table.mcms.dataJustUpdated = true;
                    table.draw();
                    break;
                default:
                    console.log(senderData.tag);
                    console.log(params);
                    break;
            }
        });

        table.mcms.getDataIndexById = function (data, id) {
            if (!id) return -1;
            for (var i = 0; i < data.length; i++) {
                if (data[i] && data[i].id === id) {
                    return i;
                }
            }
            return -1;
        };
        table.mcms.removeRowWithDataId = function (table, data, id) {
            if (!id) return;
            var index = table.mcms.getDataIndexById(data, id);
            if (index >= 0) {
                table.rows(index).remove();
            }
        };
    },
}
var mcmsTables = [];
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
jQuery.fn.dataTable.Api.register('updateSelectAllCheckbox', function (table, tableJq) {
    window.tableJq = tableJq;
    var selectionLength = this.rows({selected: true}).count();
    var allSelected = selectionLength > 0 && selectionLength === this.rows().count();
    if (!this.mcms.selectAllThi) {
        this.mcms.selectAllThi = table.header().toJQuery().add(table.footer().toJQuery()).find("th.select-all-checkbox i");
    }
    this.mcms.selectAllThi.toggleClass("fa-square", !allSelected);
    this.mcms.selectAllThi.toggleClass("fa-check-square", allSelected);
});
jQuery.fn.dataTable.ext.buttons.mcmsColVis = {
    extend: 'collection',
    text: '<i class="fas fa-grip-lines-vertical fa-fw"></i> <i class="fas fa-eye-slash fa-fw"></i> ',
    buttons: [
        {
            extend: 'columnsToggle',
            columns: ':not(.non-toggleable)'
        },
        {
            extend: 'columnToggle',
            text: 'Show all',
            visibility: true,
            className: 'mt-3 forever-inactive',
            columns: ':not(.non-toggleable)'
        },
        {
            extend: 'columnToggle',
            text: 'Hide all',
            className: 'forever-inactive',
            visibility: false,
            columns: ':not(.non-toggleable)'
        },
        {
            extend: 'colvisRestore',
            text: 'Restore'
        }
    ]
};