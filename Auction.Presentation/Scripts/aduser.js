(function () {
    $("#customRole").on("change", function() {
        var value = $(this).val().toLocaleLowerCase(),
            another = $(".another");

        if (value == "another")
            another.removeClass("invisible")
        else
            another.addClass("invisible");
    });

    $(".etcRole").on("change", function () {
        var that = $(this);
        var value = $(this).val().toLocaleLowerCase();
        var currAuction = $(this).parent().attr("data-auctionName")
        $(".categories[data-auctionName=" + currAuction + "]").removeClass("invisible");
        if (value == "moderator")
        {                      
            $.ajax({
                url: categoryUrl,
                type: "POST",
                dataType: "json",
                context: that,
                data: { 'data': JSON.stringify({ auctionId: currAuction }) },
                success: function (data) {
                    var res = JSON.parse(data);
                    if (Object.keys(res).length > 0) {
                        var currAuction = $(this.context).parent().attr("data-auctionName");
                        var categoryControl = $("[data-auctionName="+ currAuction+"]").find(".multiple").find("select");
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
        }
        else
        {
            $(".categories[data-auctionName=" + currAuction + "]").addClass("invisible");
        }
    });

    $("button.newUser").on("click", function () {
        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        }

        var userId = userModelId || guid();               
        var userWithPermissions = {};
        userWithPermissions.userModel = $("form.newUser").serialize();
        userWithPermissions.permissions = [];
        var currRole = $(".customRole").val();
        if (currRole.toLocaleLowerCase() === "another") {
            var auctions = $(".auctionName");
            for (var num = 0, length = auctions.length ; num < length; num++) {
                var name = $(auctions[num]).text();
                currRole = $("." + name + "_etcRole").val();
                var categories = [];
                if (currRole.toLowerCase() === "moderator") {
                    $("." + name + "_categories").find("option:selected").map(function (i, el) { return categories.push($(el).val()) });
                }
                userWithPermissions.permissions.push({
                    userId: userId,
                    auctionName: name,
                    role: currRole,
                    categories: categories
                })
            }
        }
        else {
            userWithPermissions.permissions.push({
                userId: userId,
                auctionName: null,
                role: currRole,
                categories: null
            })
        }

        var picture = $("#image")[0].files;
        var dfd = new $.Deferred();        
        userWithPermissions.file = "";
        
        function imageLoad(pic) {
            if (pic.length > 0 && (pic[0].type === "image/jpeg" || pic[0].type === "image/png")) {
                var fileReader = new FileReader();
                var fileToLoad = pic[0];
                fileReader.onload = function (fileLoadedEvent) {
                    var srcData = fileLoadedEvent.target.result;
                    var file = srcData.split(",")[1];
                    userWithPermissions.file = file;
                }
                fileReader.onloadend = function (res) {
                    dfd.resolve();
                }
                fileReader.readAsDataURL(fileToLoad);
            }
            else
                dfd.resolve();
            return dfd;
        }
       
        function ajaxUser() {
            if ($("form.newUser").valid()) {
                $.ajax({
                    url: addUserUrl,
                    async: true,
                    type: "POST",
                    dataType: "json",
                    data: { 'data': JSON.stringify({userWithPermissions}) },
                    success: function (data) {
                        var href = JSON.parse(data);
                        window.location.href = href;
                    },
                    error: function (xhr, status, error) {
                        console.log(error);
                    }
                });
            }
        }

        imageLoad(picture).done(ajaxUser);
    });
})()