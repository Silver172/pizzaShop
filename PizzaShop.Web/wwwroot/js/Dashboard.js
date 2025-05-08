$("#select-date-range").change(function(){
    var TimePeriod = $(this).val();
    console.log(TimePeriod)

    $.ajax({
        type: 'POST',
        url: '@Url.Action("GetDashboardData","Dashboard")',
        data: { TimePeriod: TimePeriod},
        success: function(response)
        {
            $("#dashboardContainer").html(response)
        },
        error: function()
        {
            toastr.error("Error while fetching the data");
        }
    })
})