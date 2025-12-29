window.initFlatpickr = function (dotNetHelper, unavailableDates) {
    flatpickr("#dateRange", {
        mode: "range",
        minDate: "today",
        disable: unavailableDates.map(date => {
            return {
                from: date,
                to: date,
                className: "booked-date"
            };
        }),
        dateFormat: "Y-m-d",
        onChange: function (selectedDates) {
            if (selectedDates.length === 2) {
                dotNetHelper.invokeMethodAsync("SetBookingDates",
                    selectedDates[0].toISOString(),
                    selectedDates[1].toISOString());
            }
        }
    });
};