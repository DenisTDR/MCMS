function bindDefaultDataTables(tableElem, url, columns, actionsColumnContent, hasStaticIndexColumn, lang) {
    // {"data": "index"},
    columns = columns.slice();

    var langConfig = {'ro': {"url": "/lib/datatables/Romanian.json"}};

    var config = {
        processing: true,
        ajax: {
            url: url,
            dataSrc: function (json) {
                for (var i = 0; i < json.length; i++) {
                    if (hasStaticIndexColumn) {
                        json[i].dataTablesIndex = '#';
                    }
                    json[i]._actions = actionsColumnContent.replace(/ENTITY_ID/g, json[i].id);
                }
                return json;
            }
        },
        columns: columns,
        bAutoWidth: false,
        iDisplayLength: 50,
        lengthMenu: [[10, 25, 50, 100, 250, 500, -1], [10, 25, 50, 100, 250, 500, "All"]],
        fixedHeader: {
            headerOffset: 50
        },
        language: langConfig[lang],
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'<f>>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 d-flex justify-content-between'pB>>",
        buttons: [
            {
                text: '<i class="fas fa-grip-lines-vertical fa-fw"></i><i class="fas fa-search fa-fw"></i>',
                className: 'btn-light btn-outline-info',
                action: function (e, dt, node, config) {
                    toggleDataTablesColumnSearch(tableElem, columns);
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
    if (hasStaticIndexColumn) {
        columns.splice(0, 0, {"data": "dataTablesIndex"});
        config.columnDefs = [{
            searchable: false,
            orderable: false,
            targets: 0
        }];
        config.aaSorting = [];
    }
    var table = tableElem.DataTable(config);
    if (hasStaticIndexColumn) {
        table.on('order.dt search.dt', function () {
            table.column(0, {search: 'applied', order: 'applied'}).nodes().each(function (cell, i) {
                cell.innerHTML = i + 1;
            });
        }).draw();
    }
    return table;
}

function toggleDataTablesColumnSearch(table, columns) {
    var searchRow = table.find('tfoot tr.column-search-row');
    searchRow.toggle();
    if (!searchRow.data('build')) {
        searchRow.data('build', true)

        var searchCells = searchRow.find('th');
        searchCells.each(function (index) {
            if (!columns[index].searchable) {
                $(this).html('');
                return;
            }
            var title = $(this).text();
            $(this).html('&nbsp;<input type="text" placeholder="Search ' + title + '" />');
        });

        table.dataTable().api().columns().every(function (index) {
            var that = this;
            $(searchCells[index]).find("input").on('keyup change clear', function () {
                if (that.search() !== this.value) {
                    that.search(this.value).draw();
                }
            });
        });
    }
}