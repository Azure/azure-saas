// Defines the columns to be used by the Booking Grid

export const userColumns = [
  {
    dataField: "username",
    width: 250,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "fullNames",
    width: 200,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "email",
    width: 250,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "telephone",
    width: 150,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "isActive",
    width: 50,
    cellRender: (data) => {
      return (
        <td data-row-key={data.key} data-column-index={data.columnIndex}>
          {data.value}
        </td>
      );
    },
  },
  {
    dataField: "Cost Center Name",
    width: 150,
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
