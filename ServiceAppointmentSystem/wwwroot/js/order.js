var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess")
    } else if (url.includes("pending")) {
        loadDataTable("pending")
    } else if (url.includes("completed")) {
        loadDataTable("completed")
    } else {
        loadDataTable("all")
    }
});

function loadDataTable(status) {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status=" + status
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "appUser.email", "width": "20%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "orderTotal", "width": "10%",
                "render": function (data) {
                    var number = $.fn.dataTable.render
                        .number(',', '.', 2, 'Rs')
                        .display(data);

                    return '<span>' + number + '</span>';
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Order/Detail?ID=${data}" class="btn btn-info text-white">
                                <i class="fa-solid fa-circle-info"></i>
                            </a>
                        </div>
                    `;
                }, "width": "10%"
            }
        ]
    });
}