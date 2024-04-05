var datatable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Orders/GetAll"
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
            <a class="btn btn-success" href="/Admin/Orders/ViewDetail/${data}">ViewDetails</a>
                </div>
            <br/>
            <div class="text-center">
                    <a href="/Admin/Orders/CancelOrder/${data}" class="btn btn-danger">CancelOrder</a>
                    </div>
                 `;
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                     <a class="btn btn-danger" onClick=Delete("/Admin/Orders/DeleteOrder/${data}")>
                    <i class="fas fa-trash-alt"></i>
                    </a>
                </div>
                `;
                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Want to delete Order?",
        text: "Delete Order !",
        icon: "error",
        buttons: true,
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}