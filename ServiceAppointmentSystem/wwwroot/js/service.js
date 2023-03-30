var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": {
            "url": "/Admin/Service/GetAll"
        },
        "columns": [
            { "data": "name", "width": "30%" },
            { "data": "role", "width": "30%" },
            { "data": "basePrice", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Service/Upsert/${data}" class="btn btn-info text-white" style="cursor: pointer">
                                <i class="fa-solid fa-pen-to-square"></i>
                            </a>
                            <a href="/Admin/Service/Delete/${data}" class="btn btn-danger text-white" style="cursor: pointer">
                                <i class="fa-solid fa-trash"></i>
                            </a>
                        </div>
                    `;
                }, "width": "20%"
            }
        ]
    });
}