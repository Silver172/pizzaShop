var ModifierGroupIds = [];

$(document).on('submit', '#addEditCategoryForm', function () {
    event.preventDefault();
    var form = $("#addEditCategoryForm")[0];
    var formData = new FormData(form);
    var id = $("#Id").val();
    $.ajax({
        url: '/Menu/AddEditCategory',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: async function (response) {
            if (response.isValid == false)
                return;
            if (response.isExist) {
                toastr.error(response.message);
            }
            if (response.isSuccess) {
                toastr.success(response.message);
                $("#addCategoryModal").modal('hide');
                await GetAllCategories();

                if (id != "" || id != null)
                    mark();
            }

        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
})

async function mark() {
    var id = $("#Id").val();
    categoryId = id;
    $(".itemCategory").removeClass("activeTabColor");
    $(".itemCategory").map(function () {
        if ($(this).data('id') == categoryId) {
            $(this).addClass("activeTabColor");
        }
    })
}

$(document).on('submit', '#addItemForm', function (event) {
    event.preventDefault();
    var form = $("#addItemForm")[0];
    var formData = new FormData(form);

    var modifiers = [];
    $(document).on('each', '#ModifiersLists .modifier-group', function () {
        var name = $(this).find(".modifierGrp-name").data("name");
        var modifierGroupId = $(this).find(".modifierGrp-name").data("id");
        var minValue = parseInt($(this).find(".min-value").val(), 10);
        var maxValue = parseInt($(this).find(".max-value").val(), 10);

        modifiers.push({
            ModifierGroupId: modifierGroupId,
            Name: name,
            Minselectionrequired: minValue,
            Maxselectionrequired: maxValue
        });
    })
    formData.append("ItemModifiers", JSON.stringify(modifiers));

    $.ajax({
        url: '/Menu/AddMenuItem',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.isValid == false) {
                return;
            }
            if (response.isSuccess) {
                toastr.success(response.message);
                $("#AddItemModel").modal("hide");
                GetFilteredItems()
            } else {
                toastr.error(response.message)
            }
        },
        error: function (error) {
            toastr.error(response.message);
        }
    });
});


$(document).on('change', '#SelectModifierGroup', function () {
    var id = $(this).val();
    var label = $("#SelectModifierGroup option:selected").text();
    if (ModifierGroupIds.includes(id)) {
        return;
    }
    $.ajax({
        url: '/Menu/GetModifiersByModifierGroupId',
        type: 'GET',
        data: { id: id, name: label },
        success: function (response) {
            ModifierGroupIds.push(id);
            $("#ModifiersLists").append(response);

        },
        error: function (error) {
        }
    });
});

$(document).on("click", ".remove-modifier", function () {
    var id = $(this).data("id");
    $("#modifier-group-" + id).remove();
    ModifierGroupIds = ModifierGroupIds.filter(matchMedia => matchMedia != id);
    $("#SelectModifierGroup").val("")
});

document.getElementById("file").addEventListener("change", function (event) {
    var file = event.target.files[0];
    const uploadfile = document.querySelector('#file');
    if (file) {
        if (!file.type.startsWith('image/')) {
            toastr.warning("Invalid file type. Please upload an image file.");
            uploadfile.value = '';
            return;
        }
        const maxSize = 2 * 1024 * 1024; // 5MB
        if (file.size > maxSize) {
            toastr.warning("File size exceeds 2MB. Please choose a smaller image.");
            uploadfile.value = '';
            return;
        }

        $(".uploadedFileName").text(file.name);
    }
});

$(document).on('submit', '#addNewModifierForm', function (event) {
    event.preventDefault();
    var form = $("#addNewModifierForm")[0];
    var formData = new FormData(form);

    $.ajax({
        url: '/Menu/AddModifier',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.isValid == false)
                return;
            if (response.isSuccess) {
                toastr.success(response.message);
                $("#addModifierModal").modal('hide');
                GetFilteredModifiers();
            }
            else {
                toastr.error(response.message);
                return;
            }
        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
})

$(document).on('submit', '#EditItemForm', function (event) {
    event.preventDefault();
    var form = $("#EditItemForm")[0];
    var formData = new FormData(form);

    var modifiers = [];
    $(document).on('each', '#EditModifiersLists .modifier-group', function () {
        var name = $(this).find(".modifierGrp-name").data("name");
        var modifierGroupId = $(this).find(".modifierGrp-name").data("id");
        var mappingId = $(this).find(".mapping-id").data("id");
        var minValue = parseInt($(this).find(".min-value").val(), 10);
        var maxValue = parseInt($(this).find(".max-value").val(), 10);
        modifiers.push({
            Id: mappingId == "" ? -1 : mappingId,
            ModifierGroupId: modifierGroupId,
            Name: name,
            Minselectionrequired: minValue,
            Maxselectionrequired: maxValue
        });
    })
    formData.append("ItemModifiers", JSON.stringify(modifiers));

    $.ajax({
        url: "/Menu/EditMenuItem",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response == null)
                window.location.href = "/Authentication/Login"
            if (response.isSuccess) {
                GetFilteredItems();
                $("#EditItemModel").modal('hide');
                toastr.success(response.message)

            } else {
                return;
            }
        },
        error: function (e) {
            toastr.error("Error")
        }
    })
})

$(document).on('change', '#SelectEditModifierGroup', function () {
    var id = $(this).val();
    var label = $("#SelectEditModifierGroup option:selected").text();
    if (ModifierGroupIds.includes(label))
        return;
    $.ajax({
        url: '/Menu/GetModifiersByModifierGroupId',
        type: 'GET',
        data: { id: id, name: label },
        success: function (response) {
            ModifierGroupIds.push(label);
            $("#EditModifiersLists").append(response);
        },
        error: function (error) {
        }
    });
});

document.getElementById("file1").addEventListener("change", function (event) {
    var file = event.target.files[0];
    const uploadfile = document.querySelector('#file1');
    if (file) {
        if (!file.type.startsWith('image/')) {
            toastr.warning("Invalid file type. Please upload an image file.");
            uploadfile.value = '';
            return;
        }
        const maxSize = 2 * 1024 * 1024; // 5MB
        if (file.size > maxSize) {
            toastr.warning("File size exceeds 2MB. Please choose a smaller image.");
            uploadfile.value = '';
            return;
        }

        $(".uploadedFileName").text(file.name);
    }
});

$(document).on("click", ".remove-modifier", function () {
    var id = $(this).data("id");
    var name = $(this).data("name");
    $("#modifier-group-" + id).remove();
    ModifierGroupIds = ModifierGroupIds.filter(m => m != name);
    $("#SelectEditModifierGroup").val("")
});

$(document).ready(function () {
    insertIds();
}) 

$(document).on('submit', '#editModifierModal', function (event) {
    event.preventDefault();
    var form = $("#editModifierForm")[0];
    var formData = new FormData(form);
    
    $.ajax({
        url: '/Menu/EditModifier',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if(response.isValid == false)
                return;
            if(response.isSuccess)
            {
                toastr.success(response.message);
                $("#editModifierModal").modal('hide');
                GetFilteredModifiers();
            }else{
                toastr.error(response.message);
            }
        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
})

$(document).ready(function () {
    $('.deleteCategory').click(function () {
        var categoryId = $(this).data("id");
        $("#DeleteCategoryId").val(categoryId);
        $("#deleteCategoryModal").modal('show');
    })

    $(document).on('click', '#AddMenuItemBtn', function () {
        $.ajax({
            url: '/Menu/AddMenuItem',
            type: 'GET',
            success: function (response) {
                $("#addItemContainer").html(response);
                $("#AddItemModel").modal('show');
                $("#MenuItemCategory").val(categoryId);
            },
            error: function (error) {
                toastr.error(error.message);
            }
        })
    })

    $(document).on('click', '#Addcategory', function () {
        $.ajax({
            url: '/Menu/AddEditCategory',
            type: 'GET',
            data: { id: null },
            success: function (response) {
                $("#addEditCategoryContainer").html(response);
                $("#addCategoryModal").modal('show');
            }
        })

    })
})

var deleteCategoryId;

async function GetAllCategories()
{
        await $.ajax({
            url: '/Menu/GetAllCategories',
            type: 'GET',
            success: function (response) {
                if(response.isSuccess == false)
                    toastr.error(error.message)
                $(".category-list").html(response);
            },
            error: function(error){
                toastr.error(error.message)
            }
        })
}

$(document).on('click', '#AddModifierModalBtn', function () {
    $("#addModifierGroupModal").modal('show');
})

$(document).on('click', '.AddExistingModifierModalBtn', function () {
    $("#ExistingModifier").modal('show');
})

$(document).on('click', '.closeModal', function () {
     var container = $(".selectedModifiers");
        container.empty();
    ExistingModifiers = []
    $("#editModifierGroupModal").modal('hide');
    $(`.modifierCheckbox`).prop("checked", false); 
})

$(document).on('click', '.closeAddModal', function () {
     var container = $(".selectedModifiers");
        container.empty();
    ExistingModifiers = []
    $("#addModifierGroupModal").modal('hide');
    $(`.modifierCheckbox`).prop("checked", false); 
})

$(document).on('click', '#SaveModifierGroupBtn', function () {
    var name = $("#modifierGroupName").val();
    var description = $("#modifierGroupDescription").val();

    if (name.trim() === "") {
            toastr.warning("Please fill Modifier Group Name");
        return;
    }

    if(name.length > 50)
    {
            toastr.warning("Name should be less then 50 character");
            return;
    }

    var postData = {
        Name: name,
        Description: description,
        Modifiers: ExistingModifiers
    };
    $.ajax({
            url: "/Menu/AddModifierGroup",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(postData),
            success: function (response) {
                if(response.isExist)
                {
                    toastr.error(response.message);
                    return;
                }
                $("#addModifierGroupModal").modal('hide');
                toastr.success("Modifier group added successfully");
                $("#modifier-list").html(response); 
                var container = $(".selectedModifiers");
                container.empty();
                $(`.modifierCheckbox`).prop("checked", false); 
            },
            error: function(error){
                toastr.error(error.message);
            }
        });
})

$(document).on('click',".dt-paging-button.next ",function(){
    updateModifierCheckboxes();
})

$(document).on('click',".dt-paging-button.previous ",function(){
   updateModifierCheckboxes();
})


function updateModifierCheckboxes() {
    $(`.modifierCheckbox`).prop("checked", false);
        ExistingModifiers.forEach(m => {
            $(`.modifierCheckbox[data-name='${m.name}']`).prop("checked", true);
        });
}

var editModifierGroupId;
$(document).on('click', '.editModifierGroup', function () {
    editModifierGroupId = $(this).data('id');
    
    $.ajax({
        url:'/Menu/EditModifierGroup',
        type: 'GET',
        data: {id:editModifierGroupId},
        success: function(response){
            if(response.data != null){
                $("#editModifierGroupName").val(response.data.name); 
                $("#editModifierGroupDesc").val(response.data.description);
                var container = $(".selectedModifiers");
                container.empty(); 
                ExistingModifiers = [];
                response.data.modifiers.map(m => ExistingModifiers.push({id:m.id,name:m.name})); 

                response.data.modifiers.forEach(modifier => {
                    var span = $(`<span class='modifier-item border rounded-pill p-1 me-1 my-2 em-list' data-id='${modifier.id}' data-name='${modifier.name}'></span>`).text(modifier.name);
                    var removeBtn = $("<i class='fa-solid fa-xmark remove  ms-1' style='cursor:pointer'></i>").click(function () {
                        ExistingModifiers = ExistingModifiers.filter(m => m.name != modifier.name);
                        $(this).parent().remove(); 
                        $(this).removeClass('em-list');
                        $(`.modifierCheckbox[data-name='${modifier.name}']`).prop("checked", false); 
                    });

                    span.append(removeBtn);
                    container.append(span);
                });
            $("#editModifierGroupModal").modal('show');
             updateModifierCheckboxes();
            }
        },
        error: function(error){
            toastr.error(error.message);
        }
    })
})

$(document).on('click', '.deleteModifierGroupIcon', function () {
    ModifierGroupId = $(this).data('id');
    $("#deleteModifierGroupModal").modal('show');
})

$(document).on('click', '#AddModifierBtn', function () {
    $("#addModifierGroupId").val(ModifierGroupId);
    $("#addModifierModal").modal("show");
})