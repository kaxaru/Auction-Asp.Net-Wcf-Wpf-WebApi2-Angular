function getCookie(name) {
    var matches = document.cookie.match(new RegExp(
      "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

var delay = +getCookie("Stopwatch") || 0;

products.forEach(function (pr, i) {
    $('#'+pr.id).find('.clock').FlipClock(pr.duration + delay , {
        countdown: true,
        stop: function () {
            this.$el.parent().parent().remove();
        }
    });
})

$(".ui.card").next().on("click", function (e) {
    $(".modal." + $(this).parent().find(".ui.card")[0].id).show();
})

$(".modal").find(".deny").on("click", function (e) {
    var id = $(this)[0].id;
    $(".ui.modal." + id).hide();
})
 
$(".pushBid").on("click", function(e){
    var id = $(this)[0].id,
        desc = $(".modal." + id).find(".description");
    var now = new Date(),
        UTC = now.getTime(),
        localOffset = (-1) * now.getTimezoneOffset() * 60000,
        currentTime = Math.round(new Date(UTC + localOffset).getTime());

    var userId = $("#userId").val();

    var bid = {
        auction: auctionName,
        userId: (userId === undefined) ? null : userId,
        datetime: currentTime,
        productId: id,
        price: +desc.find("input").val()  
    }

    $.ajax({
        url: bidUrl,
        type: "POST",
        dataType: "json",
        data: {'data': JSON.stringify(bid)},
        success: function(data)
        {
            var res = JSON.parse(data);
            if(Object.keys(res.response).length > 0)
            {
                var model = res["response"],
                    username = res["userName"];
                var el = $(".ui.card[id=" + model.ProductId + "]");
                var modalEl = $(".modal." + model.ProductId)
                el.find(".price").text("Now: "+ model.Price);
                el.find(".username").text(res.userName);
                modalEl.find(".price").text("Now: " + model.Price);
                modalEl.find(".username").text(res.userName);
            }
        },
        error: function(xhr, status, error)
        {
            console.log(error);
        }
    })
})



