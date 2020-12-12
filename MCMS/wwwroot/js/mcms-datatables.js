var mcmsDatatables = {
    bindDefaultDataTables: function (selector, initialConfig, actionsColumnContent, lang) {
        var tableElem = $(selector);

        initialConfig.columns = initialConfig.columns.slice();
        var shouldMakeActionsCellContent = actionsColumnContent && actionsColumnContent.trim().length > 0;

        /*
        var datas = table4cae39a53c3449059084.rows( { selected: true } ).data();
        var ids = [];
        for(var i = 0; i < datas.length; i++) {
            ids.push(datas[i].id);
        }
        console.log(ids);
        
         */

        var initialPatchRowData = function (rowData) {
            if (shouldMakeActionsCellContent) {
                rowData._actions = actionsColumnContent.replace(/ENTITY_ID/g, rowData.id);
            }
        }

        var config = {
            select: {
                style: 'multi+shift',
                // style: 'os',
                className: 'row-selected',
                selector: 'td:first-child'
            },
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
            fixedHeader: {
                headerOffset: 50
            },
            language: mcmsDatatables.getLang(lang),
            dom: "<'processing-overlay'><'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'<f>>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-12 batch-actions-container'>>" +
                "<'row footer-table-row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 d-flex justify-content-between'pB>>",
            buttons: [
                {
                    text: '<i class="fas fa-grip-lines-vertical fa-fw"></i><i class="fas fa-search fa-fw"></i>',
                    className: 'btn-light btn-outline-info',
                    action: function (e, dt, node, conf) {
                        mcmsDatatables.toggleDataTablesColumnSearch(tableElem, config.columns);
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

        config = deepmerge(initialConfig, config);

        if (config.hasStaticIndexColumn) {
            config.aaSorting = [];
        }

        mcmsDatatables.sumTotalRowIfNeeded(config);

        var tableJQuery = tableElem.dataTable(config);
        var table = tableJQuery.api();


        table.mcms = {customMethods: {}};

        table.mcms.customMethods.initialPatchRowData = initialPatchRowData;

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
            mcmsDatatables.enableCheckboxSelection(table, tableJQuery);
            mcmsDatatables.enableBatchActionButtons(table);
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
            var row = this.find(".sum-total-row");
            if (!row.data("build")) {
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
    toggleDataTablesColumnSearch: function (table, columnsConfig) {
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
            table.mcms.justFiredXhr = true;
        });
        table.on('draw', function (e, dt, type, indexes) {
            if (table.mcms.justFiredXhr) {
                table.mcms.justFiredXhr = false;
                table.updateSelectAllCheckbox(table, tableJQuery);
            }
        });
    },
    enableBatchActionButtons: function (table) {
        table.mcms.batchActionButtons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    text: '<i class="fas fa-trash"></i>',
                    className: 'btn-light btn-outline-danger',
                    name: 'delete',
                    action: function (e, dt, node, config) {
                        dt.ajax.reload();
                    }
                },
                {
                    text: 'xyyyy',
                    className: 'btn-light btn-outline-warning',
                    name: 'blabla',
                    action: function (e, dt, node, config) {
                        dt.ajax.reload();
                    }
                }
            ]
        });
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

    if (allSelected !== this.mcms.batchActionButtons.allSelected) {
        this.mcms.batchActionButtons.allSelected = allSelected;
        var checkTh = table.header().toJQuery().add(table.footer().toJQuery()).find("th.select-all-checkbox i");
        checkTh.toggleClass("fa-square", !allSelected);
        checkTh.toggleClass("fa-check-square", allSelected);
    }

    var anySelection = selectionLength > 0;
    if (anySelection !== this.mcms.batchActionButtons.anySelection) {
        this.mcms.batchActionButtons.anySelection = anySelection;
        var babContainer = this.mcms.batchActionButtons.dom.container;
        if (!babContainer.data('attached')) {
            babContainer.appendTo(tableJq.closest(".dataTables_wrapper").find(".batch-actions-container"));
            babContainer.data('attached', true);
        }
        if (anySelection) {
            babContainer.show();
        } else {
            babContainer.hide();
        }
    }
});