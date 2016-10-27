(function () {
    $("#auctionId").on("change", function () {
        var value = $(this).val();
        category = {auctionId: value }

        $.ajax({
            url: categoryUrl,
            type: "POST",
            dataType: "json",
            data: { 'data': JSON.stringify(category) },
            success: function (data) {
                var res = JSON.parse(data);
                if (Object.keys(res).length > 0) {
                    var categoryControl = $("#categoryId");
                    categoryControl.children().remove();
                    res.forEach(function (el) {
                        var option = $("<option value=" + el.Id + ">" + el.Name + "</option>");
                        categoryControl.append(option);
                    })
                }
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        })
    })
})()