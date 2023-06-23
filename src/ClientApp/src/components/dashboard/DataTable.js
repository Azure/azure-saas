import React, { useRef, useState } from "react";
import "devextreme/data/odata/store";
import DataGrid, {
  Pager,
  Paging,
  FilterRow,
  FilterPanel,
  FilterBuilderPopup,
  Editing,
  Toolbar,
  Item,
  Selection,
  Export,
  Column,
} from "devextreme-react/data-grid";
import {
  getDataGridRef,
  handleExporting,
} from "../../helpers/datagridFunctions";
import { Link } from "react-router-dom";

const DataTable = ({
  data,
  startEdit,
  columns,
  route,
  keyExpr,
  loading,
  openConfirmationPopup,
  filterValues,
}) => {
  const [collapsed, setCollapsed] = useState(false);

  const dataGridRef = useRef(null);

  const exportFormats = ["xlsx", "pdf"];

  const pageSizes = [10, 25, 50, 100];

  function onContentReady(e) {
    getDataGridRef(dataGridRef.current);
    if (!collapsed) {
      e.component.expandRow(["EnviroCare"]);
      setCollapsed({
        collapsed: true,
      });
    }
  }

  const handleContextMenuPreparing = (e) => {
    if (e.row && e.row.rowType === "data") {
      if (!e.items) e.items = [];
      e.items.push(
        {
          text: "Edit",
          icon: "edit",
          onItemClick: () => {
            startEdit(e.row);
          },
        },
        {
          text: "Export",
          icon: "export",
          items: [
            {
              text: "Export all data to Excel",
              icon: "exportxlsx",
              onItemClick: () => {
                handleExporting("Export all data to Excel");
              },
            },
            {
              text: "Export all data to PDF",
              icon: "exportpdf",
              onItemClick: () => {
                handleExporting("Export all data to PDF");
              },
            },
          ],
        },
        {
          text: "Delete",
          icon: "trash",
          onItemClick: () => {
            openConfirmationPopup(e.row);
          },
        }
      );
    }
  };

  const handleHyperlinkClick = (e) => {
    e.target.style.color = "white";
  };

  const handleFocusedRowChanging = (e) => {
    const prevRow = e.component.getRowElement(e.prevRowIndex);
    const newRow = e.component.getRowElement(e.newRowIndex);
    prevRow[0].children[0].children[0].children[0].children[0].style.color =
      "#489AEE";
    newRow[0].children[0].children[0].children[0].children[0].style.color =
      "white";
  };

  const filterBuilder = {
    logic: "and",
    filters: filterValues.map(([field, operator, value]) => ({
      field,
      operator,
      value,
    })),
  };

  return (
    <main>
      <DataGrid
        id="dataTableGrid"
        dataSource={data}
        onContextMenuPreparing={(e) => {
          handleContextMenuPreparing(e);
        }}
        showBorders={true}
        showColumnLines={true}
        showRowLines={false}
        filterBuilder={filterBuilder}
        hoverStateEnabled={true}
        keyExpr={keyExpr}
        focusedRowEnabled={true}
        onFocusedRowChanging={(e) => handleFocusedRowChanging(e)}
        // onRowClick={(e) => setRowClickItem(e)}
        onRowDblClick={(e) => startEdit(e)}
        allowColumnReordering={true}
        allowColumnResizing={true}
        columnMinWidth={100}
        columnAutoWidth={true}
        columnHidingEnabled={true}
        ref={dataGridRef}
        loadPanel={{
          enabled: true,
          text: "Loading data...",
          visible: loading,
          showPane: true,
        }}
        onContentReady={onContentReady}
      >
        {columns.map((column) => (
          <Column
            dataField={column.dataField}
            key={column.dataField}
            alignment="left"
            cellRender={
              column?.pk === true
                ? (data) => {
                    return (
                      <div
                        data-row-key={data.key}
                        data-column-index={data.columnIndex}
                      >
                        <Link to={`/dashboard/${route}/${data.row.key}/view`}>
                          <span
                            onClick={(e) => handleHyperlinkClick(e, data)}
                            className="pk-hyperlink"
                          >
                            {data.value}
                          </span>
                        </Link>
                      </div>
                    );
                  }
                : (data) => {
                    return (
                      <div
                        data-row-key={data.key}
                        data-column-index={data.columnIndex}
                      >
                        {data.value}
                      </div>
                    );
                  }
            }
            width={column.width}
            visible={true}
          />
        ))}
        <Export
          enabled={true}
          formats={exportFormats}
          allowExportSelectedData={true}
        />
        <Editing mode="row" />
        <Selection mode="none" />
        <Toolbar visible={false}>
          <Item name="groupPanel" />
          <Item name="columnChooserButton" />
        </Toolbar>
        <FilterRow visible={true} />
        <FilterPanel visible={true} />
        <FilterBuilderPopup
          position={filterBuilderPopupPosition}
          height={"50vh"}
          width={"50vw"}
        />
        <Paging defaultPageSize={50} />
        <Pager
          showPageSizeSelector={true}
          showInfo={true}
          allowedPageSizes={pageSizes}
        />
      </DataGrid>
    </main>
  );
};

const filterBuilderPopupPosition = {
  of: window,
  at: "center",
  my: "center",
  offset: { y: 10 },
};

export default DataTable;
