export const bookingColumns = [
  { dataField: "bookingId", caption: "Booking ID", width: 70 },
  {
    dataField: "bookingType",
    caption: "Booking Type",
    width: 90,
  },
  { dataField: "externalSchemeAdmin", width: 100 },
  {
    dataField: "retirementSchemeName",
    caption: "Retirement Scheme Name",
    width: 100,
  },
  { dataField: "courseDate", caption: "Course Date", width: 100 },
  { dataField: "schemePosition", width: 90 },
  { dataField: "trainingVenue", width: 150 },
  { dataField: "paymentMode", width: 150 },
  { dataField: "additionalRequirements", width: 150 },
];

export const bookingFilterValues = [
  ["bookingType", "anyof", ["First Time", "Retake", "Resit"]],
];

export const orderFilterValues = [
  ["supplier", "anyof", ["Eldoret", "Mombasa", "Nyeri", "Kisumu"]],
];
