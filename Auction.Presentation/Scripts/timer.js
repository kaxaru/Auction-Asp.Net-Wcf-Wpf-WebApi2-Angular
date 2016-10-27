$('.dateCalendar').calendar({
    monthFirst: false,
    type: 'date',
    formatter: {
        date: function (date, settings) {
            if (!date) return '';
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            return day + '/' + month + '/' + year;
        }
    }
});

$('.timeCalendar').calendar({
    monthFirst: false,
    formatter: {
        date: function (date, settings) {
            if (!date) return '';
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            return day + '/' + month + '/' + year;
        },
        time: function (date, settings) {
            if (!date) {
                return '';
            }
            settings.ampm = false;
            var hour = date.getHours();
            var minute = date.getMinutes();
            return hour + ':' + (minute < 10 ? '0' : '') + minute;
        }
    }
});