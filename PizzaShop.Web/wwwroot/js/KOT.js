$(document).on('click', '.kot-card', function () {
    const data = $(this).data("id");
    $.ajax({
        url: '/KOTOrderApp/GetOrderDetails',
        type: 'GET',
        data: { data: JSON.stringify(data) },
        success: function (response) {
            $("#tableData").html(response);

            if (filterBy === "In Progress") {
                $('#markStatusBtn').text("Mark As Prepared");
            } else {
                $('#markStatusBtn').text("Mark As In Progress");
            }

            $('#foodItemModal').modal('show');
        },
        error: function () {
            toastr.error("An error occurred while fetching order details. Please try again.");
        }
    });
});

function nextPreviousBtn(PageIndex, TotalRecord, pageSize) {
    $("#NextKot").attr("disabled", (PageIndex * pageSize >= TotalRecord));
    $("#PreviousKot").attr("disabled", (PageIndex == 1));
}

$(document).on("click", ".plus", function () {
    const input = $(this).siblings(".input-box");
    const max = parseInt(input.attr("max"));
    const value = parseInt(input.val());
    input.val(Math.min(value + 1, max));
});

$(document).on("click", ".minus", function () {
    const input = $(this).siblings(".input-box");
    const min = parseInt(input.attr("min"));
    const value = parseInt(input.val());
    input.val(Math.max(value - 1, min));
});

var kotCategoryId = 0;
var filterBy = "In Progress";
var PageIndex = 1;

function GetFilteredKot() {
    $.ajax({
        url: '/KOTOrderApp/GetKOTByCategoryId',
        type: 'GET',
        data: {
            categoryId: kotCategoryId,
            pageIndex: PageIndex,
            filterBy: filterBy,
        },
        success: function (response) {
            $("#kotListContainer").html(response);
        },
        error: function (error) {
            toastr.error("Please try again");
        }
    })
}

$(document).on('click', '.kot-category', function () {
    kotCategoryId = $(this).data('id');
    var kotCategoryName = $(this).data('name');
    $(".categoryName").text(kotCategoryName);
    filterBy = "In Progress"
    $("#inProgress").addClass('btn-primary');
    $("#inProgress").removeClass('btn-outline-primary');
    $("#ready").removeClass('btn-primary');
    $("#ready").addClass('btn-outline-primary');
    GetFilteredKot();
})


$("#inProgress").click(function () {
    filterBy = "In Progress";
    $("#inProgress").addClass('btn-primary');
    $("#inProgress").removeClass('btn-outline-primary');
    $("#ready").removeClass('btn-primary');
    $("#ready").addClass('btn-outline-primary');
    GetFilteredKot();
})

$("#ready").click(function () {
    filterBy = "Ready";
    $("#inProgress").removeClass('btn-primary');
    $("#inProgress").addClass('btn-outline-primary');
    $("#ready").addClass('btn-primary');
    $("#ready").removeClass('btn-outline-primary');
    GetFilteredKot();
})

$("#PreviousKot").click(function () {
    if (PageIndex > 0) {
        PageIndex -= 1;
        GetFilteredKot();
    }
})

$("#NextKot").click(function () {
    if (PageIndex) {
        PageIndex += 1;
        GetFilteredKot();
    }
})

$("#All").click(function () {
    kotCategoryId = 0;
    filterBy = "In Progress";
    $("#inProgress").addClass('btn-primary');
    $("#inProgress").removeClass('btn-outline-primary');
    $("#ready").removeClass('btn-primary');
    $("#ready").addClass('btn-outline-primary');
    GetFilteredKot();
})