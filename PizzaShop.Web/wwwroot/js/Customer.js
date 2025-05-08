var customerAjax;
var ToDate;
var FromDate;
var SortBy = '';
var SortType = '';

function GetFilteredCustomers() {
    var SearchString = $("#searchCustomer").val();
    var pageSize = $("#customerPageSize").val();
    var status = $("#statusDropDown").val();
    var time = $("#timeDropDown").val();

    if (customerAjax && customerAjax.readyState !== 4)
        customerAjax.abort();

    customerAjax = $.ajax({
        url: "/customer/GetFilteredCustomers",
        type: "GET",
        data: {
            pageIndex: CustomerPageIndex,
            pageSize: pageSize,
            searchString: SearchString,
            fromDate: FromDate,
            toDate: ToDate,
            status: status,
            time: time,
            sortBy: SortBy,
            sortType: SortType
        },
        success: function (data) {
            $("#OrderListContainer").html(data);
        }
    })
}

function customerPageSize(PageSize){
    var pageSize = PageSize
        var pageIndex = 1
        GetFilteredCustomers();
}

$(document).on('click', '#PreviousCustomers', function () {
    CustomerPageIndex -= 1;
    GetFilteredCustomers()
})

$(document).on('click', '#NextCustomers', function () {
    CustomerPageIndex += 1;
    GetFilteredCustomers()
})

$(document).on('click', '.customerRow', function () {
    var id = $(this).data('id');


    $.ajax({
        url: "/customer/GetCustomerHistory",
        type: "GET",
        data: { id: id },
        success: function (data) {
            $("#customerHistoryContainer").html(data);
            $("#customerHistoryModal").modal('show');
        },
        error: function () {
            toastr.error("Error while fetching the customer history");
        }
    })


})

$(document).on('change', '#FromDate', function () {
    FD = $(this).val();
    document.getElementById("ToDate").setAttribute("min", FD);
})

$(document).on('change', '#ToDate', function () {
    TD = $(this).val();
    document.getElementById("FromDate").setAttribute("max", TD);
})

$(document).on('change', '#timeDropDown', function () {
    var time = $(this).val(); 
    if(time == "Select Date Range")
    {
        $("#DateRangeForm").val('')
        $("#DateRangeModal").modal('show');
    }else{
        FromDate ='';
        ToDate ='';
        GetFilteredCustomers();
    }
})

$(document).on('click', '.closeDateRangeModal', function () {
    $("#DateRangeModal").modal('hide');
    $(".text-danger").text('')
})

function searchCustomer(PageSize){
    var filter = $("#timeDropDown").val();
    if(filter == "")
    {
        FromDate ='';
        ToDate ='';
    }
    clearTimeout(debounceTimer);
    CustomerPageIndex = 1
    pageSize = PageSize
        debounceTimer = setTimeout(() => {
            GetFilteredCustomers();
        }, 400)
}

$(document).on("click",".sorting",function()
{
    SortType = $(this).data('type');
    SortBy = $(this).data('name');
    GetFilteredCustomers();
})

$("#DateRangeForm").validate({
    rules: {
        ToDate: {
            required: true
        },
        FromDate: {
            required: true
        }
    },

    messages: {
        ToDate: {
            required: "ToDate is required"
        },
        FromDate: {
            required: "FromDate is required"
        }
    },
    errorPlacement: function (error, elment) {
        error.appendTo(elment.parent().find("span.error"));
    }
});


$("#DateRangeForm").submit(function(){
    
    if($("#DateRangeForm").valid())
    {
        FromDate =$("#FromDate").val();
        ToDate = $("#ToDate").val();
        $("#DateRangeModal").modal('hide')
        GetFilteredCustomers();
        $("#FromDate").val("")
        $("#ToDate").val("")
        var today = new Date();
        document.getElementById("FromDate").setAttribute("max", today);
        document.getElementById("ToDate").setAttribute("max", today);
        $(".text-danger").text('')
    }
    
})

$(document).on('click', '#ExportToExcel', function () {
    var SearchString = $("#searchCustomer").val();
    var pageSize = $("#customerPageSize").val();
    var status = $("#statusDropDown").val();
    var time = $("#timeDropDown").val();
    
    $.ajax({
        url: "/Customer/ExportToExcel",
        type: "GET",
        data: {
            pageIndex: CustomerPageIndex,
            pageSize: pageSize,
            searchString: SearchString,
            fromDate: FromDate,
            toDate: ToDate,
            status: status,
            time: time
        },
        xhrFields :{
            responseType: 'blob'
        },
        success: function (data, status, xhr) {
            var ExcelFile = new Blob([data], { type: xhr.getResponseHeader('content-type') });
            var a = document.createElement("a");
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = 'customer.xlsx';
            a.click();
        },
        error: function (error) {
            toastr.error("Error while download excel file");
        }
    })
});