var datatable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/ApprovedOrder/GetAll"
        },
        "columns": [
            { "data": "id", "Width": "5%" },
            { "data": "name", "Width": "10%" },
            { "data": "phoneNumber", "Width": "10%" },
            { "data": "orderDate", "Width": "10%" },
            { "data": "paymentStatus", "Width": "15%" },
            { "data": "orderStatus", "Width": "15%" },
            { "data": "orderTotal", "Width": "10%" },
            {
                "data": "id", "width": "15%",
                "render": function (data) {
                    return `
                    <div class="text-center">
            <a class="btn btn-success" href="/Admin/Approved/ViewDetail/${data}">ViewDetails</a>
                </div>            
                 `;
                }
            }
        ]
    })
}