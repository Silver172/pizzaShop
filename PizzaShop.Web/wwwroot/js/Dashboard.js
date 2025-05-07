function setGraphSize(yValues, xValues, ycValues, xcValues){
    const myChart = new Chart("revenueChart", {
        type: "line",
        data: {
            labels: xValues,
            datasets: [{
                label: "Revenue",
                fill: true,
                backgroundColor: "rgba(0, 96, 152,0.1)",
                borderColor: "rgba(0, 96, 152,0.7)",
                data: yValues
            }]
        },
    });
    const customerGrowthChart = new Chart("customerGrowthChart", {
        type: "line",
        responsive: true,
        data: {
            labels: xcValues,
            datasets: [{
                label: "Customer Growth",
                backgroundColor: "rgba(0, 96, 152,0.1)",
                borderColor: "rgba(0, 96, 152,0.7)",
                data: ycValues
            }]
        },
    });
}

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