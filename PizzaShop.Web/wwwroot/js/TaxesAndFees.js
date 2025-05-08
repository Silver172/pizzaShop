$(document).on('submit', '#addEditNewTaxForm', function (event) {
    event.preventDefault();
    var form = $("#addEditNewTaxForm")[0];
    var formData = new FormData(form);

    var isValid = validateTaxValue();
    if (!isValid) {
        return;
    }

    $.ajax({
        url: '/TaxesAndFees/AddEditTax',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.isValid == false)
                return;
            if (response.isExist) {
                toastr.error(response.message);
            }
            if (response.isSuccess) {
                toastr.success(response.message);
                $("#addEditTaxModal").modal('hide');
                GetFilteredTaxes();
            }
        },
        error: function (error) {
            toastr.error(error.message)
        }
    })
})


function validateTaxValue() {
    var taxType = $('#Type').val();
    var taxValue = parseFloat($('#TaxValue').val());

    if (taxType === 'Percentage' && (taxValue < 0 || taxValue > 100)) {
        toastr.error("Tax Value must be between 0 and 100.");
        return false;
    } else if (taxType === 'Flat Amount' && taxValue < 0) {
        toastr.error("Tax Value must be non-negative.");
        return false;
    }else if(taxType === 'Flat Amount' && taxValue > 9999999.99) {
        toastr.error("Invalid Tax Value.");
        return false;
    }
    return true;
}