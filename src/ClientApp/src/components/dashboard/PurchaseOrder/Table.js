import React, { useRef, useState } from "react";
import "../../../pages/dashboard/purchase-orders/PurchaseOrder.css";
import {
  DataGrid,
  Editing,
  FilterRow,
  Column,
  Sorting,
} from "devextreme-react/data-grid";
import "devextreme/dist/css/dx.common.css";
import "devextreme/dist/css/dx.light.css";
import { items, summary, columns } from "../../../data/PurchaseOrderData";
import dataitem from "../../../utils/Order";
import { getDataGridRef } from "../../../helpers/datagridFunctions";
import { useSelector } from "react-redux";
import request from "../../../helpers/tempRequest";

// Table component

export const Table = ({
  data,
  count,
  setMessage,
  orderstate,
  updateData,
  setUpdateData,
  order,
}) => {
  const gridRef = useRef(null);
  const [collapsed, setCollapsed] = useState(false);
  const currentUser = useSelector((state) => state.user?.currentUser?.user);
  const addRef = useRef(null);

  function onContentReady(e) {
    getDataGridRef(gridRef.current);
    if (!collapsed) {
      e.component.expandRow(["EnviroCare"]);
      setCollapsed({
        collapsed: true,
      });
    }
  }

  const renderHeader = () => {
    return (
      <button ref={addRef} onClick={handleRowAdded}>
        Add new
      </button>
    );
  };
  const handleRowAdded = (e) => {
    gridRef.current.instance.addRow();
  };

  const handleRowInserted = async (rowIndex) => {
    if (
      typeof rowIndex.data.quantity === "undefined" ||
      typeof rowIndex.data.item === "undefined"
    ) {
      rowIndex.cancel = true;
      return data.reload();
    }
    const item = items.find((x) => x.name === rowIndex.data.item);
    const itemtoupdate = data.store()._array.find((x) => x.item === item.name);

    if (typeof itemtoupdate === "undefined") {
      let extendedCost = item.amount * rowIndex.data.quantity;
      let discountAmount = extendedCost * 0.05;
      rowIndex.data = new dataitem(
        item.name,
        rowIndex.data.quantity,
        item.amount,
        extendedCost,
        extendedCost * 0.25,
        extendedCost * 0.16,
        extendedCost - discountAmount,
        order,
        `${currentUser?.email}.${rowIndex.data.item}`,
        currentUser?.email
      );
      data.reload();
      count.current++;
      setMessage(`${item.name} has been added successfully.`);
      gridRef.current.instance.focus();

      if (orderstate === 0) {
        try {
          await request.post("PurchaseOrder/insertorderitems", rowIndex.data);
        } catch (e) {
          console.log(e);
        }
      }
    } else {
      let extendedCost =
        item.amount * (rowIndex.data.quantity + itemtoupdate.quantity);
      let discountAmount = extendedCost * 0.05;
      rowIndex.data = new dataitem(
        item.name,
        rowIndex.data.quantity + itemtoupdate.quantity,
        item.amount,
        extendedCost,
        extendedCost * 0.25,
        extendedCost * 0.16,
        extendedCost - discountAmount,
        order,
        `${currentUser?.email}.${rowIndex.data.item}`,
        currentUser?.email
      );

      data.store().remove(itemtoupdate);
      data.reload();
      setMessage(`${item.name} has been updated successfully.`);

      if (orderstate === 0) {
        try {
          const response = await request.put(
            "PurchaseOrder/updateorderitem",
            rowIndex.data
          );
          console.log(response.status);
        } catch (e) {
          console.log(e);
        }
      }
      return gridRef.current.instance.focus();
    }
  };

  // Recalculates values after row info is updated
  const handleRowUpdated = async (rowIndex) => {
    let unitCost = rowIndex.oldData.unitCost;
    if (typeof rowIndex.newData.quantity !== "undefined") {
      let extendedCost = unitCost * parseInt(rowIndex.newData.quantity);
      let discountAmount = extendedCost * 0.05;
      rowIndex.newData = new dataitem(
        rowIndex.oldData.item,
        parseInt(rowIndex.newData.quantity),
        unitCost,
        extendedCost,
        extendedCost * 0.25,
        discountAmount,
        extendedCost - discountAmount,
        rowIndex.oldData.partitionKey,
        `${currentUser?.email}.${rowIndex.oldData.item}`,
        rowIndex.oldData.createdBy
      );
      rowIndex.component.saveEditData();
      data.reload();
      setMessage(`${rowIndex.oldData.item} has been updated.`);
    } else if (typeof rowIndex.newData.discountAmount !== "undefined") {
      let discountAmount = parseInt(rowIndex.newData.discountAmount);
      rowIndex.newData = new dataitem(
        rowIndex.oldData.item,
        rowIndex.oldData.quantity,
        unitCost,
        rowIndex.oldData.extendedCost,
        rowIndex.oldData.taxAmount,
        discountAmount,
        rowIndex.oldData.extendedCost - discountAmount,
        rowIndex.oldData.partitionKey,
        `${currentUser?.email}.${rowIndex.oldData.item}`,
        rowIndex.oldData.createdBy
      );
      rowIndex.component.saveEditData();
      data.reload();
      setMessage(`${rowIndex.oldData.item} has been updated.`);
    } else if (typeof rowIndex.newData.item !== "undefined") {
      console.log("Item changed");
    }

    if (orderstate === 0) {
      try {
        await request.put("PurchaseOrder/updateorderitem", rowIndex.newData);
      } catch (e) {
        console.log(e);
      }
    } else {
      setUpdateData({ ...updateData, tableData: data.store()._array });
    }
  };

  // End of function

  const handleRowRemoving = async (e) => {
    e.cancel = !window.confirm(confirmDeleteMessage(e.data));
    if (orderstate === 0) {
      if (!e.cancel) {
        try {
          await request.delete("PurchaseOrder/removeorderitem", {
            data: e.data,
          });
        } catch (ex) {
          console.log(ex);
          data.store().insert(e.data);
          data.reload();
          return setMessage("Server error. Item was not removed.");
        }
      }
    } else {
      if (!e.cancel) {
        try {
          await request.delete("PurchaseOrder/deleteorderitem", {
            data: {
              itemid: e.data.id,
              orderNo: e.data.partitionKey,
            },
          });
        } catch (ex) {
          console.log(ex);
          data.store().insert(e.data);
          data.reload();
          return setMessage("Server error. Item was not removed.");
        }
      }
    }
  };

  const confirmDeleteMessage = (rowIndex) => {
    return `Are you sure you want to delete ${rowIndex.item}?`;
  };

  const onDeleteClick = (e) => {
    const dataGrid = e.component;
    dataGrid.deleteRow(e.rowIndex);
  };

  const renderDeleteLink = (cellInfo) => {
    return (
      <button
        onClick={(e) => onDeleteClick(cellInfo)}
        className="delete-btn h-auto items-center cursor-pointer"
      >
        Remove
      </button>
    );
  };

  const handleKeyPress = (e) => {
    if (e.event.keyCode === 9 && e.event.shiftKey === true) {
      handleRowAdded();
    }
  };

  return (
    <DataGrid
      id="orderGrid"
      showBorders={true}
      dataSource={data}
      //onRowUpdating={handleRowUpdating}
      hoverStateEnabled={true}
      reshapeOnPush={true}
      onRowUpdating={handleRowUpdated}
      showRowLines={true}
      onKeyDown={handleKeyPress}
      onRowRemoving={handleRowRemoving}
      columnHidingEnabled={true}
      onRowInserting={handleRowInserted}
      // onRowInserted={handleRowInserted}
      allowColumnResizing={true}
      columnMinWidth={70}
      summary={summary}
      columnAutoWidth={true}
      allowColumnReordering={true}
      ref={gridRef}
      onContentReady={onContentReady}
    >
      <Editing mode="cell" allowUpdating={true} confirmDelete={false} />
      <Column
        dataField="item"
        allowEditing={true}
        alignment="left"
        dataType="string"
        editorType="dxSelectBox"
        allowHeaderFiltering={false}
        editorOptions={{
          items: items,
          displayExpr: "name",
          valueExpr: "name",
          searchEnabled: true,
        }}
        headerCellRender={renderHeader}
      />
      <Column
        dataField="quantity"
        allowEditing={true}
        visible={true}
        dataType="number"
        max={5000}
        showSpinButtons={true}
        alignment="left"
      />
      {columns.map((column) => (
        <Column
          dataField={column.dataField}
          allowEditing={column.allowEditing}
          visible={column.visible}
          dataType="number"
          key={column.dataField}
          format={{
            type: "fixedPoint",
            precision: 2,
            currency: "KES",
            useGrouping: true,
          }}
          alignment="left"
        />
      ))}
      <Column cellRender={renderDeleteLink} width={100} alignment="center" />
      <FilterRow visible={true} />
      <Sorting mode="none" />
    </DataGrid>
  );
};

// End of component's code
