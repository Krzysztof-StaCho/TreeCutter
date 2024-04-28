var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var index = url.lastIndexOf('=');
    if (index == -1) {
        loadDataTable("all");
    } else {
        var status = url.substring(index + 1);
        loadDataTable(status);
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": "/admin/order/getall?status=" + status,
        "columns": [
            { data: "id", "width": "5%" },
            { data: "name", "width": "25%" },
            { data: "phoneNumber", "width": "20%" },
            { data: "applicationUser.email", "width": "20%" },
            { data: "orderStatus", "width": "10%" },
            { data: "orderTotal", "width": "10%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`;
                },
                "width": "10%"
            }
        ]
    });
}