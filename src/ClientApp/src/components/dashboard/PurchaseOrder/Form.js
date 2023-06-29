import React, { useState, useEffect } from "react";
import "../../../pages/dashboard/purchase-orders/PurchaseOrder.css";
import "devextreme/dist/css/dx.common.css";
import "devextreme/dist/css/dx.light.css";
import { centerOptions } from "../../../data/PurchaseOrderData";
import SelectBox from "devextreme-react/select-box";
import TextBox from "devextreme-react/text-box";
import DateBox from "devextreme-react/date-box";
import TextArea from "devextreme-react/text-area";
import NumberBox from "devextreme-react/number-box";
import { useSelector } from "react-redux";
import request from "../../../helpers/tempRequest";

// Getting date today

const today = new Date();
const year = today.getFullYear();
const month = String(today.getMonth() + 1).padStart(2, "0");
const day = String(today.getDate()).padStart(2, "0");
const dateString = `${year}-${month}-${day}`;

// End of function

// Form Component

const Form = ({
  formUpdateData,
  setLoading,
  updateData,
  setUpdateData,
  initialRender,
  setInitialRender,
  orderstate,
  setMessage,
}) => {
  const [costCenter, setCostCenter] = useState();
  const [supplier, setSupplier] = useState();
  const [shipsTo, setShipsTo] = useState();
  const [orderDate, setOrderDate] = useState(dateString);
  const [orderAmount, setOrderAmount] = useState();
  const [firstDeliveryDate, setfirstDeliveryDate] = useState(dateString);
  const [narration, setNarration] = useState();
  const [deliveryPeriod, setDeliveryPeriod] = useState();
  const [driverDetails, setdriverDetails] = useState();
  const [orderNumber, setOrderNumber] = useState(0);
  const currentUser = useSelector((state) => state.user?.currentUser?.user);

  useEffect(
    () => {
      setCostCenter(formUpdateData.costCenter);
      setSupplier(formUpdateData.supplier);
      setShipsTo(formUpdateData.shipsTo);
      setOrderDate(dateString);
      setOrderAmount(formUpdateData.orderAmount);
      setDeliveryPeriod(formUpdateData.deliveryPeriod);
      setfirstDeliveryDate(dateString);
      setdriverDetails(formUpdateData.vehicleDetails);
      setNarration(formUpdateData.narration);
      setOrderNumber(formUpdateData.orderNumber);
      setInitialRender(false);
      setLoading(false);
    },
    // eslint-disable-next-line
    [formUpdateData]
  );

  useEffect(
    () => {
      const data = {
        costCenter: costCenter ?? "",
        supplier: supplier ?? "",
        shipsTo: shipsTo ?? "",
        orderDate: orderDate ?? dateString,
        orderAmount: orderAmount ?? 0,
        deliveryPeriod: deliveryPeriod ?? 0,
        firstDeliveryDate: firstDeliveryDate ?? dateString,
        vehicleDetails: driverDetails ?? "",
        narration: narration ?? "",
        orderNumber: orderNumber ?? 0,
        id: currentUser.email ?? "",
      };

      if (initialRender === true) {
        return;
      } else if (orderstate === 1) {
        setUpdateData({ ...updateData, formData: data });
        return;
      }
      updateToCosmos(data);
    },
    // eslint-disable-next-line
    [
      costCenter,
      supplier,
      shipsTo,
      orderDate,
      orderAmount,
      firstDeliveryDate,
      narration,
      deliveryPeriod,
      driverDetails,
    ]
  );

  const updateToCosmos = async (data) => {
    setMessage("Saving...");
    try {
      await request.put("/PurchaseOrder/updateorderinfo", data);
    } catch (e) {
      console.log(e);
      setMessage("Network problem");
    }
    setMessage();
  };

  return (
    <div className="po-form">
      <div className="w-full flex flex-wrap lg:w-[75%] box-border justify-between  gap-2">
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Cost Center:
          </label>
          <SelectBox
            items={centerOptions}
            searchEnabled={true}
            valueExpr="text"
            displayExpr="text"
            height={26}
            value={costCenter}
            onValueChanged={(e) => {
              setCostCenter(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Ships to:
          </label>
          <SelectBox
            items={centerOptions}
            valueExpr="text"
            displayExpr="text"
            height={26}
            value={shipsTo}
            onValueChanged={(e) => {
              setShipsTo(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Order Amount:
          </label>
          <NumberBox
            height={26}
            value={orderAmount}
            onValueChanged={(e) => {
              setOrderAmount(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            First Delivery Date:
          </label>
          <DateBox
            defaultValue={dateString}
            height={26}
            value={firstDeliveryDate}
            onValueChanged={(e) => {
              setfirstDeliveryDate(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Supplier:
          </label>
          <SelectBox
            items={centerOptions}
            valueExpr="text"
            displayExpr="text"
            height={26}
            value={supplier}
            onValueChanged={(e) => {
              setSupplier(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Order Date:
          </label>
          <DateBox
            defaultValue={dateString}
            height={26}
            value={orderDate}
            placeholder="Type order amount here"
            onValueChanged={(e) => {
              setOrderDate(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Delivery Period (Days):
          </label>
          <NumberBox
            height={26}
            value={deliveryPeriod}
            placeholder="Type delivery period here"
            onValueChanged={(e) => {
              setDeliveryPeriod(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
        <div className="flex flex-col gap-3 md:gap-0 md:flex-row justify-between w-full md:w-[48%]">
          <label className="label-control text-xs text-gray-600">
            Vehicle/Driver Details:
          </label>
          <TextBox
            height={26}
            value={driverDetails}
            placeholder="Type vehicle/driver here"
            onValueChanged={(e) => {
              setdriverDetails(e.value);
            }}
            style={{ fontSize: "12px" }}
            className=" border pl-1 w-full md:w-[70%]  outline-none"
          />
        </div>
      </div>

      <div className="flex justify-between box-border flex-col gap-3 md:flex-row w-full md:w-[55%]">
        <label className="label-control text-xs text-gray-600">
          Narration:
        </label>
        <TextArea
          type="text"
          height="5vh"
          placeholder="Type narration here"
          value={narration}
          onValueChanged={(e) => {
            setNarration(e.value);
          }}
          style={{ fontSize: "12px" }}
          className=" border resize-none text-xs pl-1 w-full md:w-[70%] lg:w-[80%] outline-none"
        />
      </div>
    </div>
  );
};

// End of component code

export default Form;
