(function () {

    $(".sendModel").on("click", function (e) {
        var state = $(".state");
        var products = [];
        state.each(function (ind, el) {
            var value = $(el).val();
            var stateVal = $(el).find("[value=" + value + "]").text();           
            if (stateVal !== "Draft") {
                var attrModel = $(el).attr("model");
                var attrAuction = $(el).attr("auctionName");
                var product = $("[model=" + attrModel + "]");
                var productEl = {
                    auction: attrAuction,
                    id: $("[field='Id'][model="+attrModel +"]").val(),
                    name: $("[field='Name'][model=" + attrModel + "]").val(),
                    state: stateVal
                }
                products.push(productEl);
            }
        });
        $.ajax({
            url: Url,
            type: "POST",
            dataType: "json",
            data: { 'products': JSON.stringify(products) },
            success: function (data) {
                var url = JSON.parse(data);
                location.href = url;
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    });
})()