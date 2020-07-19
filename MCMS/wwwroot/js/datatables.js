function bindDefaultDataTables(tableElem, url, columns, actionsColumnContent, hasStaticIndexColumn) {
    // {"data": "index"},
    columns = columns.slice();

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
        language: {
            "url": "/lib/datatables/Romanian.json"
        },
        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'<f>>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 d-flex justify-content-between'pB>>",
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
    if (hasStaticIndexColumn) {
        columns.splice(0, 0, {"data": "dataTablesIndex"});
        config.columnDefs = [{
            searchable: false,
            orderable: false,
            targets: 0
        }];
        config.aaSorting = [];
    }
    console.log(config.buttons);
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