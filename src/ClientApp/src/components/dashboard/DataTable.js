import React, { useRef, useState } from "react";
import "devextreme/data/odata/store";
import DataGrid, {
  Pager,
  Paging,
  FilterRow,
  FilterPanel,
  FilterBuilderPopup,
  SearchPanel,
  Editing,
  Toolbar,
  Item,
  Selection,
  Export,
} from "devextreme-react/data-grid";
import { ContextMenu } from "devextreme-react/context-menu";
import {
  getDataGridRef,
  handleExporting,
} from "../../helpers/datagridFunctions";

const DataTable = ({
  data,
  startEdit,
  setRowClickItem,
  columns,
  keyExpr,
  loading,
  openConfirmationPopup,
  filterValues,
}) => {
  const [collapsed, setCollapsed] = useState(false);

  const [contextMenuCoords, setContextMenuCoords] = useState({ x: 0, y: 0 });

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

  const handleContextMenu = (e) => {
    e.preventDefault();
    setContextMenuCoords({ x: e.clientX, y: e.clientY });
  };

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
        id="bookingGrid"
        className={"dx-card wide-card"}
        dataSource={data}
        columns={columns}
        onContextMenu={handleContextMenu}
        onContextMenuPreparing={(e) => {
          handleContextMenuPreparing(e);
        }}
        showBorders={false}
        filterBuilder={filterBuilder}
        hoverStateEnabled={true}
        keyExpr={keyExpr}
        focusedRowEnabled={true}
        onRowClick={(e) => setRowClickItem(e)}
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
        <Export
          enabled={true}
          formats={exportFormats}
          allowExportSelectedData={true}
        />
        <Editing mode="row" />
        <Selection mode="none" />
        <Toolbar>
          <Item name="groupPanel" />
          <Item name="columnChooserButton" />
          <Item name="searchPanel" />
        </Toolbar>
        <FilterRow visible={true} />
        <FilterPanel visible={true} />
        <FilterBuilderPopup
          position={filterBuilderPopupPosition}
          height={"50vh"}
          width={"50vw"}
        />

        <SearchPanel visible={true} />
        <Paging defaultPageSize={10} />
        <Pager
          showPageSizeSelector={true}
          showInfo={true}
          allowedPageSizes={pageSizes}
        />
      </DataGrid>
      {contextMenuCoords && (
        <ContextMenu
          coords={contextMenuCoords}
          onHiding={() => setContextMenuCoords(null)}
        />
      )}
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
