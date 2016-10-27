$(document).ready(function () {
    function getCookie(name) {
        var matches = document.cookie.match(new RegExp(
          "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
        ));
        return matches ? decodeURIComponent(matches[1]) : undefined;
    }

    function setCookie(name, value, options) {
        options = options || {};

        var expires = options.expires;

        if (typeof expires == "number" && expires) {
            var d = new Date();
            d.setTime(d.getTime() + expires * 1000);
            expires = options.expires = d;
        }
        if (expires && expires.toUTCString) {
            options.expires = expires.toUTCString();
        }

        value = encodeURIComponent(value);

        var updatedCookie = name + "=" + value;

        for (var propName in options) {
            updatedCookie += "; " + propName;
            var propValue = options[propName];
            if (propValue !== true) {
                updatedCookie += "=" + propValue;
            }
        }

        document.cookie = updatedCookie;
    }

    function deleteCookie(name) {
        setCookie(name, "", {
            expires: -1
        })
    }

    var userNotFound = getCookie("userNotFound") || null;
    if (userNotFound != null)
    {
        var modal = $(".loginModal");
        var span = $("<div class='userErr center'>User not found</div>");
        modal.append(span);
        $(".loginModal").modal('show');
        deleteCookie("userNotFound");
    }
    else {
        var err = $(".loginModal").find(".userErr");
        if(err != undefined)
        {
            err.remove();
        }
    }


    var close = $(".modal").find(".close");
    $(close).on("click", function (e) {
        var that = $(this);
        var modal = that.parent();
        while (true)
        {
            if (modal.hasClass("modal"))
            {
                modal.modal("hide");
                break;
            }
            modal = modal.parent();
        }
    });

    $('.right.menu.open').on("click",function(e){
        e.preventDefault();
        $('.ui.vertical.menu').toggle();
    });
    
    $('.ui.dropdown').dropdown();

    $(".login").on("click", function (e) {
        e.preventDefault();
        $(".loginModal").modal('show');
    })

    $("#divUpload").on("click", function () {
        $('#image').click();
    });
});