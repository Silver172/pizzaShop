$(document).on('submit', '#addEditNewTableForm', function (event) {
    event.preventDefault();
    var form = $("#addEditNewTableForm")[0];
    var formData = new FormData(form);
    $.ajax({
        url: '/TableAndSection/AddTable',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if(response.isValid == false)
                return;
            if(response.isExist)
            {
                toastr.error(response.message);
            }
            if(response.isSuccess)
            {
                toastr.success(response.message);
                $("#addEditTableModal").modal('hide');
                GetFilteredTables();
            }
        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
})

$(document).on('submit', '#AddSectionForm', function (event) {
    event.preventDefault();
    var form = $("#AddSectionForm")[0];
    var formData = new FormData(form);
    
    $.ajax({
        url: '/TableAndSection/AddSection',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if(response.isValid == false)
                return;
            if (response.isExist) {
                toastr.error(response.message);
            }
            else{
                toastr.success("New section added successfully");
                $("#addSectionModal").modal('hide');
                $("#section-list").html(response);
            }
        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
})

function EditSectionForm(modelId, event){
    event.preventDefault();
    var form = $("#EditSectionForm")[0];
    var formData = new FormData(form);
    
    $.ajax({
        url: '/TableAndSection/EditSection',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if(response.isValid == false)
                return;
            if (response.isExist) {
                toastr.error(response.message);
            }
            else{
                toastr.success("Section updated successfully");
                $("#editSectionModal").modal('hide');
                $("#section-list").html(response);

                $(".section-div").removeClass('activeTabColor');
                $(".section-div").map(function(){
                    if($(this).data('id') == modelId)
                    {
                        $(this).addClass("activeTabColor");
                    }
                })
            }
        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
}

var sectionId = null;

$(document).ready(function () {
    sectionId = $(".section-div").first().data('id');

    $(".section-div").click(function () {
        sectionId = $(this).data('id');
        $(".section-div").removeClass('activeTabColor');
        $(this).addClass("activeTabColor");
        selectedIds = [];
        $("#searchTables").val("");
        GetFilteredTables();
    });

    $(".editSection").click(function () {
        var id = $(this).data('id');

        $.ajax({
            url: '/TableAndSection/EditSection',
            type: 'GET',
            data: { id: id },
            success: function (response) {
                $("#editSectionContainer").html(response);
                $("#editSectionModal").modal('show');
            },
            error: function (error) {
                toastr.error("Please try again");
            }
        });
    });

    var deleteSectionId;
    $(".deleteSection").click(function () {
        deleteSectionId = $(this).data('id');
    });

    $("#confirmDeleteSectionBtn").click(function () {
        $.ajax({
            url: '/TableAndSection/DeleteSection',
            type: 'POST',
            data: { id: deleteSectionId },
            success: function (response) {
                if (response.isExist) {
                    toastr.error(response.message);
                    $("#deleteSectionModal").modal('hide');
                } else {
                    toastr.success("Section deleted successfully");
                    $("#deleteSectionModal").modal('hide');
                    $("#section-list").html(response);
                    sectionId = $(".section-div").first().data('id');
                    GetFilteredTables();
                }
            },
            error: function (error) {
                toastr.error("Please try again");
            }
        });
    });
});