var mcmsDatatables = {
    bindDefaultDataTables: function (selector, initialConfig, actionsColumnContent, lang) {
        var tableElem = $(selector);

        initialConfig.columns = initialConfig.columns.slice();
        var shouldMakeActionsCellContent = actionsColumnContent && actionsColumnContent.trim().length > 0;

        var initialPatchRowData = function (rowData) {
            if (shouldMakeActionsCellContent) {
                rowData._actions = actionsColumnContent.replace(/ENTITY_ID/g, rowData.id);
            }
        }

        var config = {
            processing: true,
            ajax: {
                dataSrc: function (json) {
                    if (shouldMakeActionsCellContent) {
                        for (var i = 0; i < json.length; i++) {
                            initialPatchRowData(json[i]);
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

        mcmsDatatables.sumTotalRowIfNeeded(config);

        var tableJQuery = tableElem.dataTable(config);
        var table = tableJQuery.api();


        table.mcms = {customMethods: {initialPatchRowData: initialPatchRowData}, $: tableJQuery, callbacks: {}};

        if (!config.skipDefaultModalEventHandlers) {
            mcmsDatatables.bindDefaultModalEventHandlers(table);
        }
        if (config.enableColumnSearch) {
            mcmsDatatables.enableColumnSearchRow(config, tableJQuery);
        }

        if (config.hasStaticIndexColumn) {
            var staticIndexColumnIndex = config.checkboxSelection ? 1 : 0;
            table.on('order.dt search.dt', function () {
                table.column(staticIndexColumnIndex, {
                    search: 'applied',
                    order: 'applied'
                }).nodes().each(function (cell, i) {
                    cell.innerHTML = i + 1;
                });
            }).draw();
        }

        if (config.checkboxSelection) {
            mcmsDatatables.enableBatchActions(table, tableJQuery, config);
        }
        if (config.tableActions && config.tableActions.length) {
            mcmsDatatables.enableTableActions(table, tableJQuery, config);
        }

        table.on('processing.dt', function (e, settings, processing) {
            var overlay = $(e.currentTarget).closest('.dataTables_wrapper').find('.processing-overlay');
            if (processing) {
                overlay.show();
            } else {
                overlay.hide();
            }
        });

        mcmsDatatables.properlyDestroyInModal(tableElem, table);

        document.addEventListener('side-menu-toggled', function () {
            table.fixedHeader.adjust();
            setTimeout(function () {
                table.fixedHeader.adjust();
            }, 500);
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
    sumTotalRowIfNeeded: function (config) {
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
            var row = this.find("tfoot tr.sum-total-row");
            if (!row.data("build")) {
                row.removeClass("d-none");
                row.toggle().data("build", true);
                var cols = row.find('th')
                cols.each(function (index) {
                    $(this).html('');
                });
                cols.eq(config.checkboxSelection ? 1 : 0).html("Total");
            }
            var api = this.api();
            api.columns().every(function (index) {
                var sumTotalOpt = config.columns[index].sumTotal;
                if (!sumTotalOpt)
                    return;

                var col = api.column(index);
                if (!col.visible())
                    return;

                var htmlElementIndex = $(col.footer()).index();
                if (htmlElementIndex === -1)
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

                row.children().eq(htmlElementIndex).html(sum + "");
            });
        }
    },
    toggleColumnSearchRow: function (table, columnsConfig) {
        var searchRow = table.find('tfoot tr.column-search-row');
        searchRow.toggle();
        if (!searchRow.data('build')) {
            searchRow.data('build', true)
            searchRow.removeClass("d-none");

            var searchCells = searchRow.find('td');
            searchCells.each(function (index) {
                if (!columnsConfig[index].searchable) {
                    $(this).html('');
                    return;
                }
                var title = $(this).text();
                $(this).html('&nbsp;<input type="text" placeholder="🔍 ' + title + '" />');
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
    },
    enableColumnSearchRow: function (config, tableJq) {
        config.buttons.splice(0, 0,
            {
                text: '<i class="fas fa-grip-lines-vertical fa-fw"></i>🔍',
                className: 'btn-light btn-outline-info',
                action: function (e, dt, node, conf) {
                    mcmsDatatables.toggleColumnSearchRow(tableJq, config.columns);
                }
            });
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
                    console.log(senderData.modalCallbackTag);
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
    }
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