export const formfields = {
  "Cost Center": "",
  Supplier: "",
  "Ship To": "",
  "Order Date": new Date(),
  "Order Amount": "",
  "Delivery Period (Days)": "",
  "First Delivery Date": new Date(),
  "Vehicle/Driver Details": "",
  Narration: "",
};

export const items = [
  { key: 1, name: "Blue Band", amount: 600 },
  { key: 2, name: "Indomie", amount: 50 },
  { key: 3, name: "Biscuits", amount: 40 },
  { key: 4, name: "Rice 1KG", amount: 260 },
  { key: 5, name: "Mumias Sugar 1KG", amount: 250 },
  { key: 6, name: "Cake", amount: 600 },
  { key: 7, name: "Drinking Chocolate", amount: 250 },
  { key: 8, name: "Minute Maid", amount: 200 },
];

// Describes the format for DevExtreme to create a summary of the table
export const summary = {
  totalItems: [
    {
      column: "Unit Cost",
      summaryType: "sum",
      valueFormat: {
        type: "fixedPoint",
        precision: 2,
        currency: "KES",
        useGrouping: true,
      },
      displayFormat: "{0}",
    },
    {
      column: "Extended Cost",
      summaryType: "sum",
      valueFormat: {
        type: "fixedPoint",
        precision: 2,
        currency: "KES",
        useGrouping: true,
      },
      displayFormat: "{0}",
    },
    {
      column: "Tax Amount",
      summaryType: "sum",
      valueFormat: {
        type: "fixedPoint",
        precision: 2,
        currency: "KES",
        useGrouping: true,
      },
      displayFormat: "{0}",
    },
    {
      column: "Discount Amount",
      summaryType: "sum",
      valueFormat: {
        type: "fixedPoint",
        precision: 2,
        currency: "KES",
        useGrouping: true,
      },
      displayFormat: "{0}",
    },
    {
      column: "Line Total",
      summaryType: "sum",
      valueFormat: {
        type: "fixedPoint",
        precision: 2,
        currency: "KES",
        useGrouping: true,
      },
      displayFormat: "{0}",
    },
  ],
};

// End of definition

// Describes the format for DevExtreme to create a summary of the table
export const reportSummary = {
  totalItems: [
    {
      column: "Order Amount",
      summaryType: "sum",
      valueFormat: {
        type: "fixedPoint",
        precision: 2,
        currency: "KES",
        useGrouping: true,
      },
      displayFormat: "{0}",
    }
  ],
};

// Sample center options

export const centerOptions = [
  { value: 1, text: "Nairobi" },
  { value: 2, text: "Kisumu" },
  { value: 3, text: "Eldoret" },
  { value: 4, text: "Thika" },
  { value: 5, text: "Nyeri" },
  { value: 6, text: "Mombasa" },
  { value: 7, text: "Embu" },
];

// Defines the columns to be used by the Purchase Order Grid

export const columns = [
  { dataField: "itemNumber", allowEditing: false, width: 50, visible: false },
  { dataField: "unitCost", visible: true, allowEditing: false },
  { dataField: "extendedCost", visible: true, allowEditing: false },
  { dataField: "taxAmount", visible: true, allowEditing: false },
  { dataField: "supplierCost", visible: false, allowEditing: false },
  { dataField: "discountAmount", visible: true, allowEditing: true },
  { dataField: "lineTotal", visible: true, allowEditing: false },
  { dataField: "discountType", visible: false, allowEditing: false },
  { dataField: "OUM", visible: false, allowEditing: false },
  { dataField: "discountRate", visible: false, allowEditing: false },
  { dataField: "taxCode", visible: false, allowEditing: false },
  { dataField: "taxRate", visible: false, allowEditing: false },
];

// End of definition

// Defines columns used by orders grid

export const orderColumns = [
  { dataField: "orderNumber", alignment:'left' },
  { dataField: "costCenter" },
  { dataField: "supplier" },
  { dataField: "shipsTo" },
  { dataField: "orderDate" },
  { dataField: "orderAmount", alignment:'left', format: {
    type: "fixedPoint",
    precision: 2,
    currency: "KES",
    useGrouping: true,
  } },
  { dataField: "deliveryPeriod", alignment:'left' },
  { dataField: "firstDeliveryDate" },
  { dataField: "vehicleDetails", alignment:'left' }

];

//

// Defines columns used by orders grid

export const reportColumns = [
  { dataField: "orderNumber", alignment:'left' },
  { dataField: "orderDate" },
  { dataField: "deliveryPeriod", alignment:'left' },
  { dataField: "vehicleDetails", alignment:'left' },
  { dataField: "orderAmount", alignment:'left', format: {
    type: "fixedPoint",
    precision: 2,
    currency: "KES",
    useGrouping: true,
  } }

];

//

// Defines the columns to be used by the Booking Grid

export const bookingColumns = [
  {
    dataField: "bookingId",
    width: 70,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "bookingType",

    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "externalSchemeAdmin",

    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "retirementSchemeName",
    width: 300,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "schemePosition",
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "trainingVenue",

    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
];

// End of definition
