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
        "ajax": "/customer/order/getall?status=" + status,
        "columns": [
            { data: "id", "width": "5%" },
            { data: "name", "width": "30%" },
            { data: "orderStatus", "width": "20%" },
            { data: "orderTotal", "width": "20%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/customer/order/details?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`;
                },
                "width": "25%"
            }
        ]
    });
}