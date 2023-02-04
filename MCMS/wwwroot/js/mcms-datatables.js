const mcmsTables = [];

(function ($) {
    mcmsTables.reload = function () {
        for (let i = 0; i < this.length; i++) {
            this[i].ajax.reload();
        }
    }
    window.mcmsDatatables = {
        bindDefaultDataTables: function (selector, initialConfig, actionsColumnContent, lang) {
            const tableId = selector.replace("#", "");
            const tableElem = $(selector);

            initialConfig.columns = initialConfig.columns.slice();

            const shouldMakeActionsCellContent = actionsColumnContent && actionsColumnContent.trim().length > 0;
            const initialPatchRowData = function (rowData) {
                if (shouldMakeActionsCellContent) {
                    rowData._actions = actionsColumnContent.replace(/ENTITY_ID/g, rowData.id);
                    if(initialConfig.itemActionsPlaceholders) {
                        for (let placeholder of Object.keys(initialConfig.itemActionsPlaceholders)) {
                            rowData._actions = rowData._actions.replace(placeholder, rowData[initialConfig.itemActionsPlaceholders[placeholder]]);
                        }
                    }
                }
            };

            let config = {
                stateSave: true,
                processing: true,
                ajax: {
                    dataSrc: function (json) {
                        return mcmsDatatables.formatDataFromApi(json, initialConfig, initialPatchRowData, shouldMakeActionsCellContent);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (errorThrown === 'abort') {
                            return;
                        }
                        let msg = (jqXHR.responseJSON ? jqXHR.responseJSON.error : jqXHR.responseText)
                            || "An error occurred while trying to access data. Please try again.";
                        if (msg.indexOf('modal-body')) {
                            msg = $(msg).find('.modal-body');
                        }
                        mcmsModals.alertModalText(msg, "Error");
                        tableJQuery._fnProcessingDisplay(false);
                    },
                    beforeSend: function (request) {
                        request.setRequestHeader("X-Request-Modal", true);
                    }
                },
                bAutoWidth: false,
                // iDisplayLength: 50, // this is set through TableConfig (initialConfig) object
                lengthMenu: [[10, 25, 50, 100, 250, 500, 1000, -1], [10, 25, 50, 100, 250, 500, 1000, "All"]],
                fixedHeader: {headerOffset: 50},
                language: mcmsDatatables.getLang(lang),
                dom: "<'processing-backdrop'><'row'<'col-sm-12 col-md-6 table-actions-container'><'col-sm-12 col-md-6'f>>" +
                    "<'row'<'col-sm-12 table-horizontal-scroll 'tr>>" +
                    "<'row'<'col-12 batch-actions-container'>>" +
                    "<'row footer-table-row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 d-flex justify-content-between 'pB>>",
                buttons: [
                    {
                        text: '<i class="fas fa-sync-alt"></i>',
                        className: 'btn-secondary',
                        action: function (e, dt) {
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

            const tableJQuery = tableElem.dataTable(config);
            const table = tableJQuery.api();

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
                const staticIndexColumnIndex = config.checkboxSelection ? 1 : 0;
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
                tableJQuery.closest('.dataTables_wrapper').find('.processing-backdrop').toggle(processing);
            });

            mcmsDatatables.properlyDestroyInModal(tableElem, table);

            mcmsDatatables.fixGlobalFilterDebounce(table, tableJQuery, config);

            mcmsDatatables.bindAutoAdjustFixedHeader(table, tableJQuery);

            mcmsTables.push(table);
            table.on('destroy', function () {
                mcmsTables.splice(mcmsTables.indexOf(table), 1);
            });
            return table;
        },
        getLang: function (lang) {
            const langConfigBasePath = typeof basePath !== 'undefined' ? basePath : '';
            const langConfig = {
                'en': {"url": langConfigBasePath + "/_content/MCMS/lib/datatables/English.json"},
                'ro': {"url": langConfigBasePath + "/_content/MCMS/lib/datatables/Romanian.json"}
            };
            return langConfig[lang];
        },
        sumTotalRowIfNeeded: function (config, tableApi, tableJQuery) {
            const sumTotalCols = config.columns.map(function (c, i) {
                return {idx: i, sumTotal: c.sumTotal};
            }).filter(function (c) {
                return c.sumTotal;
            });
            if (!sumTotalCols.length) {
                return;
            }
            tableApi.on('init', function () {
                const sumTotalRow = tableApi.footer().toJQuery().find('tr.sum-total-row');
                if (!sumTotalRow.length) {
                    return;
                }

                try {
                    const sumTotalFooterRowIndex = tableApi.footer().toJQuery().find('tr.sum-total-row').index();
                    const sumTotalFooterObjects = tableApi.settings()[0].aoFooter[sumTotalFooterRowIndex];
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
                const sumTotalFooterRowIndex = tableApi.footer().toJQuery().find('tr.sum-total-row').index();
                const sumTotalFooterObjects = tableApi.settings()[0].aoFooter[sumTotalFooterRowIndex];
                for (let i = 0; i < sumTotalCols.length; i++) {
                    const colConfig = sumTotalCols[i];
                    const col = tableApi.column(colConfig.idx);
                    if (!col.visible())
                        return;
                    const sumTotalOpt = colConfig.sumTotal;
                    let sum = '';
                    const visibleColData = tableApi.column(colConfig.idx, {page: 'current'}).data();
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
            const searchRow = tableApi.footer().toJQuery().find('tr.column-search-row');
            searchRow.toggle();
            if (!searchRow.data('build')) {
                searchRow.data('build', true)
                const searchFooterRowIndex = searchRow.index();

                const searchFooterObjects = tableApi.settings()[0].aoFooter[searchFooterRowIndex];
                for (let i = 0; i < searchFooterObjects.length; i++) {
                    const cell = $(searchFooterObjects[i].cell);
                    const colConfig = columnsConfig[i];
                    if (!colConfig.searchable) {
                        cell.html('');
                        continue;
                    }
                    const title = cell.text();
                    const input = mcmsDatatables.getInputElementForColumnSearch(colConfig, title, tableConfig.serverSide);
                    const col = tableApi.column(i);
                    const currentSearch = col.search();
                    if (currentSearch) {
                        input.val(currentSearch);
                    }
                    input.data('colApi', col);
                    input.on('keyup change clear', $.debounce(tableConfig.serverSide ? tableConfig.searchDelay : 100, function () {
                        const col = $(this).data('colApi');
                        const val = this.value;
                        if (col.search() !== val) {
                            col.search(val).draw();
                        }
                    }));
                    cell.html('&nbsp;').append(input);
                }
            }
        },
        getInputElementForColumnSearch: function (colConfig, placeholder, serverSide) {
            if (serverSide) {
                switch (colConfig.mType) {
                    case 'bool':
                    case 'nBool':
                    case 'select':
                        const elem = $('<select>');
                        if (colConfig.mFilterValues) {
                            for (let i = 0; i < colConfig.mFilterValues.length; i++) {
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
                    className: 'btn-secondary',
                    action: function () {
                        mcmsDatatables.toggleColumnSearchRow(tableApi, config.columns, config);
                    }
                });
            const search = tableApi.columns().search();
            //if there is any search in at least one column, then toggle (show) the row right now
            for (let i = 0; i < search.length; i++) {
                if (search[i]) {
                    mcmsDatatables.toggleColumnSearchRow(tableApi, config.columns, config);
                    break;
                }
            }
        },
        properlyDestroyInModal: function (tableElem, datatable) {
            const modal = tableElem.closest(".modal");
            if (!modal.length) {
                return;
            }
            modal.on('hide.bs.modal', function () {
                datatable.destroy();
                datatable.mcms.$.attr("id", datatable.mcms.$.attr("id") + "-old");
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
            table.on('select deselect', function () {
                table.updateSelectAllCheckbox(table, tableJQuery);
            });
            table.on('xhr', function () {
                table.mcms.dataJustUpdated = true;
            });
            table.on('draw', function () {
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

            for (let i = 0; i < config.batchActions.length; i++) {
                const ba = config.batchActions[i];
                ba.action = function (e, dt, node, config) {
                    if (!config.data || config.data.toggle !== 'ajax-modal') {
                        console.error(config);
                        return;
                    }
                    const sdt = dt.rows({selected: true}).data();
                    const ids = [];
                    for (let i = 0; i < sdt.length; i++) {
                        ids.push(encodeURIComponent(sdt[i].id));
                    }
                    const pn = config.data['url-arg-name'];
                    const url = config.data['url'] + '?' + pn + '=' + ids.join('&' + pn + '=');
                    const vEl = $("<div></div>");
                    const dataCloned = deepmerge({}, config.data);
                    dataCloned.url = url;
                    dataCloned['modal-callback'] = table.mcms.callbacks.modalClosed;
                    dataCloned['modal-callback-target'] = ids;
                    vEl.data(dataCloned);
                    mModals.ajaxModalItemAction.apply(vEl[0]);
                }
            }
            table.one('preInit', function () {
                const bab = new $.fn.dataTable.Buttons(table, {
                    buttons: config.batchActions
                });
                const babContainer = bab.dom.container;
                table.mcms.batchActionsContainer = table.mcms.$.closest(".dataTables_wrapper").find(".batch-actions-container");
                table.mcms.batchActionsContainer.hide();
                babContainer.appendTo(table.mcms.batchActionsContainer);
            });
        },
        enableTableActions: function (table, tableJQuery, config) {
            for (let i = 0; i < config.tableActions.length; i++) {
                let ta = config.tableActions[i];
                if (typeof ta === 'string') {
                    config.tableActions[i] = ta = {extend: ta};
                }
                ta.className = "btn-secondary";
            }

            table.one('preInit', function (e, settings, data) {
                const tab = new $.fn.dataTable.Buttons(table, {
                    buttons: config.tableActions
                });
                const babContainer = tab.dom.container;
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
                const senderData = sender.data();
                let model;
                switch (senderData.tag) {
                    case 'create':
                        model = params && params.params && (params.params.secondaryModel || params.params.model);
                        if (model && typeof model === 'object') {
                            model.id = params.params.id;
                            table.mcms.customMethods.initialPatchRowData(model);
                            table.row.add(model).draw(false);
                        }
                        break;
                    case 'edit':
                        model = params && params.params && (params.params.secondaryModel || params.params.model);
                        if (model && typeof model === 'object') {
                            const index = table.mcms.getDataIndexById(table.data(), senderData.modalCallbackTarget);
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
                        for (let i = 0; i < senderData.modalCallbackTarget.length; i++) {
                            table.mcms.removeRowWithDataId(table, table.data(), senderData.modalCallbackTarget[i]);
                        }
                        table.mcms.dataJustUpdated = true;
                        table.draw();
                        break;
                    case 'none':
                        break;
                    default:
                        if (params?.params?.reloadTable) {
                            table.draw();
                            break;
                        }
                        // console.log(senderData.tag);
                        // console.log(params);
                        break;
                }
            });

            table.mcms.getDataIndexById = function (data, id) {
                if (!id) return -1;
                for (let i = 0; i < data.length; i++) {
                    if (data[i] && data[i].id === id) {
                        return i;
                    }
                }
                return -1;
            };
            table.mcms.removeRowWithDataId = function (table, data, id) {
                if (!id) return;
                const index = table.mcms.getDataIndexById(data, id);
                if (index >= 0) {
                    table.rows(index).remove();
                }
            };
        },
        formatDataFromApi: function (json, initialConfig, initialPatchRowData, shouldMakeActionsCellContent) {
            let i;
            const data = json.data ? json.data : json;

            new ReferencesHelperService().populateReferences(data);

            if (shouldMakeActionsCellContent) {
                for (i = 0; i < data.length; i++) {
                    initialPatchRowData(data[i]);
                }
            }
            if (initialConfig.serverSide) {
                for (i = 0; i < data.length; i++) {
                    data[i]._index = i + 1;
                }
            }

            for (i = 0; i < initialConfig.columns.length; i++) {
                const col = initialConfig.columns[i];
                if (col.mType === "bool" || col.mType === "nBool") {
                    for (let j = 0; j < data.length; j++) {
                        if (data[j][col.data] === true) {
                            data[j][col.data] = '<div class="bool-wrapper"><i class="far fa-check-circle fa-lg text-success"></i></div>';
                        } else if (data[j][col.data] === false) {
                            data[j][col.data] = '<div class="bool-wrapper"><i class="far fa-times-circle fa-lg text-danger st-text"></i></div>';
                        } else if (col.mType === "nBool" && data[j][col.data] === undefined || data[j][col.data] === null) {
                            data[j][col.data] = '<div class="bool-wrapper"><i class="far fa-question-circle fa-lg text-secondary"></i></div>';
                        }
                    }
                }
            }
            return data;
        },
        fixGlobalFilterDebounce: function (table, tableJq, config) {
            table.on('preInit', function () {
                const filterInput = tableJq.closest(".dataTables_wrapper").find(".dataTables_filter input")
                    .unbind();

                filterInput.bind('input', $.debounce(config.serverSide ? config.searchDelay : 100,
                    function () {
                        table.search(filterInput.val()).draw();
                    }));

                filterInput.attr('placeholder', table.i18n('sSearch'));
                $(filterInput[0].previousSibling).remove();
            });
        },
        bindAutoAdjustFixedHeader: function (table, tableJq) {
            document.addEventListener('side-menu-toggled', function () {
                table.fixedHeader.adjust();
                setTimeout(function () {
                    table.fixedHeader.adjust();
                }, 500);
            });
            table.one('init', function (e, settings, data) {
                const parent = tableJq.parent();
                if (parent.hasClass("table-horizontal-scroll")) {
                    parent.on("scroll", $.throttle(500, function () {
                        table.fixedHeader.adjust();
                    }));
                }
            });
        }
    };
})(jQuery);


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
jQuery.fn.dataTable.Api.register('updateSelectAllCheckbox', function (table) {
    const selectionLength = this.rows({selected: true}).count();
    const allSelected = selectionLength > 0 && selectionLength === this.rows().count();
    if (!this.mcms.selectAllThi) {
        this.mcms.selectAllThi = table.header().toJQuery().add(table.footer().toJQuery()).find("th.select-all-checkbox i");
    }
    this.mcms.selectAllThi.toggleClass("fa-square", !allSelected);
    this.mcms.selectAllThi.toggleClass("fa-check-square", allSelected);
    if (!!selectionLength !== this.mcms.batchActionsContainer.is(":visible")) {
        this.mcms.batchActionsContainer.slideToggle();
    }
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
        },
        'resetSorting'
    ]
};

jQuery.fn.dataTable.ext.buttons.resetSorting = {
    text: '<i class="fas fa-redo fa-fw"></i> <i class="fas fa-sort-alpha-down fa-fw"></i>',
    action: function (e, dt) {
        dt.order([]).draw();
    }
};

class ReferencesHelperService {
    populateReferences(obj) {
        const cache = {};
        if (!obj || typeof obj !== 'object' || obj.$populated) {
            return;
        }
        try {
            this.recFunc(obj, cache);
            obj.$populated = true;
        } catch (exc) {
            console.log("can't populate?");
            console.error(exc);
        }
    }

    recFunc(obj, cache) {
        if (!obj || typeof obj !== 'object') {
            return;
        }
        if (obj.hasOwnProperty('$id') && !cache.hasOwnProperty(obj.$id)) {
            const id = obj.$id;
            delete obj.$id;
            cache[id] = obj;
            this.iteratePropsOrElements(obj, cache);
        } else if (obj instanceof Array) {
            this.iteratePropsOrElements(obj, cache);
        }
    }

    iteratePropsOrElements(obj, cache) {
        const keys = Object.keys(obj);
        for (const key of keys) {
            const child = obj[key];
            if (typeof child !== 'object' || !child) {
                continue;
            }
            if (child.hasOwnProperty('$ref')) {
                const id = child.$ref;
                if (!cache.hasOwnProperty(id)) {
                    console.log('id not in cache: ' + id);
                    continue;
                }
                obj[key] = cache[id];
            } else {
                this.recFunc(child, cache);
            }
        }
    }
}
